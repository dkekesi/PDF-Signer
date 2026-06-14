Imports PDFSignerCommon

Friend Class PDFStreamer
    Private _settings As PDFStreamerCryptoProvider

    Friend Sub Initialize(ProviderSettings As PDFStreamerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Function SignDocumentStream(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult

        Dim client As New PDFStreamerRestClient
        Dim callResult As PDFStreamerRestResult = client.Sign(
            _settings.PDFStreamerURL,
            _settings.PDFStreamerAuthorizationCode,
            _settings.PDFStreamerConfigFile,
            "1",
            "PDFSigner.pdf",
            Request.FileToSign)

        res.SignatureLog = callResult.DocumentLog
        res.ErrorMessage = callResult.ErrorMessage
        res.ExceptionText = callResult.ExceptionText

        If callResult.ValidUntil.HasValue Then
            res.SignatureExpiration = callResult.ValidUntil.Value
        End If

        If callResult.Success Then
            res.SignedFile = callResult.SignedStream
        End If

        Return res
    End Function
End Class
