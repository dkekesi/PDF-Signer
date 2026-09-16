Imports System.IO

''' <summary>Chooses the backing store of the signing module's working copy and pass outputs: memory, or a self-deleting temp file for large documents.</summary>
Friend Module SigningBufferFactory
    Private Const TempFileBufferSize As Integer = 81920

    ''' <summary>True when Source is file-backed or longer than <see cref="Constant.FileStreamSizeLimit"/>, so its copies belong on disk.</summary>
    Friend Function IsLargeDocument(Source As Stream) As Boolean
        If TypeOf Source Is FileStream Then Return True
        Return Source.CanSeek AndAlso Source.Length > Constant.FileStreamSizeLimit
    End Function

    ''' <summary>An empty read/write buffer: a temp FileStream deleted when disposed for a large document, otherwise a MemoryTributary.</summary>
    Friend Function Create(LargeDocument As Boolean) As Stream
        If Not LargeDocument Then Return New MemoryTributary
        Dim path As String = IO.Path.Combine(IO.Path.GetTempPath(), $"pdfsigner-sbb-{Guid.NewGuid():N}.tmp")
        Return New FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, TempFileBufferSize, FileOptions.DeleteOnClose)
    End Function
End Module
