Friend Class BatchFilterConditionItem
    Public Property ConditionFilterBy As String
    Public Property ConditionBatchField As String
    Public Property ConditionRelation As String
    Public Property ConditionValue As Object

    Public Sub New()
        ConditionValue = New Object
    End Sub
End Class
