Imports System.Globalization
Imports nsoftware.SecureBlackbox

''' <summary>Maps a parsed PDFVerifier onto the pure validity model: entities, covering layers, times, and per-entity MissingData from an offline re-validation at each proof-of-existence moment.</summary>
Friend Module EtsiValidityInputBuilder
    ''' <summary>ChainValidationDetails bits meaning "material is missing": cvrUnknownCA, cvrCRLNotVerified, cvrOCSPNotVerified.</summary>
    Private Const MissingDataMask As Integer = &H20 Or &H80 Or &H100
    ''' <summary>Seconds an open-ended OCSP response (no nextUpdate) stays acceptable after thisUpdate; Adobe-parity window.</summary>
    Private Const OpenEndedRevocationGraceSeconds As Integer = 300
    Private Const SbbMomentFormat As String = "yyyy-MM-dd HH:mm:ss"

    ''' <summary>Builds the document validation input from a PDFVerifier that has already run Verify() offline; Processing receives Hungarian per-entity progress lines.</summary>
    Friend Function From(Verifier As PDFVerifier, AtTime As DateTimeOffset, Processing As Action(Of String), Trace As Action(Of String)) As DocumentValidationInput
        Dim poe As New PoeValidator(Verifier, Processing, Trace)
        Dim naming As EntityNaming = EntityNaming.FromVerifier(Verifier)

        Dim docTimestamps As New List(Of KeyValuePair(Of Integer, PDFSignature))
        Dim ordinary As New List(Of KeyValuePair(Of Integer, PDFSignature))
        For i As Integer = 0 To Verifier.Signatures.Count - 1
            Dim s As PDFSignature = Verifier.Signatures(i)
            If s.SignatureType = PDFSignatureTypes.pstDocumentTimestamp Then
                docTimestamps.Add(New KeyValuePair(Of Integer, PDFSignature)(i, s))
            Else
                ordinary.Add(New KeyValuePair(Of Integer, PDFSignature)(i, s))
            End If
        Next

        Dim signatures As New List(Of SignatureInput)
        For Each entry As KeyValuePair(Of Integer, PDFSignature) In ordinary
            Dim order As Integer = entry.Key
            Dim sig As PDFSignature = entry.Value
            Dim sigId As String = EntityNaming.EntityKey(sig.EntityLabel, order, False)
            Dim covering As New List(Of Layer)

            ' The signature's own embedded timestamps (ParentEntity = EntityLabel), numbered S0T0, S0T1, …
            Dim ownTimestamps As Integer = 0
            For j As Integer = 0 To Verifier.Timestamps.Count - 1
                Dim ts As TimestampInfo = Verifier.Timestamps(j)
                If Not String.Equals(ts.ParentEntity, sig.EntityLabel, StringComparison.Ordinal) Then Continue For
                Dim tsId As String = naming.DisplayTimestamp(sig.EntityLabel, ownTimestamps)
                ownTimestamps += 1
                Dim tsTime As DateTimeOffset = Parse(ts.Time, AtTime)
                covering.Add(New Layer With {
                    .Kind = LayerKind.SignatureTimestamp, .DisplayId = tsId, .DocumentOrderIndex = order, .TrustedTime = tsTime,
                    .CertPath = New List(Of CertInfo) From {CertByIndex(Verifier, ts.CertificateIndex, AtTime)},
                    .MissingData = poe.IsMissing(CertByIndexRaw(Verifier, ts.CertificateIndex), tsTime, AtTime, tsId)})
            Next

            ' Every document timestamp later in document order covers this signature.
            For Each dtEntry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
                If dtEntry.Key <= order Then Continue For
                Dim dtTime As DateTimeOffset = SigTime(dtEntry.Value, AtTime)
                Dim dtDisplayId As String = naming.Display(EntityNaming.EntityKey(dtEntry.Value.EntityLabel, dtEntry.Key, True))
                covering.Add(New Layer With {
                    .Kind = LayerKind.DocumentTimestamp, .DisplayId = dtDisplayId, .DocumentOrderIndex = dtEntry.Key, .TrustedTime = dtTime,
                    .CertPath = New List(Of CertInfo) From {SignerCert(Verifier, dtEntry.Value, AtTime)},
                    .MissingData = poe.IsMissing(FindSignerCert(Verifier, dtEntry.Value), dtTime, AtTime, dtDisplayId)})
            Next

            ' An approval signature is validated at its earliest covering genTime (its own claimed time is untrusted), else now.
            Dim sigMoment As DateTimeOffset = SignaturePoeMoment(Verifier, sig, order, docTimestamps, AtTime)
            Dim sigDisplayId As String = naming.Display(sigId)
            signatures.Add(New SignatureInput With {
                .Id = sigId, .DisplayId = sigDisplayId, .DocumentOrderIndex = order,
                .SignerPath = New List(Of CertInfo) From {SignerCert(Verifier, sig, AtTime)},
                .ClaimedSigningTime = TryParseMoment(sig.ClaimedSigningTime),
                .CoveringTimestamps = covering,
                .MissingData = poe.IsMissing(FindSignerCert(Verifier, sig), sigMoment, AtTime, sigDisplayId)})
        Next

        ' A document timestamp is a validity object of its own, covered by the later document timestamps.
        For Each entry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
            Dim order As Integer = entry.Key
            Dim dt As PDFSignature = entry.Value
            Dim covering As New List(Of Layer)
            For Each laterEntry As KeyValuePair(Of Integer, PDFSignature) In docTimestamps
                If laterEntry.Key <= order Then Continue For
                Dim laterTime As DateTimeOffset = SigTime(laterEntry.Value, AtTime)
                Dim laterDisplayId As String = naming.Display(EntityNaming.EntityKey(laterEntry.Value.EntityLabel, laterEntry.Key, True))
                covering.Add(New Layer With {
                    .Kind = LayerKind.DocumentTimestamp, .DisplayId = laterDisplayId, .DocumentOrderIndex = laterEntry.Key, .TrustedTime = laterTime,
                    .CertPath = New List(Of CertInfo) From {SignerCert(Verifier, laterEntry.Value, AtTime)},
                    .MissingData = poe.IsMissing(FindSignerCert(Verifier, laterEntry.Value), laterTime, AtTime, laterDisplayId)})
            Next

            Dim dtOwn As DateTimeOffset = SigTime(dt, AtTime)
            Dim dtId As String = EntityNaming.EntityKey(dt.EntityLabel, order, True)
            Dim dtOwnDisplayId As String = naming.Display(dtId)
            signatures.Add(New SignatureInput With {
                .Id = dtId, .DisplayId = dtOwnDisplayId, .DocumentOrderIndex = order,
                .SignerPath = New List(Of CertInfo) From {SignerCert(Verifier, dt, AtTime)},
                .ClaimedSigningTime = TryParseMoment(dt.ClaimedSigningTime),
                .CoveringTimestamps = covering,
                .MissingData = poe.IsMissing(FindSignerCert(Verifier, dt), dtOwn, AtTime, dtOwnDisplayId)})
        Next

        Return New DocumentValidationInput With {.Aggregation = AggregationPolicy.All, .Signatures = signatures, .Sunset = AlgorithmSunsetTable.Empty}
    End Function

    ''' <summary>Earliest genTime among the timestamps covering an approval signature, or AtTime when none covers it.</summary>
    Private Function SignaturePoeMoment(Verifier As PDFVerifier, Sig As PDFSignature, Order As Integer,
                                        DocTimestamps As List(Of KeyValuePair(Of Integer, PDFSignature)), AtTime As DateTimeOffset) As DateTimeOffset
        Dim min As DateTimeOffset = DateTimeOffset.MaxValue
        Dim covered As Boolean = False
        For j As Integer = 0 To Verifier.Timestamps.Count - 1
            Dim ts As TimestampInfo = Verifier.Timestamps(j)
            If Not String.Equals(ts.ParentEntity, Sig.EntityLabel, StringComparison.Ordinal) Then Continue For
            Dim t As DateTimeOffset = Parse(ts.Time, AtTime)
            If t < min Then min = t
            covered = True
        Next
        For Each dtEntry As KeyValuePair(Of Integer, PDFSignature) In DocTimestamps
            If dtEntry.Key <= Order Then Continue For
            Dim t As DateTimeOffset = SigTime(dtEntry.Value, AtTime)
            If t < min Then min = t
            covered = True
        Next
        Return If(covered, min, AtTime)
    End Function

    Private Function SignerCert(Verifier As PDFVerifier, Sig As PDFSignature, AtTime As DateTimeOffset) As CertInfo
        Dim cert As Certificate = FindSignerCert(Verifier, Sig)
        ' Not embedded: NotAfter becomes AtTime so the result stays a concrete instant.
        If cert Is Nothing Then Return New CertInfo With {.NotBefore = DateTimeOffset.MinValue, .NotAfter = AtTime, .CommonName = "ismeretlen"}
        Return ToCertInfo(cert)
    End Function

    Private Function CertByIndex(Verifier As PDFVerifier, Index As Integer, AtTime As DateTimeOffset) As CertInfo
        Dim cert As Certificate = CertByIndexRaw(Verifier, Index)
        If cert Is Nothing Then Return New CertInfo With {.NotBefore = DateTimeOffset.MinValue, .NotAfter = AtTime, .CommonName = "ismeretlen"}
        Return ToCertInfo(cert)
    End Function

    ''' <summary>The embedded certificate that signed the entity (matched by issuer and serial), or Nothing.</summary>
    Friend Function FindSignerCert(Verifier As PDFVerifier, Sig As PDFSignature) As Certificate
        If String.IsNullOrEmpty(Sig.IssuerRDN) OrElse Sig.SerialNumber Is Nothing Then Return Nothing
        For c As Integer = 0 To Verifier.Certificates.Count - 1
            Dim cert As Certificate = Verifier.Certificates(c)
            If String.Equals(cert.IssuerRDN, Sig.IssuerRDN, StringComparison.OrdinalIgnoreCase) AndAlso BytesEqual(cert.SerialNumber, Sig.SerialNumber) Then Return cert
        Next
        Return Nothing
    End Function

    Private Function CertByIndexRaw(Verifier As PDFVerifier, Index As Integer) As Certificate
        If Index < 0 OrElse Index >= Verifier.Certificates.Count Then Return Nothing
        Return Verifier.Certificates(Index)
    End Function

    Private Function ToCertInfo(Cert As Certificate) As CertInfo
        Return New CertInfo With {.NotBefore = ToMoment(Cert.ValidFrom), .NotAfter = ToMoment(Cert.ValidTo), .CommonName = Cert.SubjectRDN}
    End Function

    Private Function SigTime(Sig As PDFSignature, AtTime As DateTimeOffset) As DateTimeOffset
        Dim s As String = If(String.IsNullOrEmpty(Sig.ValidatedSigningTime), Sig.ClaimedSigningTime, Sig.ValidatedSigningTime)
        Return Parse(s, AtTime)
    End Function

    Private Function Parse(Value As String, Fallback As DateTimeOffset) As DateTimeOffset
        Dim parsed As DateTimeOffset? = TryParseMoment(Value)
        Return If(parsed.HasValue, parsed.Value, Fallback)
    End Function

    Private Function ToMoment(Value As String) As DateTimeOffset
        Dim parsed As DateTimeOffset? = TryParseMoment(Value)
        Return If(parsed.HasValue, parsed.Value, DateTimeOffset.MaxValue)
    End Function

    ''' <summary>SecureBlackbox times are UTC strings; unparsable or empty text gives Nothing.</summary>
    Private Function TryParseMoment(Value As String) As DateTimeOffset?
        If String.IsNullOrEmpty(Value) Then Return Nothing
        Dim dt As Date
        If Date.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, dt) Then
            Return New DateTimeOffset(dt, TimeSpan.Zero)
        End If
        Return Nothing
    End Function

    Private Function BytesEqual(A As Byte(), B As Byte()) As Boolean
        If A Is Nothing OrElse B Is Nothing OrElse A.Length <> B.Length Then Return False
        For i As Integer = 0 To A.Length - 1
            If A(i) <> B(i) Then Return False
        Next
        Return True
    End Function

    ''' <summary>Re-validates a leaf chain offline over the document's embedded material at a given moment; memoised per (leaf, moment).</summary>
    Private NotInheritable Class PoeValidator
        Private ReadOnly _knownCerts As New CertificateList
        Private ReadOnly _trustedAnchors As New CertificateList
        Private ReadOnly _ocspBytes As New List(Of Byte())
        Private ReadOnly _crlBytes As New List(Of Byte())
        Private ReadOnly _cache As New Dictionary(Of String, Boolean)
        Private ReadOnly _processing As Action(Of String)
        Private ReadOnly _trace As Action(Of String)

        ''' <summary>Snapshots the verifier's embedded certificates, OCSP responses and CRLs into standalone copies for later offline re-validation.</summary>
        Friend Sub New(Verifier As PDFVerifier, Processing As Action(Of String), Trace As Action(Of String))
            _processing = Processing
            _trace = Trace
            ' Standalone copies: the verifier-owned objects are only valid while the verifier lives.
            For i As Integer = 0 To Verifier.Certificates.Count - 1
                Dim b As Byte() = Verifier.Certificates(i).Bytes
                If b Is Nothing OrElse b.Length = 0 Then Continue For
                Dim cert As New Certificate(b, 0, b.Length)
                _knownCerts.Add(cert)
                ' Self-signed anchors are trusted explicitly so their own revocation is not looked up offline.
                If cert.SelfSigned Then _trustedAnchors.Add(cert)
            Next
            For i As Integer = 0 To Verifier.OCSPs.Count - 1
                Dim b As Byte() = Verifier.OCSPs(i).Bytes
                If b IsNot Nothing AndAlso b.Length > 0 Then _ocspBytes.Add(b)
            Next
            For i As Integer = 0 To Verifier.CRLs.Count - 1
                Dim b As Byte() = Verifier.CRLs(i).Bytes
                If b IsNot Nothing AndAlso b.Length > 0 Then _crlBytes.Add(b)
            Next
        End Sub

        ''' <summary>True when the leaf's chain lacks a CA certificate / CRL / OCSP the DSS would need at Moment; a Nothing leaf counts as missing.</summary>
        Friend Function IsMissing(Leaf As Certificate, Moment As DateTimeOffset, AtTime As DateTimeOffset, Label As String) As Boolean
            If Leaf Is Nothing Then
                _processing($"  {Label} tanúsítványa nincs beágyazva – hitelesség nem bizonyítható")
                Return True
            End If

            Dim key As String = If(Leaf.SerialNumber Is Nothing, String.Empty, BitConverter.ToString(Leaf.SerialNumber)) & "|" & Leaf.IssuerRDN & "|" & Moment.UtcTicks
            Dim cached As Boolean
            If _cache.TryGetValue(key, cached) Then Return cached

            Dim result As Boolean
            Dim validationLog As String = Nothing
            Dim context As New ChainContextTracker
            Using validator As CertificateValidator = SbbLicense.CreateCertificateValidator()
                Try
                    validator.Certificate = Leaf
                    validator.KnownCertificates = _knownCerts
                    validator.TrustedCertificates = _trustedAnchors
                    For Each o As Byte() In _ocspBytes
                        validator.KnownOCSPs.Add(New OCSPResponse(o, 0, o.Length))
                    Next
                    For Each c As Byte() In _crlBytes
                        validator.KnownCRLs.Add(New CRL(c, 0, c.Length))
                    Next
                    validator.RevocationCheck = CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL
                    validator.OfflineMode = True
                    validator.Config("ImplicitlyTrustSelfSignedCertificates=true")
                    validator.Config("ForceCompleteChainValidationForTrusted=false")
                    validator.Config("RevocationMomentGracePeriod=" & OpenEndedRevocationGraceSeconds)
                    If Moment < AtTime Then validator.ValidationMoment = Moment.UtcDateTime.ToString(SbbMomentFormat, CultureInfo.InvariantCulture)

                    AddHandler validator.OnBeforeCertificateValidation, Sub(s, e) context.Update(e.Cert, e.CACert)
                    AddHandler validator.OnError, Sub(s, e) _processing("  " & SbbErrorTranslator.Describe(e.ErrorCode, e.Description, context.Current, Label))

                    validator.Validate()
                    result = (validator.ChainValidationDetails And MissingDataMask) <> 0
                    Try
                        validationLog = validator.ValidationLog
                    Catch
                        validationLog = Nothing
                    End Try
                Catch ex As Exception
                    ' An unvalidatable chain counts as missing evidence, the conservative outcome.
                    _processing($"  {Label}: hiba a létezési-bizonyíték ellenőrzése közben: {SbbErrorTranslator.ExceptionText(ex)}")
                    _cache(key) = True
                    Return True
                End Try
            End Using

            Dim momentText As String = MomentFormat.Log(Moment)
            _processing($"  {Label} létezési-bizonyíték időpontjában ({momentText}) ellenőrizve: " & If(result, "hiányzó visszavonási/lánc-adat", "a lánc bizonyítható"))
            If _trace IsNot Nothing Then _trace($"{Label} tanúsítványlánc-ellenőrzési napló (létezési bizonyíték: {momentText}):{Environment.NewLine}{validationLog}")

            _cache(key) = result
            Return result
        End Function
    End Class
End Module
