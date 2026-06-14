Imports System.IO

''' <summary>
''' A memory-backed stream that stores data in multiple fixed-size blocks instead of one
''' contiguous array, so it scales to large documents without a single large-object-heap
''' allocation. Ported from the former PDFStreamer.WCFCommon.Helper.MemoryTributary, which
''' was removed when PDF Streamer dropped its net48 WCFCommon assembly.
''' </summary>
Public Class MemoryTributary
    Inherits Stream

    Public Sub New()
        Position = 0
    End Sub

    Public Sub New(source As Byte())
        Me.Write(source, 0, source.Length)
        Position = 0
    End Sub

    ' length is ignored because capacity has no meaning unless we impose an artificial limit
    Public Sub New(length As Integer)
        SetLength(length)
        Position = length
        Dim d As Byte() = blck ' access block to prompt allocation
        Position = 0
    End Sub

    Public Overrides ReadOnly Property CanRead As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property Length As Long
        Get
            Return lngt
        End Get
    End Property

    Public Overrides Property Position As Long

    Protected lngt As Long = 0
    Protected blockSize As Long = 1048576
    Protected blcks As New List(Of Byte())

    ''' <summary>The block of memory currently addressed by Position.</summary>
    Protected ReadOnly Property blck As Byte()
        Get
            While blcks.Count <= blockId
                blcks.Add(New Byte(CInt(blockSize) - 1) {})
            End While
            Return blcks(CInt(blockId))
        End Get
    End Property

    ''' <summary>The id of the block currently addressed by Position.</summary>
    Protected ReadOnly Property blockId As Long
        Get
            Return Position \ blockSize
        End Get
    End Property

    ''' <summary>The offset of the byte currently addressed by Position, into its block.</summary>
    Protected ReadOnly Property blockOffset As Long
        Get
            Return Position Mod blockSize
        End Get
    End Property

    Public Overrides Sub Flush()
    End Sub

    Public Overrides Function Read(buff As Byte(), offset As Integer, count As Integer) As Integer
        Dim lcount As Long = CLng(count)

        If lcount < 0 Then
            Throw New ArgumentOutOfRangeException("count", lcount, "Number of bytes to copy cannot be negative.")
        End If

        Dim remaining As Long = (lngt - Position)
        If lcount > remaining Then lcount = remaining

        If buff Is Nothing Then
            Throw New ArgumentNullException("buffer", "Buffer cannot be null.")
        End If
        If offset < 0 Then
            Throw New ArgumentOutOfRangeException("offset", offset, "Destination offset cannot be negative.")
        End If

        ' guard against EOF so CopyTo terminates and no empty trailing block is allocated
        If lcount <= 0 Then Return 0

        Dim rd As Integer = 0
        Do
            Dim copysize As Long = Math.Min(lcount, (blockSize - blockOffset))
            Buffer.BlockCopy(blck, CInt(blockOffset), buff, offset, CInt(copysize))
            lcount -= copysize
            offset += CInt(copysize)
            rd += CInt(copysize)
            Position += copysize
        Loop While lcount > 0

        Return rd
    End Function

    Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
        Select Case origin
            Case SeekOrigin.Begin
                Position = offset
            Case SeekOrigin.Current
                Position += offset
            Case SeekOrigin.End
                Position = Length - offset
        End Select
        Return Position
    End Function

    Public Overrides Sub SetLength(value As Long)
        lngt = value
    End Sub

    Public Overrides Sub Write(buff As Byte(), offset As Integer, count As Integer)
        Dim initialPosition As Long = Position
        Dim copysize As Integer
        Try
            Do
                copysize = Math.Min(count, CInt(blockSize - blockOffset))
                EnsureCapacity(Position + copysize)
                Buffer.BlockCopy(buff, offset, blck, CInt(blockOffset), copysize)
                count -= copysize
                offset += copysize
                Position += copysize
            Loop While count > 0
        Catch e As Exception
            Position = initialPosition
            Throw
        End Try
    End Sub

    Public Overrides Function ReadByte() As Integer
        If Position >= lngt Then Return -1
        Dim b As Byte = blck(CInt(blockOffset))
        Position += 1
        Return b
    End Function

    Public Overrides Sub WriteByte(value As Byte)
        EnsureCapacity(Position + 1)
        blck(CInt(blockOffset)) = value
        Position += 1
    End Sub

    Protected Sub EnsureCapacity(intended_length As Long)
        If intended_length > lngt Then lngt = intended_length
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        ' no unmanaged resources
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>Returns the entire content as a byte array (may fail for very large streams).</summary>
    Public Function ToArray() As Byte()
        Dim firstposition As Long = Position
        Position = 0
        Dim destination As Byte() = New Byte(CInt(Length) - 1) {}
        Read(destination, 0, CInt(Length))
        Position = firstposition
        Return destination
    End Function

    ''' <summary>Reads length bytes from source into this instance at the current position.</summary>
    Public Sub ReadFrom(source As Stream, length As Long)
        Dim buffer As Byte() = New Byte(4095) {}
        Dim read As Integer
        Do
            read = source.Read(buffer, 0, CInt(Math.Min(4096, length)))
            length -= read
            Me.Write(buffer, 0, read)
        Loop While length > 0
    End Sub

    ''' <summary>Writes the entire stream into destination; Position is preserved.</summary>
    Public Sub WriteTo(destination As Stream)
        Dim initialpos As Long = Position
        Position = 0
        Me.CopyTo(destination)
        Position = initialpos
    End Sub
End Class
