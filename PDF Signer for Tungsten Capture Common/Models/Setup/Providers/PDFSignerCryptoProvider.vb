Imports PDFSignerCommon.My.Resources

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
    Public Property RevocationCheck As Integer
    Public Property SignatureHashMethod As Integer
    Public Property IsTimeStampingEnabled As Boolean

    Public Property AllowQualifiedCertificatesOnly As Boolean
    Public Property TSAURL As Uri
    Public Property TSAUserName As String
    Public Property TSAPassword As String
    Public Property IsDocumentTimeStamp As Boolean
    Public Property IsSinglePassPadesBLTA As Boolean
    Public Property TimeStampHashMethod As Integer

    Public Property IsProxyEnabled As Boolean
    Public Property ProxyServer As String
    Public Property ProxyPort As Integer
    Public Property ProxyAuthMethod As Integer
    Public Property ProxyUserName As String
    Public Property ProxyPassword As String

    Public Sub New()
        RevocationCheck = RevocationType.CRL
        SignatureHashMethod = HashType.SHA256
        TimeStampHashMethod = HashType.SHA256
        ProxyPort = 8080
        ProxyAuthMethod = ProxyAuthenticationMethod.NoAuthentication
    End Sub

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(SigningOrganization) Then SigningOrganization = SigningOrganization.Trim
        If Not String.IsNullOrEmpty(SigningReason) Then SigningReason = SigningReason.Trim
        If Not String.IsNullOrEmpty(TSAUserName) Then TSAUserName = TSAUserName.Trim
        If Not String.IsNullOrEmpty(ProxyServer) Then ProxyServer = ProxyServer.Trim
        If Not String.IsNullOrEmpty(ProxyUserName) Then ProxyUserName = ProxyUserName.Trim

        If Enabled Then
            If String.IsNullOrEmpty(RevocationCheck) Then
                Return Messages.Revocation_Method_Missing
            End If
            If String.IsNullOrEmpty(SignatureHashMethod) Then
                Return Messages.Signature_Hash_Algorithm_Missing
            End If

            If IsTimeStampingEnabled Then
                If String.IsNullOrEmpty(TSAURL?.ToString) Then
                    Return Messages.TSA_URL_Missing
                End If
                If String.IsNullOrEmpty(TimeStampHashMethod) Then
                    Return Messages.Time_Stamp_Hash_Algorithm_Missing
                End If

                If IsSinglePassPadesBLTA AndAlso (Not IsDocumentTimeStamp OrElse RevocationCheck <> RevocationType.OCSP) Then
                    Return Messages.LTA_Signature_Config_Incorrect
                End If
            End If

            If IsProxyEnabled Then
                If String.IsNullOrEmpty(ProxyServer) Then
                    Return Messages.Proxy_Server_Missing
                End If

                If ProxyServer.Contains("://") OrElse ProxyServer.Contains(":") Then
                    Return Messages.Proxy_Server_Invalid
                End If

                If String.IsNullOrEmpty(ProxyAuthMethod) Then
                    Return Messages.Proxy_Auth_Method_Missing
                End If

                If ProxyAuthMethod = ProxyAuthenticationMethod.UserPassword AndAlso String.IsNullOrWhiteSpace(ProxyUserName) Then
                    Return Messages.Proxy_User_Missing
                End If
            End If
        End If

        Return String.Empty
    End Function

    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        With Parser
            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsPDFSignerEnabled))
            SigningOrganization = .ReadSetupCSS(CSS.SigningOrganization)
            SigningReason = .ReadSetupCSS(CSS.SigningReason)
            RevocationCheck = .ReadSetupCSSInteger(CSS.RevocationCheck, 0)
            SignatureHashMethod = .ReadSetupCSSInteger(CSS.SignatureHashMethod, 28932)
            AllowQualifiedCertificatesOnly = Converter.StringToBoolean(.ReadSetupCSS(CSS.AllowQualifiedCertificatesOnly))

            IsTimeStampingEnabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsTimeStampingEnabled))
            TSAURL = Converter.StringToUri(.ReadSetupCSS(CSS.TSAURL))
            TSAUserName = .ReadSetupCSS(CSS.TSAUserName)
            TSAPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.TSAPassword))
            IsDocumentTimeStamp = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsDocumentTimeStamp))
            IsSinglePassPadesBLTA = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsSinglePassPadesBLTA))
            TimeStampHashMethod = .ReadSetupCSSInteger(CSS.TimeStampHashMethod, 28932)

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
            .WriteSetupCSS(CSS.RevocationCheck, RevocationCheck)
            .WriteSetupCSS(CSS.SignatureHashMethod, SignatureHashMethod)
            .WriteSetupCSS(CSS.AllowQualifiedCertificatesOnly, Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly))

            .WriteSetupCSS(CSS.IsTimeStampingEnabled, Converter.BooleanToNumericString(IsTimeStampingEnabled))
            If TSAURL Is Nothing Then
                .WriteSetupCSS(CSS.TSAURL, String.Empty)
            Else
                .WriteSetupCSS(CSS.TSAURL, TSAURL?.ToString)
            End If
            .WriteSetupCSS(CSS.TSAUserName, TSAUserName)
            .WriteSetupCSS(CSS.TSAPassword, enc.AES256Encrypt(TSAPassword))
            .WriteSetupCSS(CSS.IsDocumentTimeStamp, Converter.BooleanToNumericString(IsDocumentTimeStamp))
            .WriteSetupCSS(CSS.IsSinglePassPadesBLTA, Converter.BooleanToNumericString(IsSinglePassPadesBLTA))
            .WriteSetupCSS(CSS.TimeStampHashMethod, TimeStampHashMethod)

            .WriteSetupCSS(CSS.IsProxyEnabled, Converter.BooleanToNumericString(IsProxyEnabled))
            .WriteSetupCSS(CSS.ProxyAuthMethod, ProxyAuthMethod)
            .WriteSetupCSS(CSS.ProxyServer, ProxyServer)
            .WriteSetupCSS(CSS.ProxyPort, ProxyPort.ToString)
            .WriteSetupCSS(CSS.ProxyUserName, ProxyUserName)
            .WriteSetupCSS(CSS.ProxyPassword, enc.AES256Encrypt(ProxyPassword))
        End With
    End Sub

    Public Overrides Function SetupDataToXml() As XElement
        Dim dom As New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("SigningOrganization", SigningOrganization),
                New XElement("SigningReason", SigningReason),
                New XElement("RevocationCheck", RevocationCheck),
                New XElement("SignatureHashMethod", SignatureHashMethod),
                New XElement("AllowQualifiedCertificatesOnly", Converter.BooleanToNumericString(AllowQualifiedCertificatesOnly)),
                New XElement("IsTimeStampingEnabled", Converter.BooleanToNumericString(IsTimeStampingEnabled)),
                New XElement("TSAURL", TSAURL),
                New XElement("TSAUserName", TSAUserName),
                New XElement("TSAPassword", enc.AES256Encrypt(TSAPassword)),
                New XElement("IsDocumentTimeStamp", Converter.BooleanToNumericString(IsDocumentTimeStamp)),
                New XElement("IsSinglePassPadesBLTA", Converter.BooleanToNumericString(IsSinglePassPadesBLTA)),
                New XElement("TimeStampHashMethod", TimeStampHashMethod),
                New XElement("IsProxyEnabled", Converter.BooleanToNumericString(IsProxyEnabled)),
                New XElement("ProxyServer", ProxyServer),
                New XElement("ProxyPort", ProxyPort),
                New XElement("ProxyAuthMethod", ProxyAuthMethod),
                New XElement("ProxyUserName", ProxyUserName),
                New XElement("ProxyPassword", enc.AES256Encrypt(ProxyPassword)))

        Return dom
    End Function

    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        SigningOrganization = ConfigElement.Element("SigningOrganization")
        SigningReason = ConfigElement.Element("SigningReason")
        RevocationCheck = CInt(ConfigElement.Element("RevocationCheck"))
        SignatureHashMethod = CInt(ConfigElement.Element("SignatureHashMethod"))
        AllowQualifiedCertificatesOnly = Converter.StringToBoolean(ConfigElement.Element("AllowQualifiedCertificatesOnly"))

        IsTimeStampingEnabled = Converter.StringToBoolean(ConfigElement.Element("IsTimeStampingEnabled"))
        TSAURL = Converter.StringToUri(ConfigElement.Element("TSAURL"))
        TSAUserName = ConfigElement.Element("TSAUserName")
        TSAPassword = enc.AES256Decrypt(ConfigElement.Element("TSAPassword"))
        IsDocumentTimeStamp = Converter.StringToBoolean(ConfigElement.Element("IsDocumentTimeStamp"))
        IsSinglePassPadesBLTA = Converter.StringToBoolean(ConfigElement.Element("IsSinglePassPadesBLTA"))
        TimeStampHashMethod = CInt(ConfigElement.Element("TimeStampHashMethod"))

        IsProxyEnabled = Converter.StringToBoolean(ConfigElement.Element("IsProxyEnabled"))
        ProxyServer = ConfigElement.Element("ProxyServer")
        ProxyPort = ConfigElement.Element("ProxyPort")
        ProxyAuthMethod = CInt(ConfigElement.Element("ProxyAuthMethod"))
        ProxyUserName = ConfigElement.Element("ProxyUserName")
        ProxyPassword = enc.AES256Decrypt(ConfigElement.Element("ProxyPassword"))
    End Sub
End Class
