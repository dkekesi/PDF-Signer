Imports System.IO
Imports nsoftware.SecureBlackbox
Imports PDFSigner

''' <summary>Verifies the licensed component factory produces components accepted by SecureBlackbox.</summary>
<TestClass>
Public Class SbbLicenseTests
    <TestMethod>
    Public Sub Runtime_license_is_accepted_by_the_verifier()
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            Using doc As New MemoryStream(TestPdf.Minimal())
                verifier.InputStream = doc
                verifier.AutoValidateSignatures = False
                verifier.Open(False)
                Assert.AreEqual(0, verifier.Signatures.Count)
                verifier.Close(False)
            End Using
        End Using
    End Sub

    <TestMethod>
    Public Sub Every_factory_returns_a_licensed_component()
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateSigner().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateVerifier().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateCertificateManager().RuntimeLicense)
        Assert.AreEqual(SbbLicense.Key, SbbLicense.CreateCertificateValidator().RuntimeLicense)
    End Sub
End Class
