Imports System.IO
Imports PDFSignerCommon

Friend Class PreProcessRequest
    Friend Property FileToPreProcess As Stream
    Friend Property FileName As String
    Friend Property MetaDataSettings As MetaData
    Friend Property DocItem As DocumentItem
    Friend Property CryptoProvider As CryptoProviderBase
End Class

Friend Class PreProcessResult
    Friend Property ErrorMessage As String
    Friend Property PreProcessedFile As Stream
    Friend Property PreProcessedFileSize As Integer
    Friend Property MetaDataContent As String
    Friend Property PreProcessLog As String

    Friend Function Validate() As String
        If Not String.IsNullOrEmpty(ErrorMessage) Then Return ErrorMessage

        Return Nothing
    End Function
End Class

