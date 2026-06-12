Imports PDFSigner.My.Resources
Imports SBPDF
Imports System.IO
Imports System.Text

Friend Class PDFPreProcessor
    Private _request As PreProcessRequest
    Private _docTempFile As String
    Private _fileTempFile As String

    Friend Function Process(Request As PreProcessRequest) As PreProcessResult
        Dim res As New PreProcessResult
        Dim sbPreProcessLog As New StringBuilder
        Dim originalAttachmentCount As Integer = 0
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

            sbPreProcessLog.AppendLine(Messages.Validating_PDF)

            Try
                doc.Open(_request.FileToPreProcess)
            Catch ex As Exception
                res.ErrorMessage = String.Format(Messages.Error_Opening_PDF_For_Validation, ex.Message)
                sbPreProcessLog.AppendLine(res.ErrorMessage)
                res.PreProcessLog = sbPreProcessLog.ToString
                Return res
            End Try

            sbPreProcessLog.AppendLine(Messages.Checking_PDF_Encryption)
            If doc.Encrypted Then
                res.ErrorMessage = Messages.Error_PDF_Encrypted
                sbPreProcessLog.AppendLine(res.ErrorMessage)
                res.PreProcessLog = sbPreProcessLog.ToString
                Return res
            End If
            sbPreProcessLog.AppendLine(Messages.PDF_Not_Encrypted)

            sbPreProcessLog.AppendLine(Messages.Looking_For_Signatures_In_PDF)
            If doc.SignatureCount > 0 Then
                res.ErrorMessage = Messages.PDF_Already_Signed
                sbPreProcessLog.AppendLine(res.ErrorMessage)
                res.PreProcessLog = sbPreProcessLog.ToString
                Return res
            End If
            sbPreProcessLog.AppendLine(Messages.PDF_No_Signatures)

            sbPreProcessLog.AppendLine(Messages.Validating_PDF_Finished)

            '********** Adding clause **********

            'create the file
            sbPreProcessLog.AppendLine(Messages.Creating_Certification_Clause)
            Dim MetaDataContent As String = _request.MetaDataSettings.ToString(_request.CryptoProvider)
            Dim MetaDataBin As Byte() = Encoding.UTF8.GetBytes(MetaDataContent)

            originalAttachmentCount = doc.AttachedFileCount

            Using msAtt As New MemoryStream(MetaDataBin)
                Dim ind As Integer = doc.AddAttachedFile()
                Dim att As TElPDFFileAttachment = doc.AttachedFiles(ind)
                att.Init(msAtt)
                att.UnicodeFilename = _request.MetaDataSettings.GetFileName
                att.Description = _request.MetaDataSettings.GetDescription
                att.ModificationDate = _request.MetaDataSettings.ConversionDate

                sbPreProcessLog.AppendLine(String.Format(Messages.Clause_Attachment_Prepared, att.UnicodeFilename))
            End Using

            res.MetaDataContent = MetaDataContent

            sbPreProcessLog.AppendLine(Messages.Adding_Certification_Clause_To_PDF)
            doc.Close(True)
            sbPreProcessLog.AppendLine(Messages.Added_Certification_Clause_To_PDF)

            res.PreProcessedFile = _request.FileToPreProcess
            res.PreProcessedFileSize = _request.FileToPreProcess.Length
            res.PreProcessLog = sbPreProcessLog.ToString

            ' Uncomment to save PDF into file
            ' Useful for debugging for PDF/A conformance or other issues
            'File.WriteAllBytes("c:\temp\AfterPreProcess.pdf", Request.FileToPreProcess.ToArray)

            Return res

        Catch ex As Exception
            res.ErrorMessage = ex.Message
            sbPreProcessLog.AppendLine(ex.ToString)
            res.PreProcessLog = sbPreProcessLog.ToString
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
