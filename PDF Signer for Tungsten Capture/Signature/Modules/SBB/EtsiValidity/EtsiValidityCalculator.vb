Imports System.Linq

''' <summary>Pure ETSI EN 319 102-1 long-term validity-expiry calculator: archival expiry (extends across missing data) and evidence expiry (freezes on it). No SecureBlackbox, no wall clock.</summary>
Friend Module EtsiValidityCalculator
    Private Enum WalkMode
        Archival
        Evidence
    End Enum

    Private Structure SelfValidity
        Friend EndTime As DateTimeOffset
        Friend Missing As Boolean
        Friend Revocation As DateTimeOffset?
    End Structure

    Private Structure WalkResult
        Friend Reach As DateTimeOffset
        Friend Missing As Boolean
        Friend Gap As Boolean
        Friend GapBefore As DateTimeOffset?
    End Structure

    ''' <summary>Computes the document's end-of-validity outcome at AtTime from its signatures' evidence chains.</summary>
    Friend Function Compute(Input As DocumentValidationInput, AtTime As DateTimeOffset) As DocumentValidityResult
        Dim per As List(Of PerSignatureResult) = Input.Signatures.Select(Function(s) Evaluate(s, AtTime, Input.Sunset)).ToList()

        If per.Count = 0 Then
            Return New DocumentValidityResult With {.Expiry = AtTime, .EvidenceExpiry = AtTime, .Status = ValidityStatus.Expired, .PerSignature = per}
        End If

        ' All: the document is valid only while every signature is (min); Any: max.
        Dim driver As PerSignatureResult = If(Input.Aggregation = AggregationPolicy.All,
                                              per.OrderBy(Function(p) p.Expiry).First(),
                                              per.OrderByDescending(Function(p) p.Expiry).First())
        Dim evidenceExpiry As DateTimeOffset = If(Input.Aggregation = AggregationPolicy.All,
                                                  per.Min(Function(p) p.EvidenceExpiry),
                                                  per.Max(Function(p) p.EvidenceExpiry))

        Dim status As ValidityStatus = driver.Status
        If status = ValidityStatus.ValidUntil AndAlso driver.Expiry < AtTime Then status = ValidityStatus.Expired

        Return New DocumentValidityResult With {.Expiry = driver.Expiry, .EvidenceExpiry = evidenceExpiry, .Status = status, .PerSignature = per}
    End Function

    Private Function Evaluate(S As SignatureInput, AtTime As DateTimeOffset, Sunset As AlgorithmSunsetTable) As PerSignatureResult
        Dim trace As New List(Of String)
        Dim layers As List(Of Layer) = S.CoveringTimestamps.OrderBy(Function(l) l.TrustedTime).ToList()

        ' T0 is the first covering timestamp's genTime, else now; the claimed time is never trusted.
        Dim t0 As DateTimeOffset = If(layers.Count > 0, layers(0).TrustedTime, AtTime)
        Dim self As SelfValidity = SelfValidityEnd(S.SignerPath, S.Algorithm, Nothing, S.MissingData, Sunset)
        Dim selfId As String = If(S.DisplayId, S.Id)
        trace.Add($"{selfId}: T0={Fmt(t0)} E0={Fmt(self.EndTime)}{If(self.Missing, " (hiányzó adat)", "")}")

        If t0 < S.SignerLeaf.NotBefore Then Return Invalid(S, S.SignerLeaf.NotBefore, "a token megelőzi saját tanúsítványát", trace)
        If self.Revocation.HasValue AndAlso t0 >= self.Revocation.Value Then
            Return Invalid(S, self.Revocation.Value, $"visszavonás ({Fmt(self.Revocation.Value)}) a létezési bizonyíték előtt", trace)
        End If

        ' A later-in-document-order timestamp with an earlier genTime is a suspect timeline.
        Dim suspect As Boolean = False
        For i As Integer = 1 To layers.Count - 1
            If layers(i).DocumentOrderIndex < layers(i - 1).DocumentOrderIndex Then suspect = True
        Next

        Dim arch As WalkResult = Walk(selfId, layers, self.EndTime, self.Missing, Sunset, WalkMode.Archival, trace)
        Dim evid As WalkResult = Walk(selfId, layers, self.EndTime, self.Missing, Sunset, WalkMode.Evidence, trace)

        Dim status As ValidityStatus
        If arch.Gap Then
            status = ValidityStatus.BrokenChain
        ElseIf arch.Missing Then
            status = ValidityStatus.MissingRevocationData
        ElseIf suspect Then
            status = ValidityStatus.SuspectTimeline
        Else
            status = If(arch.Reach >= AtTime, ValidityStatus.ValidUntil, ValidityStatus.Expired)
        End If

        ' Missing embedded evidence is surfaced only when the archival outcome is otherwise clean.
        If evid.Reach < arch.Reach AndAlso (status = ValidityStatus.ValidUntil OrElse status = ValidityStatus.Expired) Then
            status = ValidityStatus.MissingRevocationData
        End If

        trace.Add($"Eredmény: {status}, archív-lejárat={Fmt(arch.Reach)}, bizonyítható-lejárat={Fmt(evid.Reach)}")
        Return New PerSignatureResult With {
            .SignatureId = S.Id, .DisplayId = S.DisplayId, .Expiry = arch.Reach, .EvidenceExpiry = evid.Reach,
            .Status = status, .GapBefore = arch.GapBefore, .Trace = trace}
    End Function

    ''' <summary>One chain walk; in Evidence mode the walk freezes as soon as the reach is derived from missing data.</summary>
    Private Function Walk(SelfId As String, Layers As List(Of Layer), E0 As DateTimeOffset, E0Missing As Boolean,
                          Sunset As AlgorithmSunsetTable, Mode As WalkMode, Trace As List(Of String)) As WalkResult
        Dim allowCrossMissing As Boolean = Mode = WalkMode.Archival
        Dim tag As String = If(allowCrossMissing, "arch", "evid")
        Dim result As New WalkResult With {.Reach = E0, .Missing = E0Missing}

        If Not allowCrossMissing AndAlso result.Missing Then
            Trace.Add($"  [{tag}] {SelfId} hiányzó adat → befagyasztva {Fmt(result.Reach)}")
            Return result
        End If

        For i As Integer = 0 To Layers.Count - 1
            Dim l As Layer = Layers(i)
            Dim ei As SelfValidity = SelfValidityEnd(l.CertPath, l.Algorithm, l.MissingRevocationFrom, l.MissingData, Sunset)
            Dim layerId As String = If(l.DisplayId, $"L{i + 1}")
            ' A layer that predates its own certificate, or was revoked before its genTime, cannot contribute.
            Dim rejected As Boolean = l.TrustedTime < l.CertPath(0).NotBefore OrElse (ei.Revocation.HasValue AndAlso l.TrustedTime >= ei.Revocation.Value)

            If l.TrustedTime <= result.Reach Then
                If Not rejected AndAlso ei.EndTime > result.Reach Then
                    result.Reach = ei.EndTime
                    result.Missing = ei.Missing
                End If
                Trace.Add($"  [{tag}] {layerId}: T={Fmt(l.TrustedTime)} E={Fmt(ei.EndTime)} → reach={Fmt(result.Reach)}{If(result.Missing, " (hiányzó adat)", "")}")
                If Not allowCrossMissing AndAlso result.Missing Then
                    Trace.Add($"  [{tag}] befagyasztva {Fmt(result.Reach)} (hiányzó adat – időbélyeg nem terjeszti tovább)")
                    Exit For
                End If
            Else
                result.Gap = True
                result.GapBefore = l.TrustedTime
                Trace.Add($"  [{tag}] {layerId}: T={Fmt(l.TrustedTime)} > reach {Fmt(result.Reach)} → lánc megszakadt")
                Exit For
            End If
        Next
        Return result
    End Function

    ''' <summary>E_i = min(path NotAfter, revocation cap, missing-data cap, algorithm sunset); missing data keeps the end "soft".</summary>
    Private Function SelfValidityEnd(Path As List(Of CertInfo), Alg As AlgorithmId, MissingRevocationFrom As DateTimeOffset?,
                                     MissingData As Boolean, Sunset As AlgorithmSunsetTable) As SelfValidity
        Dim result As New SelfValidity With {.EndTime = DateTimeOffset.MaxValue}
        For Each c As CertInfo In Path
            If c.NotAfter < result.EndTime Then result.EndTime = c.NotAfter
        Next

        For Each c As CertInfo In Path
            If c.Revocation IsNot Nothing AndAlso c.Revocation.Revoked Then
                Dim ri As DateTimeOffset = c.Revocation.EffectiveR()
                If Not result.Revocation.HasValue OrElse ri < result.Revocation.Value Then result.Revocation = ri
                If ri < result.EndTime Then result.EndTime = ri
            End If
        Next

        If MissingRevocationFrom.HasValue AndAlso MissingRevocationFrom.Value < result.EndTime Then
            result.EndTime = MissingRevocationFrom.Value
            result.Missing = True
        End If

        Dim sunsetCap As DateTimeOffset = Sunset.EarliestSunset(Alg)
        If sunsetCap < result.EndTime Then
            result.EndTime = sunsetCap
            result.Missing = False
        End If

        ' A missing CA certificate / CRL / OCSP dominates: applied last so a public sunset cap cannot clear it.
        If MissingData Then result.Missing = True
        Return result
    End Function

    Private Function Invalid(S As SignatureInput, Boundary As DateTimeOffset, Reason As String, Trace As List(Of String)) As PerSignatureResult
        Trace.Add($"Érvénytelen: {Reason}; lejárat={Fmt(Boundary)}")
        Return New PerSignatureResult With {
            .SignatureId = S.Id, .DisplayId = S.DisplayId, .Expiry = Boundary, .EvidenceExpiry = Boundary,
            .Status = ValidityStatus.Invalid, .Trace = Trace}
    End Function

    ''' <summary>Compact UTC form for the dense trace lines; ±∞ for the two extreme values.</summary>
    Friend Function TraceMoment(T As DateTimeOffset) As String
        Return If(Sentinel(T), MomentFormat.Display(T, Utc:=True))
    End Function

    ''' <summary>Dual local (UTC) form for operator-facing outcome lines; ±∞ for the two extreme values.</summary>
    Friend Function OutcomeMoment(T As DateTimeOffset) As String
        Return If(Sentinel(T), MomentFormat.Log(T))
    End Function

    Private Function Sentinel(T As DateTimeOffset) As String
        If T = DateTimeOffset.MaxValue Then Return "+∞"
        If T = DateTimeOffset.MinValue Then Return "-∞"
        Return Nothing
    End Function

    Private Function Fmt(T As DateTimeOffset) As String
        Return TraceMoment(T)
    End Function
End Module
