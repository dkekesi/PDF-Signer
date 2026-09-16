Imports PDFSigner

''' <summary>Verifies the offline signing-certificate pre-checks and their Hungarian operator messages.</summary>
<TestClass>
Public Class SigningCertificatePrecheckTests
    ' AuthorityInfoAccess with one OCSP access description (1.3.6.1.5.5.7.48.1, URI http://ocsp.test).
    Private Shared ReadOnly AiaWithOcsp As Byte() = {
        &H30, &H1E, &H30, &H1C, &H6, &H8, &H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H1,
        &H86, &H10, &H68, &H74, &H74, &H70, &H3A, &H2F, &H2F, &H6F, &H63, &H73, &H70, &H2E, &H74, &H65, &H73, &H74}

    ' Same shape with caIssuers (…48.2) only.
    Private Shared ReadOnly AiaWithCaIssuersOnly As Byte() = {
        &H30, &H1E, &H30, &H1C, &H6, &H8, &H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H2,
        &H86, &H10, &H68, &H74, &H74, &H70, &H3A, &H2F, &H2F, &H6F, &H63, &H73, &H70, &H2E, &H74, &H65, &H73, &H74}

    <TestMethod>
    Public Sub Aia_with_ocsp_is_detected()
        Assert.IsTrue(SigningCertificatePrecheck.AiaContainsOcsp(AiaWithOcsp))
        Assert.IsFalse(SigningCertificatePrecheck.AiaContainsOcsp(AiaWithCaIssuersOnly))
        Assert.IsFalse(SigningCertificatePrecheck.AiaContainsOcsp(New Byte() {}))
    End Sub

    <TestMethod>
    Public Sub Key_usage_must_allow_signing()
        Using signing = TestCertificates.SelfSigned("CN=Signer", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsTrue(SigningCertificatePrecheck.HasSigningKeyUsage(signing))
        End Using
        Using other = TestCertificates.SelfSigned("CN=Encipher", signing:=False, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(SigningCertificatePrecheck.HasSigningKeyUsage(other))
            StringAssert.Contains(SigningCertificatePrecheck.Check(other, False), "Digital Signature")
        End Using
    End Sub

    <TestMethod>
    Public Sub Expired_certificate_is_reported()
        Using expired = TestCertificates.SelfSigned("CN=Old", signing:=True, notAfter:=Date.Now.AddDays(-1), notBefore:=Date.Now.AddDays(-10))
            StringAssert.Contains(SigningCertificatePrecheck.Check(expired, False), "már nem érvényes")
        End Using
    End Sub

    <TestMethod>
    Public Sub Untrusted_self_signed_certificate_fails_the_chain_check()
        Using cert = TestCertificates.SelfSigned("CN=Island", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(SigningCertificatePrecheck.ChainIsComplete(cert))
            StringAssert.Contains(SigningCertificatePrecheck.Check(cert, False), "tanúsítványlánc")
            Assert.AreEqual("Island", SigningCertificatePrecheck.CommonName(cert))
        End Using
    End Sub
End Class
