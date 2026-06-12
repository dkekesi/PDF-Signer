Imports PDFSigner.My.Resources

Friend Module FilterValue
    Friend Const Ready As Integer = 2
    Friend Const Suspended As Integer = 8

    Friend Const TrueValue As Integer = 1
    Friend Const FalseValue As Integer = 0

    Friend Function GetBatchStatusList() As List(Of FilterValueItem)
        Dim res As New List(Of FilterValueItem)

        Dim fvi As FilterValueItem

        fvi = New FilterValueItem With {.DisplayName = GUIText.Batch_State_Ready, .Value = Ready}
        res.Add(fvi)

        fvi = New FilterValueItem With {.DisplayName = GUIText.Batch_State_Suspended, .Value = Suspended}
        res.Add(fvi)

        Return res
    End Function

    Friend Function GetBooleanList() As List(Of FilterValueItem)
        Dim res As New List(Of FilterValueItem)

        Dim fvi As FilterValueItem

        fvi = New FilterValueItem With {.DisplayName = GUIText.Filter_True, .Value = TrueValue}
        res.Add(fvi)

        fvi = New FilterValueItem With {.DisplayName = GUIText.Filter_False, .Value = FalseValue}
        res.Add(fvi)

        Return res
    End Function
End Module