Imports System.Security.Cryptography.X509Certificates
Imports System.IO
Imports PDFSigner.My.Resources
Imports PDFSignerCommon

Friend Class SignatureRequest
    Friend Property FileToSign As Stream
    Friend Property UnsignedFileName As String ' only the file name, we will save the file to the appropriate path, not the crypto module
    Friend Property SigningCertificate As SigningCertificate
    Friend Property SignSettings As CryptoProviderBase
    Friend Property MetaDataSettings As MetaData
    Friend Property DocItem As DocumentItem
End Class

Friend Class SignatureResult
    Friend Property ErrorMessage As String
    Friend Property SignedFile As Stream
    Friend Property SignedAt As DateTime
    Friend Property SignatureExpiration As DateTime
    Friend Property DetachedSignatureFileName As String
    Friend Property DetachedSignature As Byte()
    Friend Property SignatureLog As String
    Friend Property ExceptionText As String

    Public ReadOnly Property SaveDetachedSignature As Boolean
        Get
            Return DetachedSignature IsNot Nothing
        End Get
    End Property

    Friend Property MetaDataContent As String
    Friend Property MetaDataFileName As String
    Public ReadOnly Property SaveMetaDataAsFile As Boolean
        Get
            Return Not String.IsNullOrEmpty(MetaDataFileName)
        End Get
    End Property

    Friend Function Validate(Document As DocumentItem) As String
        ' *************** more thorough checking needed when we open up for other crypto providers
        If Not String.IsNullOrEmpty(ErrorMessage) Then Return ErrorMessage

        If SignedFile Is Nothing OrElse SignedFile.Length = 0 Then Return Messages.Error_No_Signed_File_Returned

        ' comparing original and files
        If Document.UnsignedFileSize >= SignedFile.Length AndAlso Not SaveDetachedSignature Then
            Return String.Format(Messages.Error_File_Sizes_Identical, SignedFile.Length, Document.UnsignedFileSize)
        End If

        Return Nothing
    End Function
End Class

Friend Class SigningCertificate
    Friend Property Certificate As X509Certificate2

    Friend Property IsQualified As Boolean

    Public Overrides Function ToString() As String
        If Certificate Is Nothing Then Return String.Empty

        Try
            Dim ct As New CertificateTranslator
            Dim res As String = ct.GetIssuedToName(Certificate)

            If IsQualified Then res += GUIText.Cert_Qualified

            Return res

        Catch ex As Exception
            Return Certificate.Subject
        End Try
    End Function

    Friend Function GetIssuedToName() As String
        If Certificate IsNot Nothing Then
            Dim ct As New CertificateTranslator
            Return ct.GetIssuedToName(Certificate)
        End If

        Return String.Empty
    End Function
End Class