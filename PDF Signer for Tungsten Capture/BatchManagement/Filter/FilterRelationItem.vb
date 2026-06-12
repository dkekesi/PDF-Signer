Friend Class FilterRelationItem
    Public Property DisplayName As String
    Public Property Value As String

    Public Overrides Function ToString() As String
        Return DisplayName
    End Function
End Class
