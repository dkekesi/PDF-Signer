Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports PDFSignerCommon

''' <summary>Verifies SbbTranslator's mappings from provider settings to SecureBlackbox names/enums and from SecureBlackbox enums to Hungarian text.</summary>
<TestClass>
Public Class SbbTranslatorTests
    <TestMethod>
    Public Sub Hash_ids_map_to_sbb_names()
        Assert.AreEqual("SHA1", SbbTranslator.HashAlgorithmName(HashType.SHA1))
        Assert.AreEqual("SHA224", SbbTranslator.HashAlgorithmName(HashType.SHA224))
        Assert.AreEqual("SHA256", SbbTranslator.HashAlgorithmName(HashType.SHA256))
        Assert.AreEqual("SHA384", SbbTranslator.HashAlgorithmName(HashType.SHA384))
        Assert.AreEqual("SHA512", SbbTranslator.HashAlgorithmName(HashType.SHA512))
    End Sub

    <TestMethod>
    Public Sub Policy_hash_base64_becomes_uppercase_hex_blank_stays_empty_and_invalid_is_nothing()
        Assert.AreEqual("000AFF10", SbbTranslator.PolicyHashHex(Convert.ToBase64String(New Byte() {&H0, &HA, &HFF, &H10})))
        Assert.AreEqual("000AFF10", SbbTranslator.PolicyHashHex(" AAr/EA== "))
        Assert.AreEqual(String.Empty, SbbTranslator.PolicyHashHex(Nothing))
        Assert.AreEqual(String.Empty, SbbTranslator.PolicyHashHex("  "))
        Assert.IsNull(SbbTranslator.PolicyHashHex("not base64!"))
    End Sub

    <TestMethod>
    Public Sub Unknown_hash_is_rejected()
        Assert.ThrowsException(Of NotSupportedException)(Sub() SbbTranslator.HashAlgorithmName(0))
    End Sub

    <TestMethod>
    Public Sub Revocation_types_map_to_both_component_enums()
        Assert.AreEqual(PDFSignerRevocationChecks.crcNone, SbbTranslator.RevocationCheck(RevocationType.None))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyCRL, SbbTranslator.RevocationCheck(RevocationType.CRL))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSP, SbbTranslator.RevocationCheck(RevocationType.OCSP))
        Assert.AreEqual(PDFSignerRevocationChecks.crcAnyOCSPOrCRL, SbbTranslator.RevocationCheck(RevocationType.OCSPWithCRLFallback))
        Assert.AreEqual(CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL, SbbTranslator.RevocationCheckForValidator(RevocationType.OCSPWithCRLFallback))
        Assert.AreEqual(CertificateValidatorRevocationChecks.crcAnyCRL, SbbTranslator.RevocationCheckForValidator(RevocationType.CRL))
    End Sub

    <TestMethod>
    Public Sub Proxy_methods_map_to_sbb_auth_types()
        Assert.AreEqual(ProxyAuthTypes.patNoAuthentication, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.NoAuthentication))
        Assert.AreEqual(ProxyAuthTypes.patBasic, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.UserPassword))
        Assert.AreEqual(ProxyAuthTypes.patDigest, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.Digest))
        Assert.AreEqual(ProxyAuthTypes.patNTLM, SbbTranslator.ProxyAuth(ProxyAuthenticationMethod.NTLM))
        Assert.AreEqual("NTLM", SbbTranslator.ProxyAuthName(ProxyAuthenticationMethod.NTLM))
    End Sub

    <TestMethod>
    Public Sub Chain_kinds_and_event_kinds_are_hungarian()
        Assert.AreEqual("CRL", SbbTranslator.ChainKind(2))
        Assert.AreEqual("OCSP válasz", SbbTranslator.ChainKind(3))
        Assert.AreEqual("CRL letöltve", SbbTranslator.EventKind("CRLRetrieved"))
        Assert.AreEqual("SomethingNew", SbbTranslator.EventKind("SomethingNew"))
    End Sub

    <TestMethod>
    Public Sub Validities_are_hungarian()
        Assert.AreEqual("OK", SbbTranslator.ChainValidity(ChainValidities.cvtValid))
        Assert.AreEqual("a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig", SbbTranslator.ChainValidity(ChainValidities.cvtCantBeEstablished))
        Assert.AreEqual("a tanúsítvány érvénytelen", SbbTranslator.ValidatorChainValidity(CertificateValidatorChainValidationResults.cvtInvalid))
        Assert.AreEqual("az aláírás érvényes", SbbTranslator.SignatureValidity(SignatureValidities.svtValid))
    End Sub
End Class
