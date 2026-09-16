Imports PDFSigner

''' <summary>Verifies the ETSI end-of-validity calculator against archival/evidence chain, revocation, and aggregation scenarios.</summary>
<TestClass>
Public Class EtsiValidityCalculatorTests
    Private Shared Function Y(year As Integer) As DateTimeOffset
        Return New DateTimeOffset(year, 1, 1, 0, 0, 0, TimeSpan.Zero)
    End Function

    Private Shared Function Cert(notAfter As Integer, Optional notBefore As Integer = 2000, Optional rev As RevocationInfo = Nothing) As CertInfo
        Return New CertInfo With {.NotBefore = Y(notBefore), .NotAfter = Y(notAfter), .Revocation = rev, .CommonName = "CN"}
    End Function

    Private Shared Function Ts(kind As LayerKind, genTime As Integer, tsaNotAfter As Integer, docOrder As Integer,
                               Optional rev As RevocationInfo = Nothing, Optional missingFrom As Integer? = Nothing, Optional missingData As Boolean = False) As Layer
        Return New Layer With {
            .Kind = kind, .DocumentOrderIndex = docOrder, .TrustedTime = Y(genTime),
            .CertPath = New List(Of CertInfo) From {Cert(tsaNotAfter, rev:=rev)},
            .MissingRevocationFrom = If(missingFrom.HasValue, Y(missingFrom.Value), CType(Nothing, DateTimeOffset?)),
            .MissingData = missingData}
    End Function

    Private Shared Function Sig(id As String, signerNotAfter As Integer, docOrder As Integer, signerRev As RevocationInfo,
                                missingData As Boolean, ParamArray covering As Layer()) As SignatureInput
        Return New SignatureInput With {
            .Id = id, .DocumentOrderIndex = docOrder,
            .SignerPath = New List(Of CertInfo) From {Cert(signerNotAfter, rev:=signerRev)},
            .CoveringTimestamps = covering.ToList(), .MissingData = missingData}
    End Function

    Private Shared Function Sig(id As String, signerNotAfter As Integer, docOrder As Integer) As SignatureInput
        Return Sig(id, signerNotAfter, docOrder, Nothing, False)
    End Function

    Private Shared Function Run(atTime As DateTimeOffset, ParamArray sigs As SignatureInput()) As DocumentValidityResult
        Return EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Signatures = sigs.ToList()}, atTime)
    End Function

    Private Shared Function Revoked(time As Integer, Optional reason As String = "affiliationChanged", Optional invalidityDate As Integer? = Nothing) As RevocationInfo
        Return New RevocationInfo With {.Revoked = True, .RevocationTime = Y(time), .Reason = reason,
            .InvalidityDate = If(invalidityDate.HasValue, Y(invalidityDate.Value), CType(Nothing, DateTimeOffset?))}
    End Function

    <TestMethod> Public Sub A_plain_signature()
        Dim r = Run(Y(2020), Sig("S", 2026, 0))
        Assert.AreEqual(Y(2026), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub B_inner_timestamp_does_not_shorten()
        Dim r = Run(Y(2020), Sig("S", 2030, 0, Nothing, False, Ts(LayerKind.SignatureTimestamp, 2024, 2026, 1)))
        Assert.AreEqual(Y(2030), r.Expiry)
    End Sub

    <TestMethod> Public Sub C_timestamp_extends()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2040), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub D_archival_chain()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1), Ts(LayerKind.DocumentTimestamp, 2029, 2045, 2)))
        Assert.AreEqual(Y(2045), r.Expiry)
    End Sub

    <TestMethod> Public Sub E_late_archive_gap()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2028, 2050, 1)))
        Assert.AreEqual(Y(2026), r.Expiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.Status)
        Assert.AreEqual(Y(2028), r.PerSignature(0).GapBefore)
    End Sub

    <TestMethod> Public Sub F_expired_but_covered()
        Dim r = Run(Y(2030), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2027, 1), Ts(LayerKind.DocumentTimestamp, 2026, 2050, 2)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub G_expired_uncovered_gap()
        Dim r = Run(Y(2030), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2027, 1), Ts(LayerKind.DocumentTimestamp, 2028, 2050, 2)))
        Assert.AreEqual(Y(2027), r.Expiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.Status)
    End Sub

    <TestMethod> Public Sub H_revoked_signer_poe_before_revocation()
        Dim r = Run(Y(2030), Sig("S", 2050, 0, Revoked(2025), False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2040), r.Expiry)
        Assert.AreEqual(ValidityStatus.ValidUntil, r.Status)
    End Sub

    <TestMethod> Public Sub I_key_compromise_is_invalid_from_creation()
        Dim r = Run(Y(2030), Sig("S", 2050, 0, Revoked(2026, "keyCompromise", 2023), False, Ts(LayerKind.SignatureTimestamp, 2024, 2040, 1)))
        Assert.AreEqual(Y(2023), r.Expiry)
        Assert.AreEqual(ValidityStatus.Invalid, r.Status)
        Assert.AreNotEqual(DateTimeOffset.MinValue, r.Expiry)
    End Sub

    <TestMethod> Public Sub J_two_signatures_all_and_any()
        Dim a = Sig("A", 2045, 0)
        Dim b = Sig("B", 2031, 1)
        Assert.AreEqual(Y(2031), EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Aggregation = AggregationPolicy.All, .Signatures = New List(Of SignatureInput) From {a, b}}, Y(2020)).Expiry)
        Assert.AreEqual(Y(2045), EtsiValidityCalculator.Compute(New DocumentValidationInput With {.Aggregation = AggregationPolicy.Any, .Signatures = New List(Of SignatureInput) From {a, b}}, Y(2020)).Expiry)
    End Sub

    <TestMethod> Public Sub K_missing_revocation_data()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2050, 1, missingFrom:=2027)))
        Assert.AreEqual(Y(2027), r.Expiry)
        Assert.AreEqual(ValidityStatus.MissingRevocationData, r.Status)
    End Sub

    <TestMethod> Public Sub Suspect_timeline_when_a_later_timestamp_has_an_earlier_gentime()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False, Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1), Ts(LayerKind.DocumentTimestamp, 2024, 2031, 2)))
        Assert.AreEqual(ValidityStatus.SuspectTimeline, r.Status)
    End Sub

    <TestMethod> Public Sub Missing_signer_caps_evidence_while_archival_extends()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, True, Ts(LayerKind.DocumentTimestamp, 2025, 2045, 1)))
        Assert.AreEqual(Y(2045), r.Expiry)
        Assert.AreEqual(Y(2026), r.EvidenceExpiry)
        Assert.AreEqual(ValidityStatus.MissingRevocationData, r.PerSignature(0).Status)
    End Sub

    <TestMethod> Public Sub Missing_mid_chain_timestamp_freezes_evidence()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False,
            Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1),
            Ts(LayerKind.DocumentTimestamp, 2028, 2035, 2, missingData:=True),
            Ts(LayerKind.DocumentTimestamp, 2032, 2050, 3)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(Y(2035), r.EvidenceExpiry)
    End Sub

    <TestMethod> Public Sub Severe_status_survives_the_evidence_divergence()
        Dim r = Run(Y(2020), Sig("S", 2026, 0, Nothing, False,
            Ts(LayerKind.DocumentTimestamp, 2025, 2030, 1),
            Ts(LayerKind.DocumentTimestamp, 2028, 2035, 2, missingData:=True),
            Ts(LayerKind.DocumentTimestamp, 2032, 2050, 3),
            Ts(LayerKind.DocumentTimestamp, 2060, 2070, 4)))
        Assert.AreEqual(Y(2050), r.Expiry)
        Assert.AreEqual(Y(2035), r.EvidenceExpiry)
        Assert.AreEqual(ValidityStatus.BrokenChain, r.PerSignature(0).Status)
    End Sub

    <TestMethod> Public Sub No_signatures_expire_now()
        Dim r = Run(Y(2020))
        Assert.AreEqual(Y(2020), r.Expiry)
        Assert.AreEqual(ValidityStatus.Expired, r.Status)
    End Sub

    <TestMethod> Public Sub Moment_helpers_render_sentinels_and_utc()
        Assert.AreEqual("+∞", EtsiValidityCalculator.TraceMoment(DateTimeOffset.MaxValue))
        Assert.AreEqual("-∞", EtsiValidityCalculator.OutcomeMoment(DateTimeOffset.MinValue))
        Assert.AreEqual("2026.09.08. 17:18:20 UTC", EtsiValidityCalculator.TraceMoment(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero)))
        StringAssert.EndsWith(EtsiValidityCalculator.OutcomeMoment(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero)), " UTC)")
    End Sub
End Class
