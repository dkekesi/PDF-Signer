Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports PDFSignerCommon

''' <summary>Verifies SigningPlanBuilder's mapping from provider settings to a SigningPlan for the four PAdES baseline levels.</summary>
<TestClass>
Public Class SigningPlanBuilderTests
    Private Shared Function Cfg(level As PAdESLevelType, Optional checking As Boolean = False, Optional protocol As RevocationType = RevocationType.OCSP) As PDFSignerCryptoProvider
        Return New PDFSignerCryptoProvider With {.PAdESLevel = level, .EnableRevocationChecking = checking, .RevocationCheckProtocol = protocol, .EmbedRevocationInformation = True}
    End Function

    <TestMethod>
    Public Sub BaselineB_is_signature_only()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineB, p.SignatureLevel)
        Assert.IsFalse(p.EmbedSignatureTimestamp)
        Assert.IsFalse(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
        Assert.IsFalse(p.EmbedDocumentTimestampRevocation)
    End Sub

    <TestMethod>
    Public Sub BaselineT_signs_with_a_signature_timestamp_and_no_update()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.EmbedSignatureTimestamp)
        Assert.IsFalse(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
    End Sub

    <TestMethod>
    Public Sub BaselineLT_is_two_phase_without_document_timestamp()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLT, checking:=True))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.EmbedSignatureTimestamp)
        Assert.IsTrue(p.UpdateToEmbed)
        Assert.IsFalse(p.AddDocumentTimestamp)
        Assert.IsFalse(p.EmbedDocumentTimestampRevocation)
    End Sub

    <TestMethod>
    Public Sub BaselineLTA_adds_a_lean_document_timestamp_and_its_own_update()
        Dim p = SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=True))
        Assert.AreEqual(PAdESSignatureLevels.paslBaselineT, p.SignatureLevel)
        Assert.IsTrue(p.UpdateToEmbed)
        Assert.IsTrue(p.AddDocumentTimestamp)
        Assert.IsTrue(p.EmbedDocumentTimestampRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=False)).EmbedDocumentTimestampRevocation)
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=False)).AddDocumentTimestamp)
    End Sub

    <TestMethod>
    Public Sub Checking_off_forces_crcNone_and_checking_on_maps_the_protocol()
        Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=False)).Revocation)
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyCRL, SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=True, protocol:=RevocationType.CRL)).Revocation)
    End Sub

    <TestMethod>
    Public Sub Non_deferred_levels_check_during_sign_and_deferred_levels_sign_lean()
        For Each level In {PAdESLevelType.BaselineB, PAdESLevelType.BaselineT}
            Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, SigningPlanBuilder.Build(Cfg(level, checking:=True)).SignPassRevocationCheck, level.ToString())
            Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SigningPlanBuilder.Build(Cfg(level, checking:=False)).SignPassRevocationCheck, level.ToString())
        Next
        For Each level In {PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Dim p = SigningPlanBuilder.Build(Cfg(level, checking:=True))
            Assert.AreEqual(PDFSignerRevocationChecks.crcNone, p.SignPassRevocationCheck, level.ToString())
            Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, p.Revocation, level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Explicit_validator_checks_belong_to_the_non_embedding_levels_only()
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=True)).CheckSigningCertRevocation)
        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=True)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineB, checking:=False)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLT, checking:=True)).CheckSigningCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineLTA, checking:=True)).CheckSigningCertRevocation)

        Assert.IsTrue(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=True)).CheckTimestampCertRevocation)
        Assert.IsFalse(SigningPlanBuilder.Build(Cfg(PAdESLevelType.BaselineT, checking:=False)).CheckTimestampCertRevocation)
        For Each level In {PAdESLevelType.BaselineB, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.IsFalse(SigningPlanBuilder.Build(Cfg(level, checking:=True)).CheckTimestampCertRevocation, level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Undefined_level_is_rejected()
        Assert.ThrowsException(Of NotSupportedException)(Sub() SigningPlanBuilder.Build(New PDFSignerCryptoProvider With {.PAdESLevel = 0}))
    End Sub
End Class
