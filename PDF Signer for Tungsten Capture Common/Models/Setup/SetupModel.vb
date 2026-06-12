Public Class SetupModel
    Public Property IsSigningEnabled As Boolean

    Public Property IndexIsSigned As String
    Public Property IndexSignedDateTime As String
    Public Property IndexSignedBy As String
    Public Property IndexSignatureValidUntil As String
    Public Property IndexSkipNote As String

    Public Property SignAllDocuments As Boolean
    Public Property IndexBarCode As String
    Public Property SignatureMarker As String
    Public Property SignatureMarkerPosition As Integer
    Public Property AllowSkipRequiredDocument As Boolean

    Public Property IndexDocumentName As String
    Public Property DocumentNameFromFormType As Boolean
    Public Property DocumentNameDefault As String
    Public Property ConvertingOrganization As String
    Public Property ConvertingRegulationName As String
    Public Property ConvertingRegulationURL As Uri
    Public Property ConvertingRegulationVersion As String
    Public Property MetadataFormat As Integer
    Public Property SignaturePolicyURL As Uri
    Public Property SignaturePolicyHash As String
    Public Property SignaturePolicyOID As String

    Public Property FileOverwriteOriginal As Boolean
    Public Property FileCreateNew As Boolean
    Public Property FileNameAppend As String
    Public Property FileExtensionReplace As String

    Public Property DocumentViewer As Integer
    Public Property DefaultCryptographicProvider As Integer
    Public Property CryptographicProviders As New List(Of CryptoProviderBase)
    Public Property PDFSignerProvider As New PDFSignerCryptoProvider
    Public Property PDFStreamerProvider As New PDFStreamerCryptoProvider
    Public Property MQFTPProvider As New MQFTPCryptoProvider
    Public Property MNBSignerProvider As New MNBSignerCryptoProvider

    Public Sub New()
        CryptographicProviders.Add(PDFSignerProvider)
        CryptographicProviders.Add(PDFStreamerProvider)
        CryptographicProviders.Add(MQFTPProvider)
        CryptographicProviders.Add(MNBSignerProvider)
    End Sub
End Class
