Imports PDFSignerCommon.My.Resources

Public Class MQFTPCryptoProvider
    Inherits CryptoProviderBase

    Public Overrides Function ToString() As String
        Return "MQFTP"
    End Function

    Public Overrides ReadOnly Property ProviderType As CryptoProviderType = CryptoProviderType.MQFTP
    Public Overrides ReadOnly Property ConfigXmlElementName As String = "MQFTPCryptoProvider"
    Public Overrides ReadOnly Property SupportsLocalCertificates As Boolean = False

    Public Property MQFolderOut As String
    Public Property MQFolderIn As String
    Public Property MQDomain As String
    Public Property MQUserName As String
    Public Property MQPassword As String
    Public Property IndexMQDocUID As String

    Public Overrides Function Validate() As String
        If Not String.IsNullOrEmpty(MQFolderOut) Then MQFolderOut = MQFolderOut.Trim
        If Not String.IsNullOrEmpty(MQFolderIn) Then MQFolderIn = MQFolderIn.Trim
        If Not String.IsNullOrEmpty(MQDomain) Then MQDomain = MQDomain.Trim
        If Not String.IsNullOrEmpty(MQUserName) Then MQUserName = MQUserName.Trim

        If Enabled Then
            If String.IsNullOrEmpty(MQFolderOut) Then
                Return Messages.MQ_FolderOut_Missing
            End If
            If String.IsNullOrEmpty(MQFolderIn) Then
                Return Messages.MQ_FolderIn_Missing
            End If
            If String.IsNullOrEmpty(MQDomain) Then
                Return Messages.MQ_Domain_Missing
            End If
            If String.IsNullOrEmpty(MQUserName) Then
                Return Messages.MQ_UserName_Missing
            End If
            If String.IsNullOrEmpty(MQPassword) Then
                Return Messages.MQ_Password_Missing
            End If
        End If

        Return String.Empty
    End Function

    Public Overrides Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            Enabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsMQEnabled))
            MQFolderOut = .ReadSetupCSS(CSS.MQFolderOut)
            MQFolderIn = .ReadSetupCSS(CSS.MQFolderIn)
            MQDomain = .ReadSetupCSS(CSS.MQDomain)
            MQUserName = .ReadSetupCSS(CSS.MQUserName)
            MQPassword = enc.AES256Decrypt(.ReadSetupCSS(CSS.MQPassword))
            IndexMQDocUID = .ReadSetupCSS(CSS.MQDocUID)
        End With
    End Sub

    Public Overrides Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
        Dim enc As New Encrypt

        With Parser
            .WriteSetupCSS(CSS.IsMQEnabled, Converter.BooleanToNumericString(Enabled))
            .WriteSetupCSS(CSS.MQFolderOut, MQFolderOut)
            .WriteSetupCSS(CSS.MQFolderIn, MQFolderIn)
            .WriteSetupCSS(CSS.MQDomain, MQDomain)
            .WriteSetupCSS(CSS.MQUserName, MQUserName)
            .WriteSetupCSS(CSS.MQPassword, enc.AES256Encrypt(MQPassword))
            .WriteSetupCSS(CSS.MQDocUID, IndexMQDocUID)
        End With
    End Sub

    Public Overrides Function SetupDataToXml() As XElement
        Dim dom As New XElement(ConfigXmlElementName,
                New XElement("Enabled", Converter.BooleanToNumericString(Enabled)),
                New XElement("FolderOut", MQFolderOut),
                New XElement("FolderIn", MQFolderIn),
                New XElement("Domain", MQDomain),
                New XElement("UserName", MQUserName),
                New XElement("Password", enc.AES256Encrypt(MQPassword)),
                New XElement("DocUID", IndexMQDocUID))

        Return dom
    End Function

    Public Overrides Sub SetupDataFromXml(ConfigElement As XElement)
        Enabled = Converter.StringToBoolean(ConfigElement.Element("Enabled"))
        MQFolderOut = ConfigElement.Element("FolderOut")
        MQFolderIn = ConfigElement.Element("FolderIn")
        MQDomain = ConfigElement.Element("Domain")
        MQUserName = ConfigElement.Element("UserName")
        MQPassword = enc.AES256Decrypt(ConfigElement.Element("Password"))
        IndexMQDocUID = ConfigElement.Element("DocUID")
    End Sub
End Class
