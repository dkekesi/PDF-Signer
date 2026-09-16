Imports PDFSignerCommon

''' <summary>Verifies the PAdES level and revocation switches derived from the legacy timestamp switches.</summary>
<TestClass>
Public Class LegacyLevelDerivationTests
    <TestMethod>
    Public Sub No_timestamping_is_BaselineB()
        Dim d = LegacyLevelDerivation.FromLegacy(False, False, RevocationType.CRL)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineB), CInt(d.Level))
    End Sub

    <TestMethod>
    Public Sub Timestamping_without_document_timestamp_and_without_revocation_is_BaselineT()
        Dim d = LegacyLevelDerivation.FromLegacy(True, False, RevocationType.None)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineT), CInt(d.Level))
        Assert.IsFalse(d.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), d.RevocationCheckProtocol)
    End Sub

    <TestMethod>
    Public Sub Timestamping_without_document_timestamp_with_revocation_is_BaselineLT()
        Dim d = LegacyLevelDerivation.FromLegacy(True, False, RevocationType.OCSPWithCRLFallback)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLT), CInt(d.Level))
        Assert.IsTrue(d.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSPWithCRLFallback), d.RevocationCheckProtocol)
    End Sub

    <TestMethod>
    Public Sub Document_timestamp_is_BaselineLTA_regardless_of_revocation()
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), CInt(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.None).Level))
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), CInt(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.OCSP).Level))
    End Sub

    <TestMethod>
    Public Sub Embedding_is_always_on()
        Assert.IsTrue(LegacyLevelDerivation.FromLegacy(False, False, RevocationType.None).EmbedRevocationInformation)
        Assert.IsTrue(LegacyLevelDerivation.FromLegacy(True, True, RevocationType.CRL).EmbedRevocationInformation)
    End Sub
End Class
