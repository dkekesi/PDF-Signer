Public Class FilterValueItem
    Public Property DisplayName As String
    Public Property Value As Object

    Public Overrides Function ToString() As String
        Return DisplayName
    End Function
End Class
