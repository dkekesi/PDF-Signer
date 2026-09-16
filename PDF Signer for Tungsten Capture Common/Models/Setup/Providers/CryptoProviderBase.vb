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
    ''' <summary>
    ''' Serializes the provider settings. Secrets are AES-encrypted with a random IV when <paramref name="EncryptSecrets"/> is True,
    ''' so only unencrypted output is stable enough to compare.
    ''' </summary>
    Public MustOverride Function SetupDataToXml(EncryptSecrets As Boolean) As XElement
    Public MustOverride Sub SetupDataFromXml(XmlElement As XElement)

    ''' <summary>Returns a secret value for XML output, AES-encrypted when <paramref name="EncryptSecrets"/> is True.</summary>
    Protected Function SecretToXml(Value As String, EncryptSecrets As Boolean) As String
        Return If(EncryptSecrets, enc.AES256Encrypt(Value), Value)
    End Function
End Class
