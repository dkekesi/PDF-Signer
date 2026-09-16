Imports PDFSignerCommon.My.Resources

''' <summary>Settings of the built-in SecureBlackbox signing provider ("PDF Signer") for one document class.</summary>
Public Class PDFSignerCryptoProvider
    Inherits CryptoProviderBase

    Public Overrides Function ToString() As String
        Return "PDF Signer"
    End Function

    Public Overrides ReadOnly Property ProviderType As CryptoProviderType = CryptoProviderType.PDFSigner
    Public Overrides ReadOnly Property ConfigXmlElementName As String = "PDFSignerCryptoProvider"
    Public Overrides ReadOnly Property SupportsLocalCertificates As Boolean = True

    Public Property SigningOrganization As String
    Public Property SigningReason As String
    ''' <summary>PAdES baseline level to produce (a <see cref="PAdESLevelType"/> value).</summary>
    Public Property PAdESLevel As Integer
    Public Property SignatureHashMethod As Integer

    Public Property AllowQualifiedCertificatesOnly As Boolean
    Public Property SignerNameFromLoggedOnUser As Boolean
    Public Property TSAURL As Uri
    Public Property TSAUserName As String
    Public Property TSAPassword As String
    ''' <summary>Hash of the B-LTA archive document timestamp.</summary>
    Public Property TimeStampHashMethod As Integer

    ''' <summary>Whether the chains of the new signature and its timestamps are revocation-checked.</summary>
    Public Property EnableRevocationChecking As Boolean
    ''' <summary>Protocol of the revocation check (a <see cref="RevocationType"/> value); stored under the CSS/XML name RevocationCheck.</summary>
    Public Property RevocationCheckProtocol As Integer
    ''' <summary>Whether the revocation data is written into the document (B-LT / B-LTA).</summary>
    Public Property EmbedRevocationInformation As Boolean

    Public Property IsProxyEnabled As Boolean
    Public Property ProxyServer As String
    Public Property ProxyPort As Integer
    Public Property ProxyAuthMethod As Integer
    Public Property ProxyUserName As String
    Public Property ProxyPassword As String

    Public Sub New()
        PAdESLevel = PAdESLevelType.BaselineB
        SignatureHashMethod = HashType.SHA256
        TimeStampHashMethod = HashType.SHA256
        EnableRevocationChecking = False
        RevocationCheckProtocol = RevocationType.OCSP
        EmbedRevocationInformation = True
        ProxyPort = 8080
        ProxyAuthMethod = ProxyAuthenticationMethod.NoAuthentication
    End Sub

    ''' <summary>True when the value is one of the four offered baseline levels.</summary>
    Public Shared Function IsDefinedLevel(Level As Integer) As Boolean
        Return Level >= PAdESLevelType.BaselineB AndAlso Level <= PAdESLevelType.BaselineLTA
    End Function

    Private ReadOnly Property IsTimestamped As Boolean
        Get
            Return PAdESLevel = PAdESLevelType.BaselineT OrElse IsLongTerm
        End Get
    End Property

    Private ReadOnly Property IsLongTerm As Boolean
        Get
            Return PAdESLevel = PAdESLevelType.BaselineLT OrElse PAdESLevel = PAdESLevelType.BaselineLTA
        End Get
    End Property

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(SigningOrganization) Then SigningOrganization = SigningOrganization.Trim
        If Not String.IsNullOrEmpty(SigningReason) Then SigningReason = SigningReason.Trim
        If Not String.IsNullOrEmpty(TSAUserName) Then TSAUserName = TSAUserName.Trim
        If Not String.IsNullOrEmpty(ProxyServer) Then ProxyServer = ProxyServer.Trim
        If Not String.IsNullOrEmpty(ProxyUserName) Then ProxyUserName = ProxyUserName.Trim

        If Not Enabled Then Return String.Empty

        If SignatureHashMethod = HashType.None Then Return Messages.Signature_Hash_Algorithm_Missing
        If Not IsDefinedLevel(PAdESLevel) Then Return Messages.PAdES_Level_Missing

        If IsTimestamped AndAlso String.IsNullOrEmpty(TSAURL?.ToString) Then Return Messages.TSA_URL_Missing
        If PAdESLevel = PAdESLevelType.BaselineLTA AndAlso TimeStampHashMethod = HashType.None Then Return Messages.Time_Stamp_Hash_Algorithm_Missing

        If EnableRevocationChecking AndAlso RevocationCheckProtocol = RevocationType.None Then Return Messages.Revocation_Method_Missing
        ' B-LT / B-LTA only exist with collected and embedded revocation data.
        If IsLongTerm AndAlso Not (EnableRevocationChecking AndAlso EmbedRevocationInformation) Then Return Messages.LTV_Requires_Revocation_Embedding

        If IsProxyEnabled Then
            If String.IsNullOrEmpty(ProxyServer) Then Return Messages.Proxy_Server_Missing
            If ProxyServer.Contains("://") OrElse ProxyServer.Contains(":") Then Return Messages.Proxy_Server_Invalid
            If ProxyAuthMethod = ProxyAuthenticationMethod.UserPassword AndAlso String.IsNullOrWhiteSpace(ProxyUserName) Then Return Messages.Proxy_User_Missing
        End If

        Return String.Empty
    End Function

    ''' <summary>Loads the block from the document class; a class without a stored PAdES level keeps the constructor defaults.</summary>
    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        With Parser
            Dim levelValue As String = .ReadSetupCSS(CSS.PAdESLevel)
            If String.IsNullOrEmpty(levelValue) Then Return

            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsPDFSignerEnabled))
            SigningOrganization = .ReadSetupCSS(CSS.SigningOrganization)
            SigningReason = .ReadSetupCSS(CSS.SigningReason)
            PAdESLevel = .ReadSetupCSSInteger(CSS.PAdESLevel, PAdESLevelType.BaselineB)
            SignatureHashMethod = .ReadSetupCSSInteger(CSS.SignatureHashMethod, HashType.SHA256)
            AllowQualifiedCertificatesOnly = Converter.StringToBoolean(.ReadSetupCSS(CSS.AllowQualifiedCertificatesOnly))
            SignerNameFromLoggedOnUser = Converter.StringToBoolean(.ReadSetupCSS(CSS.SignerNameFromLoggedOnUser))

            TSAURL = Converter.StringToUri(.ReadSetupCSS(CSS.TSAURL))
            TSAUserName = .ReadSetupCSS(CSS.TSAUserName)
            TSAPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.TSAPassword))
            TimeStampHashMethod = .ReadSetupCSSInteger(CSS.TimeStampHashMethod, HashType.SHA256)

            EnableRevocationChecking = Converter.StringToBoolean(.ReadSetupCSS(CSS.EnableRevocationChecking))
            RevocationCheckProtocol = .ReadSetupCSSInteger(CSS.RevocationCheck, RevocationType.OCSP)
            EmbedRevocationInformation = Converter.StringToBoolean(.ReadSetupCSS(CSS.EmbedRevocationInformation))

            IsProxyEnabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsProxyEnabled))
            ProxyAuthMethod = .ReadSetupCSSInteger(CSS.ProxyAuthMethod, 0)
            ProxyServer = .ReadSetupCSS(CSS.ProxyServer)
            ProxyPort = .ReadSetupCSSInteger(CSS.ProxyPort, 8080)
            ProxyUserName = .ReadSetupCSS(CSS.ProxyUserName)
            ProxyPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.ProxyPassword))
        End With
    End Sub

    Public Overrides Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
        With Parser
            .WriteSetupCSS(CSS.IsPDFSignerEnabled, Converter.BooleanToNumericString(Enabled))
            .WriteSetupCSS(CSS.SigningOrganization, SigningOrganization)
            .WriteSetupCSS(CSS.SigningReason, SigningReason)
            .WriteSetupCSS(CSS.PAdESLevel, PAdESLevel)
            .WriteSetupCSS(CSS.SignatureHashMethod, SignatureHashMethod)
            .WriteSetupCSS(CSS.AllowQualifiedCertificatesOnly, Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly))
            .WriteSetupCSS(CSS.SignerNameFromLoggedOnUser, Converter.BooleanToNumericString(SignerNameFromLoggedOnUser))

            .WriteSetupCSS(CSS.TSAURL, If(TSAURL Is Nothing, String.Empty, TSAURL.ToString))
            .WriteSetupCSS(CSS.TSAUserName, TSAUserName)
            .WriteSetupCSS(CSS.TSAPassword, enc.AES256Encrypt(TSAPassword))
            .WriteSetupCSS(CSS.TimeStampHashMethod, TimeStampHashMethod)

            .WriteSetupCSS(CSS.EnableRevocationChecking, Converter.BooleanToNumericString(EnableRevocationChecking))
            .WriteSetupCSS(CSS.RevocationCheck, RevocationCheckProtocol)
            .WriteSetupCSS(CSS.EmbedRevocationInformation, Converter.BooleanToNumericString(EmbedRevocationInformation))

            .WriteSetupCSS(CSS.IsProxyEnabled, Converter.BooleanToNumericString(IsProxyEnabled))
            .WriteSetupCSS(CSS.ProxyAuthMethod, ProxyAuthMethod)
            .WriteSetupCSS(CSS.ProxyServer, ProxyServer)
            .WriteSetupCSS(CSS.ProxyPort, ProxyPort.ToString)
            .WriteSetupCSS(CSS.ProxyUserName, ProxyUserName)
            .WriteSetupCSS(CSS.ProxyPassword, enc.AES256Encrypt(ProxyPassword))
        End With
    End Sub

    Public Overrides Function SetupDataToXml(EncryptSecrets As Boolean) As XElement
        Return New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("SigningOrganization", SigningOrganization),
                New XElement("SigningReason", SigningReason),
                New XElement("PAdESLevel", PAdESLevel),
                New XElement("SignatureHashMethod", SignatureHashMethod),
                New XElement("AllowQualifiedCertificatesOnly", Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly)),
                New XElement("SignerNameFromLoggedOnUser", Converter.BooleanToNumericString(SignerNameFromLoggedOnUser)),
                New XElement("TSAURL", TSAURL),
                New XElement("TSAUserName", TSAUserName),
                New XElement("TSAPassword", SecretToXml(TSAPassword, EncryptSecrets)),
                New XElement("TimeStampHashMethod", TimeStampHashMethod),
                New XElement("EnableRevocationChecking", Converter.BooleanToNumericString(EnableRevocationChecking)),
                New XElement("RevocationCheck", RevocationCheckProtocol),
                New XElement("EmbedRevocationInformation", Converter.BooleanToNumericString(EmbedRevocationInformation)),
                New XElement("IsProxyEnabled", Converter.BooleanToNumericString(IsProxyEnabled)),
                New XElement("ProxyServer", ProxyServer),
                New XElement("ProxyPort", ProxyPort),
                New XElement("ProxyAuthMethod", ProxyAuthMethod),
                New XElement("ProxyUserName", ProxyUserName),
                New XElement("ProxyPassword", SecretToXml(ProxyPassword, EncryptSecrets)))
    End Function

    ''' <summary>Restores the block from a backup; a backup without a PAdESLevel element is mapped through <see cref="LegacyLevelDerivation"/>.</summary>
    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        SigningOrganization = ConfigElement.Element("SigningOrganization")
        SigningReason = ConfigElement.Element("SigningReason")
        SignatureHashMethod = CInt(ConfigElement.Element("SignatureHashMethod"))
        AllowQualifiedCertificatesOnly = Converter.StringToBoolean(ConfigElement.Element("AllowQualifiedCertificatesOnly"))
        SignerNameFromLoggedOnUser = Converter.StringToBoolean(ConfigElement.Element("SignerNameFromLoggedOnUser"))

        TSAURL = Converter.StringToUri(ConfigElement.Element("TSAURL"))
        TSAUserName = ConfigElement.Element("TSAUserName")
        TSAPassword = enc.AES256Decrypt(ConfigElement.Element("TSAPassword"))
        TimeStampHashMethod = CInt(ConfigElement.Element("TimeStampHashMethod"))

        If ConfigElement.Element("PAdESLevel") IsNot Nothing Then
            PAdESLevel = CInt(ConfigElement.Element("PAdESLevel"))
            EnableRevocationChecking = Converter.StringToBoolean(ConfigElement.Element("EnableRevocationChecking"))
            RevocationCheckProtocol = CInt(ConfigElement.Element("RevocationCheck"))
            EmbedRevocationInformation = Converter.StringToBoolean(ConfigElement.Element("EmbedRevocationInformation"))
        Else
            Dim derived As LegacyLevelDerivation = LegacyLevelDerivation.FromLegacy(
                Converter.StringToBoolean(ConfigElement.Element("IsTimeStampingEnabled")),
                Converter.StringToBoolean(ConfigElement.Element("IsDocumentTimeStamp")),
                CInt(ConfigElement.Element("RevocationCheck")))
            PAdESLevel = derived.Level
            EnableRevocationChecking = derived.EnableRevocationChecking
            RevocationCheckProtocol = derived.RevocationCheckProtocol
            EmbedRevocationInformation = derived.EmbedRevocationInformation
        End If

        IsProxyEnabled = Converter.StringToBoolean(ConfigElement.Element("IsProxyEnabled"))
        ProxyServer = ConfigElement.Element("ProxyServer")
        ProxyPort = ConfigElement.Element("ProxyPort")
        ProxyAuthMethod = CInt(ConfigElement.Element("ProxyAuthMethod"))
        ProxyUserName = ConfigElement.Element("ProxyUserName")
        ProxyPassword = enc.AES256Decrypt(ConfigElement.Element("ProxyPassword"))
    End Sub
End Class
