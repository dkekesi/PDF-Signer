Imports System.IO
Imports PDFSigner
Imports PDFSignerCommon

''' <summary>Verifies SbbSignatureCreator's offline failure paths, which end before any store load, network access or signing pass.</summary>
<TestClass>
Public Class SbbSignatureCreatorTests
    Private Shared Function Sign(level As Integer, certificate As SigningCertificate) As SignatureResult
        Dim creator As New SbbSignatureCreator
        creator.Initialize(New PDFSignerCryptoProvider With {.PAdESLevel = level})
        Using input As New MemoryStream(TestPdf.Minimal())
            Return creator.SignDocument(New SignatureRequest With {.FileToSign = input, .SigningCertificate = certificate})
        End Using
    End Function

    <TestMethod>
    Public Sub Missing_certificate_fails_with_the_selection_message()
        Dim res = Sign(PAdESLevelType.BaselineB, Nothing)
        Assert.AreEqual("Nincs kiválasztva aláíró tanúsítvány!", res.ErrorMessage)
        Assert.IsNull(res.SignedFile)
        StringAssert.StartsWith(res.SignatureLog, "Aláíró tanúsítvány off-line ellenőrzése")
        StringAssert.Contains(res.SignatureLog, res.ErrorMessage)
    End Sub

    <TestMethod>
    Public Sub Untrusted_certificate_fails_the_offline_chain_precheck()
        Using cert = TestCertificates.SelfSigned("CN=Untrusted Signer", signing:=True, notAfter:=Date.Now.AddYears(1))
            Dim res = Sign(PAdESLevelType.BaselineB, New SigningCertificate With {.Certificate = cert})
            StringAssert.Contains(res.ErrorMessage, "nem építhető fel megbízható gyökértanúsítványig")
            Assert.IsNull(res.SignedFile)
        End Using
    End Sub

    <TestMethod>
    Public Sub Unsupported_level_is_reported_without_throwing()
        Dim res = Sign(0, Nothing)
        StringAssert.StartsWith(res.ErrorMessage, "Váratlan hiba az aláírás közben:")
        StringAssert.Contains(res.ErrorMessage, "Nem támogatott PAdES szint")
        Assert.IsFalse(String.IsNullOrEmpty(res.ExceptionText))
        Assert.IsNull(res.SignedFile)
    End Sub
End Class
