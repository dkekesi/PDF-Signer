Imports System.Xml.Linq
Imports PDFSignerCommon
Imports PDFSignerCommon.My.Resources

''' <summary>Verifies validation, defaults, and XML persistence of the built-in PDF Signer provider settings.</summary>
<TestClass>
Public Class PDFSignerCryptoProviderTests
    Private Shared Function Enabled(level As PAdESLevelType, Optional checking As Boolean = False, Optional embed As Boolean = True,
                                    Optional protocol As RevocationType = RevocationType.OCSP, Optional tsa As String = "https://tsa.example/ts") As PDFSignerCryptoProvider
        Return New PDFSignerCryptoProvider With {
            .Enabled = True,
            .PAdESLevel = level,
            .EnableRevocationChecking = checking,
            .EmbedRevocationInformation = embed,
            .RevocationCheckProtocol = protocol,
            .TSAURL = If(tsa Is Nothing, Nothing, New Uri(tsa))}
    End Function

    <TestMethod>
    Public Sub Defaults_are_BaselineB_with_OCSP_protocol_and_embedding_on()
        Dim p As New PDFSignerCryptoProvider
        Assert.AreEqual(CInt(PAdESLevelType.BaselineB), p.PAdESLevel)
        Assert.IsFalse(p.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), p.RevocationCheckProtocol)
        Assert.IsTrue(p.EmbedRevocationInformation)
        Assert.AreEqual(CInt(HashType.SHA256), p.SignatureHashMethod)
    End Sub

    <TestMethod>
    Public Sub Disabled_provider_is_valid_whatever_it_holds()
        Dim p As New PDFSignerCryptoProvider With {.Enabled = False, .PAdESLevel = 0}
        Assert.AreEqual(String.Empty, p.Validate())
    End Sub

    <TestMethod>
    Public Sub Undefined_level_is_rejected()
        Dim p = Enabled(PAdESLevelType.BaselineB)
        p.PAdESLevel = 1
        Assert.AreEqual(Messages.PAdES_Level_Missing, p.Validate())
    End Sub

    <TestMethod>
    Public Sub BaselineB_needs_no_tsa()
        Assert.AreEqual(String.Empty, Enabled(PAdESLevelType.BaselineB, tsa:=Nothing).Validate())
    End Sub

    <TestMethod>
    Public Sub Timestamped_levels_need_a_tsa_url()
        For Each level In {PAdESLevelType.BaselineT, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.AreEqual(Messages.TSA_URL_Missing, Enabled(level, checking:=True, tsa:=Nothing).Validate(), level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub Checking_on_needs_a_protocol()
        Assert.AreEqual(Messages.Revocation_Method_Missing, Enabled(PAdESLevelType.BaselineB, checking:=True, protocol:=RevocationType.None).Validate())
    End Sub

    <TestMethod>
    Public Sub Long_term_levels_need_checking_and_embedding()
        For Each level In {PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA}
            Assert.AreEqual(Messages.LTV_Requires_Revocation_Embedding, Enabled(level, checking:=False, embed:=True).Validate(), level.ToString())
            Assert.AreEqual(Messages.LTV_Requires_Revocation_Embedding, Enabled(level, checking:=True, embed:=False).Validate(), level.ToString())
            Assert.AreEqual(String.Empty, Enabled(level, checking:=True, embed:=True).Validate(), level.ToString())
        Next
    End Sub

    <TestMethod>
    Public Sub BaselineLTA_needs_a_timestamp_hash()
        Dim p = Enabled(PAdESLevelType.BaselineLTA, checking:=True)
        p.TimeStampHashMethod = 0
        Assert.AreEqual(Messages.Time_Stamp_Hash_Algorithm_Missing, p.Validate())
    End Sub

    <TestMethod>
    Public Sub Xml_round_trip_keeps_the_new_fields()
        Dim p = Enabled(PAdESLevelType.BaselineLT, checking:=True, protocol:=RevocationType.CRL)
        p.EmbedRevocationInformation = True
        Dim restored As New PDFSignerCryptoProvider
        restored.SetupDataFromXml(p.SetupDataToXml(EncryptSecrets:=True))
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLT), restored.PAdESLevel)
        Assert.IsTrue(restored.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.CRL), restored.RevocationCheckProtocol)
        Assert.IsTrue(restored.EmbedRevocationInformation)
        Assert.IsNull(p.SetupDataToXml(EncryptSecrets:=False).Element("IsTimeStampingEnabled"))
    End Sub

    <TestMethod>
    Public Sub Xml_without_a_level_is_derived_from_the_legacy_elements()
        Dim legacy As XElement = <PDFSignerCryptoProvider>
                                     <Enabled>1</Enabled>
                                     <SigningOrganization>Org</SigningOrganization>
                                     <SigningReason>Reason</SigningReason>
                                     <RevocationCheck>2</RevocationCheck>
                                     <SignatureHashMethod>28932</SignatureHashMethod>
                                     <AllowQualifiedCertificatesOnly>0</AllowQualifiedCertificatesOnly>
                                     <SignerNameFromLoggedOnUser>0</SignerNameFromLoggedOnUser>
                                     <IsTimeStampingEnabled>1</IsTimeStampingEnabled>
                                     <TSAURL>https://tsa.example/ts</TSAURL>
                                     <TSAUserName></TSAUserName>
                                     <TSAPassword></TSAPassword>
                                     <IsDocumentTimeStamp>1</IsDocumentTimeStamp>
                                     <IsSinglePassPadesBLTA>1</IsSinglePassPadesBLTA>
                                     <TimeStampHashMethod>28932</TimeStampHashMethod>
                                     <IsProxyEnabled>0</IsProxyEnabled>
                                     <ProxyServer></ProxyServer>
                                     <ProxyPort>8080</ProxyPort>
                                     <ProxyAuthMethod>0</ProxyAuthMethod>
                                     <ProxyUserName></ProxyUserName>
                                     <ProxyPassword></ProxyPassword>
                                 </PDFSignerCryptoProvider>
        Dim p As New PDFSignerCryptoProvider
        p.SetupDataFromXml(legacy)
        Assert.AreEqual(CInt(PAdESLevelType.BaselineLTA), p.PAdESLevel)
        Assert.IsTrue(p.EnableRevocationChecking)
        Assert.AreEqual(CInt(RevocationType.OCSP), p.RevocationCheckProtocol)
        Assert.IsTrue(p.EmbedRevocationInformation)
        Assert.AreEqual("Org", p.SigningOrganization)
    End Sub
End Class
