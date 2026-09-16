Imports PDFSigner

''' <summary>Verifies the minimal DER tag-length-value reader used by the certificate-extension parsers.</summary>
<TestClass>
Public Class DerReaderTests
    <TestMethod>
    Public Sub Reads_a_short_form_element()
        Dim data As Byte() = {&H6, &H3, &H2A, &H3, &H4}
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsTrue(DerReader.TryRead(data, offset, e))
        Assert.AreEqual(CByte(&H6), e.Tag)
        CollectionAssert.AreEqual(New Byte() {&H2A, &H3, &H4}, e.Content)
        Assert.AreEqual(5, offset)
    End Sub

    <TestMethod>
    Public Sub Reads_a_long_form_length()
        Dim content(199) As Byte
        Dim data As New List(Of Byte) From {&H4, &H81, &HC8}
        data.AddRange(content)
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsTrue(DerReader.TryRead(data.ToArray(), offset, e))
        Assert.AreEqual(200, e.Content.Length)
        Assert.AreEqual(203, offset)
    End Sub

    <TestMethod>
    Public Sub Truncated_input_is_rejected()
        Dim offset As Integer = 0
        Dim e As DerReader.DerElement
        Assert.IsFalse(DerReader.TryRead(New Byte() {&H30, &H5, &H1}, offset, e))
        Assert.IsFalse(DerReader.TryRead(New Byte() {&H30}, offset, e))
        Assert.IsFalse(DerReader.TryRead(Nothing, offset, e))
    End Sub

    <TestMethod>
    Public Sub Children_splits_a_sequence()
        Dim seq As Byte() = {&H2, &H1, &H5, &H6, &H2, &H2A, &H3}
        Dim children = DerReader.Children(seq)
        Assert.AreEqual(2, children.Count)
        Assert.AreEqual(CByte(&H2), children(0).Tag)
        Assert.IsTrue(DerReader.IsOid(children(1), New Byte() {&H2A, &H3}))
    End Sub
End Class
