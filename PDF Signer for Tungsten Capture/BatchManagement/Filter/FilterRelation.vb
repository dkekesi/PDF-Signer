Imports PDFSigner.My.Resources

Friend Module FilterRelation
    Friend Const Contains As String = "Contains"
    Friend Const EqualTo As String = "Equal to"
    Friend Const NotEqualTo As String = "Not equal to"
    Friend Const GreaterThan As String = "Greater than"
    Friend Const LessThan As String = "Less than"
    Friend Const StartsWith As String = "Starts with"

    Friend Function GetFilterRelationList() As List(Of FilterRelationItem)
        Dim res As New List(Of FilterRelationItem)
        Dim fri As FilterRelationItem

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Contains, .Value = Contains}
        res.Add(fri)

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Starts_With, .Value = StartsWith}
        res.Add(fri)

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Equal_To, .Value = EqualTo}
        res.Add(fri)

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Not_Equal_To, .Value = NotEqualTo}
        res.Add(fri)

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Greater_Than, .Value = GreaterThan}
        res.Add(fri)

        fri = New FilterRelationItem With {.DisplayName = GUIText.Filter_Less_Than, .Value = LessThan}
        res.Add(fri)

        Return res
    End Function

    Friend Function GetSupportedFilterRelations(FilterByString As String) As List(Of String)
        Dim res As New List(Of String)

        Select Case FilterByString
            Case FilterBy.BatchClass
                res.Add(EqualTo)
            Case FilterBy.BatchCreationDateTime
                res.Add(GreaterThan)
                res.Add(LessThan)
            Case FilterBy.BatchField
                res.Add(Contains)
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
                res.Add(StartsWith)
            Case FilterBy.BatchName
                res.Add(Contains)
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
                res.Add(StartsWith)
            Case FilterBy.HasError
                res.Add(EqualTo)
            Case FilterBy.Priority
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
            Case FilterBy.ScanStationID
                res.Add(Contains)
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
                res.Add(StartsWith)
            Case FilterBy.ScanUser
                res.Add(Contains)
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
                res.Add(StartsWith)
            Case FilterBy.StationID
                res.Add(Contains)
                res.Add(EqualTo)
                res.Add(NotEqualTo)
                res.Add(GreaterThan)
                res.Add(LessThan)
                res.Add(StartsWith)
            Case FilterBy.Status
                res.Add(EqualTo)
        End Select

        Return res
    End Function
End Module
