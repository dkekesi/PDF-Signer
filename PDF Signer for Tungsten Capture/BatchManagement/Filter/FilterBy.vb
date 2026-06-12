Imports PDFSigner.My.Resources

Friend Module FilterBy
    Friend Const BatchName As String = "Batch name"
    Friend Const BatchClass As String = "Batch class"
    Friend Const Status As String = "Status"
    Friend Const Priority As String = "Priority"
    Friend Const HasError As String = "Has error"
    Friend Const StationID As String = "Station ID"
    Friend Const BatchCreationDateTime As String = "Batch creation date/time"
    Friend Const BatchField As String = "Batch field"
    Friend Const ScanStationID As String = "Scan station ID"
    Friend Const ScanUser As String = "Scan user"

    Friend Function GetFilterByList() As List(Of FilterByItem)
        Dim res As New List(Of FilterByItem)
        Dim fbi As FilterByItem

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Batch_Class, .Value = BatchClass}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Batch_Creation_Date_Time, .Value = BatchCreationDateTime}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Batch_Field, .Value = BatchField}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Batch_Name, .Value = BatchName}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Has_Error, .Value = HasError}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Priority, .Value = Priority}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Scan_Station_ID, .Value = ScanStationID}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Scan_User, .Value = ScanUser}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Station_ID, .Value = StationID}
        res.Add(fbi)

        fbi = New FilterByItem With {.DisplayName = GUIText.Filter_Status, .Value = Status}
        res.Add(fbi)

        Return res
    End Function
End Module
