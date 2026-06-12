Public MustInherit Class CryptoProviderBase
    Friend enc As New Encrypt

    Public Property Enabled As Boolean
    Public MustOverride ReadOnly Property ProviderType As CryptoProviderType
    Public MustOverride ReadOnly Property ConfigXmlElementName As String
    Public MustOverride ReadOnly Property SupportsLocalCertificates As Boolean

    Public Overrides Function ToString() As String
        Throw New NotImplementedException("ToString() function must be implemented in the cryptographic provider module and return the name of the module.")
    End Function

    Public MustOverride Function Validate() As String
    Public MustOverride Sub LoadSetupDataInAdmin(Parser As SetupCSSParser)
    Public MustOverride Sub SaveSetupDataInAdmin(Parser As SetupCSSParser)
    Public MustOverride Function SetupDataToXml() As XElement
    Public MustOverride Sub SetupDataFromXml(XmlElement As XElement)
End Class
