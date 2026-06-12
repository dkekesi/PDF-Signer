Imports System.Threading
Imports System.IO
Imports NLog
Imports PDFSigner.My.Resources

Friend Class FileOperation
    Friend Function ReadAllBytesWhenAvailable(FileNameWithPath As String) As Byte()
        Const maxRetryCount As Integer = 15
        Dim retryCount As Integer = 0
        Dim errMsg As String = Messages.File_Read_Error
        Dim exMsg As String = String.Empty

        While retryCount < maxRetryCount
            Try
                Using stream As FileStream = File.OpenRead(FileNameWithPath)
                    Dim res As Byte() = New Byte(stream.Length - 1) {}
                    stream.Read(res, 0, stream.Length)
                    Return res
                End Using

            Catch Ex As IOException
                retryCount += 1
                exMsg = Ex.Message
                '_logger.Debug(errMsg, FileNameWithPath, retryCount, Ex.Message)
            End Try

            Thread.Sleep(1000 + retryCount * 100)
        End While

        Throw New IOException(String.Format(errMsg, FileNameWithPath, maxRetryCount, exMsg))
    End Function

    Friend Function ReadFileStreamWhenAvailable(FileNameWithPath As String) As Stream
        Const maxRetryCount As Integer = 15
        Dim retryCount As Integer = 0
        Dim errMsg As String = Messages.File_Read_Error
        Dim exMsg As String = String.Empty

        While retryCount < maxRetryCount
            Try
                Dim strm As FileStream = File.OpenRead(FileNameWithPath)
                Return strm

            Catch Ex As IOException
                retryCount += 1
                exMsg = Ex.Message
                '_logger.Debug(errMsg, FileNameWithPath, retryCount, Ex.Message)
            End Try

            Thread.Sleep(1000 + retryCount * 100)
        End While

        Throw New IOException(String.Format(errMsg, FileNameWithPath, maxRetryCount, exMsg))
    End Function

    Friend Sub WriteAllBytesWhenAvailable(FileNameWithPath As String, Content As Byte())
        Const maxRetryCount As Integer = 15
        Dim retryCount As Integer = 0
        Dim errMsg As String = Messages.File_Write_Error
        Dim exMsg As String = String.Empty

        While retryCount < maxRetryCount
            Try
                File.WriteAllBytes(FileNameWithPath, Content)
                Return
            Catch Ex As IOException
                retryCount += 1
                exMsg = Ex.Message
                '_logger.Debug(errMsg, FileNameWithPath, retryCount, Ex.Message)
            End Try

            Thread.Sleep(1000 + retryCount * 100)
        End While

        Throw New IOException(String.Format(errMsg, FileNameWithPath, maxRetryCount, exMsg))
    End Sub

    Friend Sub MoveWhenAvailable(SourceFileName As String, DestinationFileName As String)
        Const maxRetryCount As Integer = 15
        Dim retryCount As Integer = 0
        Dim errMsg As String = Messages.File_Move_Error
        Dim exMsg As String = String.Empty

        While retryCount < maxRetryCount
            Try
                File.Move(SourceFileName, DestinationFileName)
                Return
            Catch Ex As IOException
                retryCount += 1
                exMsg = Ex.Message
                '_logger.Debug(errMsg, SourceFileName, DestinationFileName, maxRetryCount, Ex.Message)
            End Try

            Thread.Sleep(1000 + retryCount * 100)
        End While

        Throw New IOException(String.Format(errMsg, SourceFileName, DestinationFileName, maxRetryCount, exMsg))
    End Sub

    Friend Sub DeleteWhenAvailable(FileNameWithPath As String)
        Const maxRetryCount As Integer = 15
        Dim retryCount As Integer = 0
        Dim errMsg As String = Messages.File_Delete_Error
        Dim exMsg As String = String.Empty

        While retryCount < maxRetryCount
            Try
                If File.Exists(FileNameWithPath) Then File.Delete(FileNameWithPath)
                Return
            Catch Ex As IOException
                retryCount += 1
                exMsg = Ex.Message
                '_logger.Debug(errMsg, FileNameWithPath, retryCount, Ex.Message)
            End Try

            Thread.Sleep(1000 + retryCount * 100)
        End While

        Throw New IOException(String.Format(errMsg, FileNameWithPath, maxRetryCount, exMsg))
    End Sub

    Friend Function GetTempFile(FileGUIDPrefix As String) As String
        Const TempFolderName As String = "PDF Signer"
        Dim TempFolder = Path.Combine(Path.GetTempPath, TempFolderName)

        If Not Directory.Exists(TempFolder) Then
            Directory.CreateDirectory(TempFolder)
        End If

        Dim tempFileName As String = Path.Combine(TempFolder, FileGUIDPrefix + Guid.NewGuid().ToString)

        Return tempFileName
    End Function
End Class
