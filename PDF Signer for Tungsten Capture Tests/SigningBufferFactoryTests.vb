Imports System.IO
Imports PDFSigner

''' <summary>Verifies SigningBufferFactory's memory/temp-file choice and the self-deleting temp buffer.</summary>
<TestClass>
Public Class SigningBufferFactoryTests
    ''' <summary>Seekable stream that only reports a length, for the size threshold.</summary>
    Private NotInheritable Class LengthOnlyStream
        Inherits MemoryStream
        Private ReadOnly _length As Long

        Public Sub New(length As Long)
            _length = length
        End Sub

        Public Overrides ReadOnly Property Length As Long
            Get
                Return _length
            End Get
        End Property
    End Class

    <TestMethod>
    Public Sub Small_memory_input_uses_memory_and_large_or_file_input_uses_disk()
        Assert.IsFalse(SigningBufferFactory.IsLargeDocument(New MemoryStream(New Byte(9) {})))
        Assert.IsFalse(SigningBufferFactory.IsLargeDocument(New LengthOnlyStream(Constant.FileStreamSizeLimit)))
        Assert.IsTrue(SigningBufferFactory.IsLargeDocument(New LengthOnlyStream(Constant.FileStreamSizeLimit + 1L)))
        Using file = SigningBufferFactory.Create(True)
            Assert.IsTrue(SigningBufferFactory.IsLargeDocument(file))
        End Using
    End Sub

    <TestMethod>
    Public Sub Memory_buffer_is_a_MemoryTributary()
        Using buffer = SigningBufferFactory.Create(False)
            Assert.IsInstanceOfType(buffer, GetType(MemoryTributary))
        End Using
    End Sub

    <TestMethod>
    Public Sub Disk_buffer_is_a_read_write_temp_file_deleted_on_dispose()
        Dim path As String
        Using buffer = SigningBufferFactory.Create(True)
            Dim file = DirectCast(buffer, FileStream)
            path = file.Name
            Assert.AreEqual(IO.Path.GetFullPath(IO.Path.GetTempPath()), IO.Path.GetDirectoryName(path) & IO.Path.DirectorySeparatorChar)
            Assert.IsTrue(buffer.CanRead AndAlso buffer.CanWrite AndAlso buffer.CanSeek)
            buffer.Write(New Byte() {1, 2, 3}, 0, 3)
            buffer.Seek(0, SeekOrigin.Begin)
            Assert.AreEqual(2, New BinaryReader(buffer).ReadBytes(3)(1))
            Assert.IsTrue(IO.File.Exists(path))
        End Using
        Assert.IsFalse(IO.File.Exists(path))
    End Sub
End Class
