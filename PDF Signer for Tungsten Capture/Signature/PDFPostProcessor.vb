Imports PDFSigner.My.Resources
Imports SBPDF
Imports System.IO
Imports System.Text

Friend Class PDFPostProcessor
    Private _request As PostProcessRequest
    Private _docTempFile As String
    Private _fileTempFile As String

    Friend Function Process(Request As PostProcessRequest) As PostProcessResult
        Dim res As New PostProcessResult
        Dim sbPostProcessLog As New StringBuilder
        Dim doc As New TElPDFDocument

        _request = Request

        Try
            ' Override SBB MemoryStream if file size is above threshold
            If _request.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
                Dim fo As New FileOperation
                _docTempFile = fo.GetTempFile("ds-")
                _fileTempFile = fo.GetTempFile("fs-")

                AddHandler doc.OnCreateTemporaryStream, AddressOf DocOnCreateTemporaryStream
                AddHandler doc.PDFFile.OnCreateTemporaryStream, AddressOf FileOnCreateTemporaryStream
            End If

            Dim originalAttachmentCount As Integer = 0

            sbPostProcessLog.AppendLine()
            sbPostProcessLog.AppendLine(Messages.PostProcess_Validating_Signed_PDF)

            Try
                sbPostProcessLog.AppendLine(Messages.Check_PDF_Can_Be_Opened)
                _request.FileToPostProcess.Seek(0, SeekOrigin.Begin)
                doc.Open(_request.FileToPostProcess)
                sbPostProcessLog.AppendLine(Messages.PDF_Can_Be_Opened)
            Catch ex As Exception
                res.ErrorMessage = String.Format(Messages.PDF_Cannot_Be_Opened, ex.Message)
                res.PostProcessLog = sbPostProcessLog.ToString
                Return res
            End Try

            sbPostProcessLog.AppendLine(Messages.Validating_PDF_Attachments)
            If doc.AttachedFileCount < 1 Then
                res.ErrorMessage = Messages.Error_No_Attachments_In_PDF
                res.PostProcessLog = sbPostProcessLog.ToString
                Return res
            End If
            sbPostProcessLog.AppendLine(Messages.PDF_Has_Attachments)

            sbPostProcessLog.AppendLine(Messages.Validating_PDF_Signature)
            If doc.SignatureCount < 1 Then
                res.ErrorMessage = Messages.Error_No_Signature_In_PDF
                res.PostProcessLog = sbPostProcessLog.ToString
                Return res
            End If
            sbPostProcessLog.AppendLine(Messages.PDF_Has_Signature)

            doc.Close(False)

            sbPostProcessLog.AppendLine(Messages.PostProcess_Validation_Completed)
            res.PostProcessLog = sbPostProcessLog.ToString

            Return res

        Catch ex As Exception
            res.ErrorMessage = ex.Message
            sbPostProcessLog.AppendLine(res.ErrorMessage)
            res.PostProcessLog = sbPostProcessLog.ToString
            Return res

        Finally
            If _request.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
                Try
                    If File.Exists(_docTempFile) Then File.Delete(_docTempFile)
                    If File.Exists(_fileTempFile) Then File.Delete(_fileTempFile)

                    RemoveHandler doc.OnCreateTemporaryStream, AddressOf DocOnCreateTemporaryStream
                    RemoveHandler doc.PDFFile.OnCreateTemporaryStream, AddressOf FileOnCreateTemporaryStream
                Catch
                End Try
            End If
        End Try

    End Function

    Private Sub DocOnCreateTemporaryStream(Sender As Object, ByRef Stream As Stream, ByRef FreeOnClose As Boolean)
        If _request.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
            Stream = New FileStream(_docTempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
            FreeOnClose = True
        End If
    End Sub

    Private Sub FileOnCreateTemporaryStream(Sender As Object, ByRef Stream As Stream, ByRef FreeOnClose As Boolean)
        If _request.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
            Stream = New FileStream(_fileTempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
            FreeOnClose = True
        End If
    End Sub

End Class
