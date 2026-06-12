Imports PDFSigner.PDFStreamerServiceReference
Imports PDFSignerCommon
Imports PDFStreamer.WCFCommon.Helper
Imports System.IO

Friend Class PDFStreamer
    Private _settings As PDFStreamerCryptoProvider

    Friend Sub Initialize(ProviderSettings As PDFStreamerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Function SignDocumentStream(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult

        Dim req As New StreamedDocumentCreateRequest
        Dim response As New StreamedDocumentCreateResponse

        With req
            .DocumentConfigurationFilename = _settings.PDFStreamerConfigFile
            .AuthorizationKey = _settings.PDFStreamerAuthorizationCode
            .DocumentStream = Request.FileToSign
            .DocumentFileNameWithExtension = "PDFSigner.pdf"
            .TransactionID = 1
        End With

        req.DocumentStream.Seek(0, SeekOrigin.Begin)

        PDFStreamerProxy.SetDefaultBindingAndAddress(_settings.PDFStreamerURL.ToString, False)
        Try
            Using clt As New PDFStreamerClient(PDFStreamerProxy.ServiceBinding, PDFStreamerProxy.RemoteAddress)
                response.DocumentLog = clt.CreatePDFStream(req.AuthorizationKey, req.DocumentConfigurationFilename, req.DocumentFileNameWithExtension, req.TransactionID, req.DocumentStream, response.ErrorMessage, response.SignerNameFromCert, response.ValidUntil)
            End Using
        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = ex.ToString
            Return res
        End Try

        res.ErrorMessage = response.ErrorMessage
        res.SignatureLog = response.DocumentLog

        If response.ValidUntil IsNot Nothing Then
            res.SignatureExpiration = response.ValidUntil
        End If

        ' read from WCF stream
        Dim bufferLen As Integer = 65000
        Dim buffer() As Byte = New Byte(bufferLen - 1) {}
        Dim count As Integer
        Dim hasData As Boolean = True

        ' we have to seek to the beginning before writing,
        'otherwise the signed file will be appended to the unsigned byte stream
        Request.FileToSign.Seek(0, SeekOrigin.Begin)

        While hasData
            count = req.DocumentStream.Read(buffer, 0, bufferLen)
            If count > 0 Then
                Request.FileToSign.Write(buffer, 0, count)
            Else
                hasData = False
            End If
        End While

        res.SignedFile = Request.FileToSign

        Return res

    End Function
End Class
