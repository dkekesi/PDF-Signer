Imports System.IO
Imports System.Text
Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports PDFSignerCommon

''' <summary>Verifies SbbSignatureCreator's offline failure paths and the signature-policy embedding, without network access.</summary>
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
    Public Sub Signature_policy_check_requires_an_oid_and_valid_hash_only_when_a_url_is_set()
        Dim url As New Uri("http://policy.test/pol.pdf")
        Dim hex As String = "unset"

        Assert.IsNull(SbbSignatureCreator.CheckSignaturePolicy(Nothing, hex))
        Assert.IsNull(hex, "no metadata: no policy")
        hex = "unset"
        Assert.IsNull(SbbSignatureCreator.CheckSignaturePolicy(New MetaData With {.SignaturePolicyOID = "1.2.3"}, hex))
        Assert.IsNull(hex, "no URL: no policy")

        For Each blank In {Nothing, "", "  "}
            hex = "unset"
            Assert.AreEqual("Az aláírás-szabályzat azonosítója (OID) nincs megadva, pedig a szabályzat URL be van állítva!",
                            SbbSignatureCreator.CheckSignaturePolicy(New MetaData With {.SignaturePolicyURL = url, .SignaturePolicyOID = blank, .SignaturePolicyHash = "AAr/EA=="}, hex))
            Assert.IsNull(hex)
        Next

        Assert.AreEqual("Az aláírás-szabályzat lenyomata nem érvényes base64 érték!",
                        SbbSignatureCreator.CheckSignaturePolicy(New MetaData With {.SignaturePolicyURL = url, .SignaturePolicyOID = "1.2.3", .SignaturePolicyHash = "not base64!"}, hex))

        Assert.IsNull(SbbSignatureCreator.CheckSignaturePolicy(New MetaData With {.SignaturePolicyURL = url, .SignaturePolicyOID = "1.2.3", .SignaturePolicyHash = "AAr/EA=="}, hex))
        Assert.AreEqual("000AFF10", hex)
        Assert.IsNull(SbbSignatureCreator.CheckSignaturePolicy(New MetaData With {.SignaturePolicyURL = url, .SignaturePolicyOID = "1.2.3"}, hex))
        Assert.AreEqual(String.Empty, hex, "URL and OID without a hash: policy with an empty hash")
    End Sub

    <TestMethod>
    Public Sub Applied_signature_policy_is_embedded_in_the_signature()
        Dim hash = New Byte(31) {}
        For i = 0 To hash.Length - 1 : hash(i) = CByte(i * 7 + 3) : Next
        Dim policy As New MetaData With {
            .SignaturePolicyOID = "1.2.3.4.5",
            .SignaturePolicyHash = Convert.ToBase64String(hash),
            .SignaturePolicyURL = New Uri("http://policy.test/pol.pdf")}

        Dim cms As Byte() = SignOffline(Sub(s) SbbSignatureCreator.ApplySignaturePolicy(s.NewSignature, policy, SbbTranslator.PolicyHashHex(policy.SignaturePolicyHash), HashType.SHA256))

        Assert.IsTrue(Contains(cms, SigPolicyIdAttribute), "sigPolicyId attribute")
        Assert.IsTrue(Contains(cms, New Byte() {&H6, &H4, &H2A, &H3, &H4, &H5}), "policy OID")
        Assert.IsTrue(Contains(cms, hash), "policy hash")
        Assert.IsTrue(Contains(cms, Encoding.ASCII.GetBytes("http://policy.test/pol.pdf")), "policy URI")
        Assert.IsFalse(Contains(SignOffline(Nothing), SigPolicyIdAttribute), "no policy without ApplySignaturePolicy")
    End Sub

    ''' <summary>DER of id-aa-ets-sigPolicyId (1.2.840.113549.1.9.16.2.15).</summary>
    Private Shared ReadOnly SigPolicyIdAttribute As Byte() = {&H6, &HB, &H2A, &H86, &H48, &H86, &HF7, &HD, &H1, &H9, &H10, &H2, &HF}

    ''' <summary>Offline B-B signature of the minimal PDF with a self-signed certificate; returns the signature's CMS.</summary>
    Private Shared Function SignOffline(configure As Action(Of nsoftware.SecureBlackbox.PDFSigner)) As Byte()
        Using cert = TestCertificates.SelfSigned("CN=Policy Signer", signing:=True, notAfter:=Date.Now.AddYears(1))
            Using manager = SbbLicense.CreateCertificateManager(), output As New MemoryStream
                manager.ImportFromObject(cert)
                Using signer = SbbLicense.CreateSigner()
                    signer.InputStream = New MemoryStream(TestPdf.Minimal())
                    signer.OutputStream = output
                    signer.SigningCertificate = manager.Certificate
                    signer.NewSignature.Level = PAdESSignatureLevels.paslBaselineB
                    signer.NewSignature.HashAlgorithm = "SHA256"
                    signer.Widget.Invisible = True
                    signer.RevocationCheck = PDFSignerRevocationChecks.crcNone
                    signer.OfflineMode = True
                    signer.IgnoreChainValidationErrors = True
                    configure?.Invoke(signer)
                    signer.Sign()
                End Using
                Using verifier = SbbLicense.CreateVerifier()
                    verifier.InputStream = New MemoryStream(output.ToArray())
                    verifier.OfflineMode = True
                    verifier.AutoValidateSignatures = False
                    verifier.Verify()
                    Return verifier.Signatures(0).SignatureBytes
                End Using
            End Using
        End Using
    End Function

    Private Shared Function Contains(haystack As Byte(), needle As Byte()) As Boolean
        For i = 0 To haystack.Length - needle.Length
            Dim match = True
            For j = 0 To needle.Length - 1
                If haystack(i + j) <> needle(j) Then match = False : Exit For
            Next
            If match Then Return True
        Next
        Return False
    End Function

    <TestMethod>
    Public Sub Unsupported_level_is_reported_without_throwing()
        Dim res = Sign(0, Nothing)
        StringAssert.StartsWith(res.ErrorMessage, "Váratlan hiba az aláírás közben:")
        StringAssert.Contains(res.ErrorMessage, "Nem támogatott PAdES szint")
        Assert.IsFalse(String.IsNullOrEmpty(res.ExceptionText))
        Assert.IsNull(res.SignedFile)
    End Sub
End Class
