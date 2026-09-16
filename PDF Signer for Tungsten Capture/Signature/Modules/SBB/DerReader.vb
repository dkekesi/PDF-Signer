Imports System.Linq

''' <summary>Minimal DER reader for the two certificate extensions the module inspects (QC statements, authority information access).</summary>
Friend Module DerReader
    ''' <summary>One DER tag-length-value element: the tag byte and the content bytes.</summary>
    Friend Structure DerElement
        ''' <summary>The element's DER tag byte.</summary>
        Friend Tag As Byte
        ''' <summary>The element's content bytes (the value, without tag or length).</summary>
        Friend Content As Byte()
    End Structure

    ''' <summary>Reads the element starting at Offset and advances Offset past it; False when the bytes end early or the length is unusable.</summary>
    Friend Function TryRead(Data As Byte(), ByRef Offset As Integer, ByRef Element As DerElement) As Boolean
        If Data Is Nothing OrElse Offset < 0 OrElse Offset + 2 > Data.Length Then Return False

        Dim tag As Byte = Data(Offset)
        Dim pos As Integer = Offset + 1
        Dim first As Integer = Data(pos)
        pos += 1

        Dim length As Integer
        If first < &H80 Then
            length = first
        Else
            ' Long form: the low bits give the byte count of the length (1-4 supported).
            Dim count As Integer = first And &H7F
            If count = 0 OrElse count > 4 OrElse pos + count > Data.Length Then Return False
            length = 0
            For i As Integer = 1 To count
                length = (length << 8) Or Data(pos)
                pos += 1
            Next
        End If

        If length < 0 OrElse pos + length > Data.Length Then Return False
        Dim content(length - 1) As Byte
        Array.Copy(Data, pos, content, 0, length)
        Element = New DerElement With {.Tag = tag, .Content = content}
        Offset = pos + length
        Return True
    End Function

    ''' <summary>Child elements of a constructed element's content, stopping at the first malformed element.</summary>
    Friend Function Children(Content As Byte()) As List(Of DerElement)
        Dim res As New List(Of DerElement)
        If Content Is Nothing Then Return res
        Dim offset As Integer = 0
        While offset < Content.Length
            Dim element As DerElement
            If Not TryRead(Content, offset, element) Then Exit While
            res.Add(element)
        End While
        Return res
    End Function

    ''' <summary>True when the element is an OBJECT IDENTIFIER with exactly the given encoded value.</summary>
    Friend Function IsOid(Element As DerElement, EncodedOid As Byte()) As Boolean
        Return Element.Tag = &H6 AndAlso Element.Content IsNot Nothing AndAlso Element.Content.SequenceEqual(EncodedOid)
    End Function
End Module
