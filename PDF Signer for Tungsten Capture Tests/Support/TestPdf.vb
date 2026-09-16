Imports System.Text

''' <summary>Builds a minimal, unsigned, one-page PDF in memory for SecureBlackbox smoke tests.</summary>
Friend Module TestPdf
    Friend Function Minimal() As Byte()
        Dim objects As String() = {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] >>"}
        Dim sb As New StringBuilder("%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Integer)
        For i As Integer = 0 To objects.Length - 1
            offsets.Add(sb.Length)
            sb.Append($"{i + 1} 0 obj" & vbLf & objects(i) & vbLf & "endobj" & vbLf)
        Next
        Dim xref As Integer = sb.Length
        sb.Append("xref" & vbLf & $"0 {objects.Length + 1}" & vbLf & "0000000000 65535 f " & vbLf)
        For Each o As Integer In offsets
            sb.Append(o.ToString("D10") & " 00000 n " & vbLf)
        Next
        sb.Append("trailer" & vbLf & $"<< /Size {objects.Length + 1} /Root 1 0 R >>" & vbLf & "startxref" & vbLf & xref & vbLf & "%%EOF" & vbLf)
        Return Encoding.ASCII.GetBytes(sb.ToString())
    End Function
End Module
