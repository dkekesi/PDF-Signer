Imports PDFSignerCommon.My.Resources

Public Class MNBSignerCryptoProvider
    Inherits CryptoProviderBase

    Public Overrides Function ToString() As String
        Return "MNB Signer"
    End Function

    Public Overrides ReadOnly Property ProviderType As CryptoProviderType = CryptoProviderType.MNBSigner
    Public Overrides ReadOnly Property ConfigXmlElementName As String = "MNBSignerCryptoProvider"
    Public Overrides ReadOnly Property SupportsLocalCertificates As Boolean = False

    Public Property MNBSignerURL As Uri
    Public Property MNBSignerWSTimeout As Integer
    Public Property MNBSignerSigningTimeout As Integer
    Public Property MNBSignerChunkSize As Integer
    Public Property IsMNBSignerWindowsAuthentication As Boolean
    Public Property IsMNBSignerUserPasswordAuthentication As Boolean
    Public Property MNBSignerDomain As String
    Public Property MNBSignerUserName As String
    Public Property MNBSignerPassword As String

    Public Sub New()
        MNBSignerWSTimeout = 60
        MNBSignerSigningTimeout = 600
        MNBSignerChunkSize = 20000000
        IsMNBSignerWindowsAuthentication = True
        IsMNBSignerUserPasswordAuthentication = False
    End Sub

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(MNBSignerDomain) Then MNBSignerDomain = MNBSignerDomain.Trim
        If Not String.IsNullOrEmpty(MNBSignerUserName) Then MNBSignerUserName = MNBSignerUserName.Trim

        If Enabled Then
            If MNBSignerURL Is Nothing OrElse String.IsNullOrEmpty(MNBSignerURL?.ToString) Then
                Return Messages.MNBSigner_URL_Missing
            End If
            If Not MNBSignerURL.IsAbsoluteUri Then
                Return Messages.URL_Not_Complete
            End If
            If MNBSignerURL.Scheme.ToLower <> "http" AndAlso MNBSignerURL.Scheme.ToLower <> "https" Then
                Return Messages.MNBSigner_URL_Not_Http
            End If
            If IsMNBSignerUserPasswordAuthentication AndAlso MNBSignerURL.Scheme <> Uri.UriSchemeHttps Then
                Return Messages.MNBSigner_Basic_Auth_Requires_Https
            End If
            If Not IsMNBSignerWindowsAuthentication AndAlso Not IsMNBSignerUserPasswordAuthentication Then
                Return Messages.MNBSigner_Auth_Method_Not_Selected
            End If
            If IsMNBSignerUserPasswordAuthentication Then
                If String.IsNullOrEmpty(MNBSignerDomain) Then
                    Return Messages.MNBSigner_Domain_Missing
                End If
                If String.IsNullOrEmpty(MNBSignerUserName) Then
                    Return Messages.MNBSigner_UserName_Missing
                End If
                If String.IsNullOrEmpty(MNBSignerPassword) Then
                    Return Messages.MNBSigner_Password_Missing
                End If
            End If
        End If

        Return String.Empty
    End Function

    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsMNBSignerEnabled))
            MNBSignerURL = Converter.StringToUri(.ReadSetupCSS(CSS.MNBSignerURL))
            MNBSignerWSTimeout = .ReadSetupCSSInteger(CSS.MNBSignerWSTimeout, 60)
            MNBSignerSigningTimeout = .ReadSetupCSSInteger(CSS.MNBSignerSigningTimeout, 60)
            MNBSignerChunkSize = .ReadSetupCSSInteger(CSS.MNBSignerChunkSize, 64)
            IsMNBSignerWindowsAuthentication = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsMNBSignerWindowsAuthentication))
            IsMNBSignerUserPasswordAuthentication = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsMNBSignerUserPasswordAuthentication))
            MNBSignerDomain = .ReadSetupCSS(CSS.MNBSignerDomain)
            MNBSignerUserName = .ReadSetupCSS(CSS.MNBSignerUserName)
            MNBSignerPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.MNBSignerPassword))
        End With
    End Sub

    Public Overrides Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            .WriteSetupCSS(CSS.IsMNBSignerEnabled, Converter.BooleanToNumericString(Enabled))
            If MNBSignerURL Is Nothing Then
                .WriteSetupCSS(CSS.MNBSignerURL, String.Empty)
            Else
                .WriteSetupCSS(CSS.MNBSignerURL, MNBSignerURL.ToString)
            End If
            .WriteSetupCSS(CSS.MNBSignerWSTimeout, MNBSignerWSTimeout.ToString)
            .WriteSetupCSS(CSS.MNBSignerSigningTimeout, MNBSignerSigningTimeout.ToString)
            .WriteSetupCSS(CSS.MNBSignerChunkSize, MNBSignerChunkSize.ToString)
            .WriteSetupCSS(CSS.IsMNBSignerWindowsAuthentication, Converter.BooleanToNumericString(IsMNBSignerWindowsAuthentication))
            .WriteSetupCSS(CSS.IsMNBSignerUserPasswordAuthentication, Converter.BooleanToNumericString(IsMNBSignerUserPasswordAuthentication))
            .WriteSetupCSS(CSS.MNBSignerDomain, MNBSignerDomain)
            .WriteSetupCSS(CSS.MNBSignerUserName, MNBSignerUserName)
            .WriteSetupCSS(CSS.MNBSignerPassword, enc.AES256Encrypt(MNBSignerPassword))
        End With
    End Sub

    Public Overrides Function SetupDataToXml() As XElement
        Dim dom As New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("URL", MNBSignerURL),
                New XElement("WSTimeout", MNBSignerWSTimeout),
                New XElement("SigningTimeout", MNBSignerSigningTimeout),
                New XElement("ChunkSize", MNBSignerChunkSize),
                New XElement("IsWindowsAuthentication", Converter.BooleanToNumericString(IsMNBSignerWindowsAuthentication)),
                New XElement("IsUserPasswordAuthentication", Converter.BooleanToNumericString(IsMNBSignerUserPasswordAuthentication)),
                New XElement("Domain", MNBSignerDomain),
                New XElement("UserName", MNBSignerUserName),
                New XElement("Password", enc.AES256Encrypt(MNBSignerPassword)))

        Return dom
    End Function

    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        MNBSignerURL = Converter.StringToUri(ConfigElement.Element("URL"))
        MNBSignerWSTimeout = ConfigElement.Element("WSTimeout")
        MNBSignerSigningTimeout = ConfigElement.Element("SigningTimeout")
        MNBSignerChunkSize = ConfigElement.Element("ChunkSize")
        IsMNBSignerWindowsAuthentication = Converter.StringToBoolean(ConfigElement.Element("IsWindowsAuthentication"))
        IsMNBSignerUserPasswordAuthentication = Converter.StringToBoolean(ConfigElement.Element("IsUserPasswordAuthentication"))
        MNBSignerDomain = ConfigElement.Element("Domain")
        MNBSignerUserName = ConfigElement.Element("UserName")
        MNBSignerPassword = enc.AES256Decrypt(ConfigElement.Element("Password"))
    End Sub
End Class
