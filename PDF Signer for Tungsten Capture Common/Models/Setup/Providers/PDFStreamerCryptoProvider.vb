Imports PDFSignerCommon.My.Resources

Public Class PDFStreamerCryptoProvider
    Inherits CryptoProviderBase

    Public Overrides Function ToString() As String
        Return "PDF Streamer"
    End Function

    Public Overrides ReadOnly Property ProviderType As CryptoProviderType = CryptoProviderType.PDFStreamer
    Public Overrides ReadOnly Property ConfigXmlElementName As String = "PDFStreamerCryptoProvider"
    Public Overrides ReadOnly Property SupportsLocalCertificates As Boolean = False

    Public Property PDFStreamerURL As Uri
    Public Property PDFStreamerConfigFile As String
    Public Property PDFStreamerAuthorizationCode As String

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(PDFStreamerConfigFile) Then PDFStreamerConfigFile = PDFStreamerConfigFile.Trim

        If Enabled Then
            If PDFStreamerURL Is Nothing OrElse String.IsNullOrEmpty(PDFStreamerURL?.ToString) Then
                Return Messages.PDFStreamer_URL_Missing
            End If
            If Not PDFStreamerURL.IsAbsoluteUri Then
                Return Messages.URL_Not_Complete
            End If
            If Not PDFStreamerURL.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) AndAlso
               Not PDFStreamerURL.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) Then
                Return Messages.PDFStreamer_URL_Not_Http
            End If
            If String.IsNullOrEmpty(PDFStreamerConfigFile) Then
                Return Messages.PDFStreamer_ConfigFile_Missing
            End If
            If String.IsNullOrEmpty(PDFStreamerAuthorizationCode) Then
                Return Messages.PDFStreamer_AuthCode_Missing
            End If
        End If

        Return String.Empty
    End Function

    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsPDFStreamerEnabled))
            PDFStreamerURL = Converter.StringToUri(.ReadSetupCSS(CSS.PDFStreamerURL))
            PDFStreamerConfigFile = .ReadSetupCSS(CSS.PDFStreamerConfigFile)
            PDFStreamerAuthorizationCode = enc.AES256Decrypt(.ReadSetupCSS(CSS.PDFStreamerAuthorizationCode))
        End With
    End Sub

    Public Overrides Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            .WriteSetupCSS(CSS.IsPDFStreamerEnabled, Converter.BooleanToNumericString(Enabled))
            If PDFStreamerURL Is Nothing Then
                .WriteSetupCSS(CSS.PDFStreamerURL, String.Empty)
            Else
                .WriteSetupCSS(CSS.PDFStreamerURL, PDFStreamerURL?.ToString)
            End If
            .WriteSetupCSS(CSS.PDFStreamerConfigFile, PDFStreamerConfigFile)
            .WriteSetupCSS(CSS.PDFStreamerAuthorizationCode, enc.AES256Encrypt(PDFStreamerAuthorizationCode))
        End With
    End Sub

    Public Overrides Function SetupDataToXml(EncryptSecrets As Boolean) As XElement
        Dim dom As New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("URL", PDFStreamerURL),
                New XElement("ConfigFile", PDFStreamerConfigFile),
                New XElement("AuthorizationCode", SecretToXml(PDFStreamerAuthorizationCode, EncryptSecrets)))

        Return dom
    End Function

    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        PDFStreamerURL = Converter.StringToUri(ConfigElement.Element("URL"))
        PDFStreamerConfigFile = ConfigElement.Element("ConfigFile")
        PDFStreamerAuthorizationCode = enc.AES256Decrypt(ConfigElement.Element("AuthorizationCode"))
    End Sub
End Class
