Imports System.Linq
Imports Kofax.Capture.DBLite
Imports PDFSigner.My.Resources
Imports PDFSignerCommon
Imports System.ComponentModel
Imports DevExpress.XtraEditors.Repository
Imports DevExpress.Utils
Imports NLog
Imports Kofax.Capture.SDK.CustomModule

Friend Class FrmOpenBatch
    Private _BatchColl As BatchCollection
    Private _SelectedBatch As Batch
    Private ReadOnly _FormCSHIDString As String = "OPENBATCH"
    Private _BatchFilterList As BindingList(Of BatchFilter)
    Private _CurrentBatchFilter As New BatchFilter
    Private _IsLoaded As Boolean
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Friend Property KfxRunTimeSession As IRuntimeSession
    Friend Property KfxProcessID As Integer
    Friend ReadOnly Property Batch() As IBatch
        Get
            Try
                Return _SelectedBatch
            Finally
                If _BatchColl IsNot Nothing Then
                    _BatchColl = Nothing
                End If
            End Try
        End Get
    End Property

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' Indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub FrmBatchSelect_Load(sender As Object, e As EventArgs) Handles Me.Load
        Hide()

        Dim lm As New LayoutManager
        BarManager1.ForceInitialize()
        lm.LoadWorkspace(WsManager, Me.Name)

        Show()

        SetupBatchFilter()
        _BatchFilterList = PDFSignerCurrentUserRegistry.LoadBatchFilterList()
        GridFilter.DataSource = _BatchFilterList

        _CurrentBatchFilter = LoadBatchFilter(PDFSignerCurrentUserRegistry.CurrentBatchFilterEntryName)
        RefreshBatchList()
        _IsLoaded = True
    End Sub

    Private Sub FrmBatchSelect_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Dim lm As New LayoutManager
        lm.SaveWorkspace(WsManager, Me.Name)
    End Sub

    Private Sub RefreshBatchList()
        Dim batchList As New List(Of BatchListItem)

        ' Set and save the filter
        Dim errMsg As String = _CurrentBatchFilter.Validate
        If Not String.IsNullOrEmpty(errMsg) Then
            MsgBox(errMsg, MsgBoxStyle.Exclamation, My.Resources.MessageBoxTitle)
            Return
        End If

        Dim filterSet As Boolean = KfxRunTimeSession.SetBatchFilter(_CurrentBatchFilter.ToXml)
        PDFSignerCurrentUserRegistry.SaveBatchFilterXml(_CurrentBatchFilter.FilterXml, PDFSignerCurrentUserRegistry.CurrentBatchFilterEntryName)

        If _CurrentBatchFilter IsNot Nothing AndAlso _CurrentBatchFilter.Enabled Then
            _BatchColl = KfxRunTimeSession.BatchCollectionFiltered(_CurrentBatchFilter.Enabled)
        Else
            _BatchColl = KfxRunTimeSession.BatchCollection(KfxDbFilter.KfxDbFilterOnProcess Or KfxDbFilter.KfxDbFilterOnStates Or KfxDbFilter.KfxDbSortOnPriorityDescending, KfxProcessID, KfxDbState.KfxDbBatchReady Or KfxDbState.KfxDbBatchSuspended, True)
        End If

        For Each b As Batch In _BatchColl
            Dim bli As New BatchListItem With {
                .Name = b.Name,
                .BatchClass = b.BatchClassName,
                .BatchDateTime = b.CreateDate,
                .Status = GetStatusString(b.State),
                .DocumentCount = b.DocumentCount,
                .PageCount = b.PageCount,
                .Priority = b.Priority
                }
            batchList.Add(bli)

            b = Nothing
        Next

        If _CurrentBatchFilter IsNot Nothing AndAlso _CurrentBatchFilter.Enabled Then
            Text = String.Format(GUIText.Header_Batch_Open_Batch_Count, batchList.Count) + GUIText.Filter_Enabled_Window_Header
        Else
            Text = String.Format(GUIText.Header_Batch_Open_Batch_Count, batchList.Count)
        End If

        GridBatch.DataSource = batchList

        If batchList.Count > 0 Then
            GridBatch.Select()
        End If
    End Sub

    Private Sub BtnCancel_Click(eventSender As Object, eventArgs As EventArgs) Handles BtnCancel.Click
        Hide()
    End Sub

    Private Sub BtnOK_Click(eventSender As Object, eventArgs As EventArgs) Handles BtnOK.Click
        OKClicked()
    End Sub

    Private Function GetStatusString(lState As Integer) As String
        Dim strState As String

        Select Case lState
            Case KfxDbState.KfxDbBatchReady
                strState = GUIText.Batch_State_Ready
            Case KfxDbState.KfxDbBatchInProgress
                strState = GUIText.Batch_State_In_Progress
            Case KfxDbState.KfxDbBatchSuspended
                strState = GUIText.Batch_State_Suspended
            Case KfxDbState.KfxDbBatchError
                strState = GUIText.Batch_State_Error
            Case KfxDbState.KfxDbBatchCompleted
                strState = GUIText.Batch_State_Completed
            Case KfxDbState.KfxDbBatchReserved
                strState = GUIText.Batch_State_Reserved
            Case Else
                Debug.Assert(False, String.Empty)
                strState = GUIText.Batch_State_Unknown
        End Select

        Return strState
    End Function

    Private Sub OKClicked()
        If GridViewBatch.SelectedRowsCount > 0 Then
            Dim bi As BatchListItem = CType(GridViewBatch.GetRow(GridViewBatch.FocusedRowHandle), BatchListItem)
            _SelectedBatch = _BatchColl(bi.Name)
            Hide()
        End If
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles BtnRefresh.Click
        RefreshBatchList()
    End Sub

    Private Sub GridViewBatch_DoubleClick(sender As Object, e As MouseEventArgs) Handles GridViewBatch.DoubleClick
        Dim ea As DXMouseEventArgs = TryCast(e, DXMouseEventArgs)
        Dim info As DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo = GridViewBatch.CalcHitInfo(ea.Location)
        If info.InDataRow And info.Column IsNot Nothing Then
            OKClicked()
        End If
    End Sub

    Private Sub GridViewBatch_KeyDown(sender As Object, e As KeyEventArgs) Handles GridViewBatch.KeyDown
        If e.KeyCode = Keys.Enter Then
            OKClicked()
        End If
    End Sub

    Private Sub SetupBatchFilter()
        LookUpEditFilterBy.DataSource = FilterBy.GetFilterByList
        LookUpEditFilterRelation.DataSource = FilterRelation.GetFilterRelationList
        LookUpEditStatus.DataSource = FilterValue.GetBatchStatusList
        LookUpEditValueBoolean.DataSource = FilterValue.GetBooleanList
    End Sub

    Private Function LoadBatchFilter(Name As String) As BatchFilter
        Try
            _CurrentBatchFilter.FilterXml = PDFSignerCurrentUserRegistry.LoadBatchFilterXml(Name)
            _CurrentBatchFilter.LoadFromXml()

            GridCondition.DataSource = _CurrentBatchFilter.Items
            TBConditionsEnabled.Checked = _CurrentBatchFilter.Enabled
            Return _CurrentBatchFilter

        Catch ex As Exception
            Dim errMsg As String = String.Format(Messages.Filter_Load_Error, Name, ex.ToString)
            _logger.Error(errMsg)
            MsgBox(errMsg, MsgBoxStyle.Critical, My.Resources.MessageBoxTitle)
            Return Nothing
        End Try
    End Function

    Private Sub TBConditionsEnabled_CheckedChanged(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBConditionsEnabled.CheckedChanged
        _CurrentBatchFilter.Enabled = TBConditionsEnabled.Checked
        RefreshBatchList()
    End Sub

    Private Sub TBConditionAdd_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBConditionAdd.ItemClick
        _CurrentBatchFilter.Items.Add(New BatchFilterConditionItem With {.ConditionFilterBy = FilterBy.BatchName, .ConditionRelation = FilterRelation.Contains, .ConditionValue = String.Empty})
    End Sub

    Private Sub TBConditionDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBConditionDelete.ItemClick
        Dim batchFilterItem = CType(GridViewCondition.GetRow(GridViewCondition.FocusedRowHandle), BatchFilterConditionItem)
        _CurrentBatchFilter.Items.Remove(batchFilterItem)
        RefreshBatchList()
    End Sub

    Private Sub TBConditionDeleteAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBConditionDeleteAll.ItemClick
        _CurrentBatchFilter.Items.Clear()
        RefreshBatchList()
    End Sub

    Private Sub TBConditionSave_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBConditionSave.ItemClick
        RefreshBatchList()

        Dim errMsg As String = _CurrentBatchFilter.Validate
        If Not String.IsNullOrEmpty(errMsg) Then
            MsgBox(errMsg, MsgBoxStyle.Exclamation, My.Resources.MessageBoxTitle)
            Return
        End If

        Dim frm As New FrmSaveFilter
        Dim selectedFilter As BatchFilter = CType(GridViewFilters.GetFocusedRow, BatchFilter)

        frm.SavedFilterList = _BatchFilterList
        If selectedFilter IsNot Nothing Then
            frm.FilterName = selectedFilter.Name
        End If

        frm.ShowDialog()

        If frm.DialogResult = DialogResult.OK Then
            PDFSignerCurrentUserRegistry.SaveBatchFilterXml(_CurrentBatchFilter.FilterXml, frm.FilterName)
            If Not _BatchFilterList.Any(Function(f) f.Name = frm.FilterName) Then
                _BatchFilterList.Add(New BatchFilter With {.Name = frm.FilterName})
            End If
        End If
    End Sub

    Private Sub GridViewCondition_CustomRowCellEdit(sender As Object, e As DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs) Handles GridViewCondition.CustomRowCellEdit
        Dim bfi As BatchFilterConditionItem = CType(GridViewCondition.GetRow(e.RowHandle), BatchFilterConditionItem)

        If e.Column Is GridConditionValue Then
            Select Case bfi.ConditionFilterBy
                Case FilterBy.Priority
                    e.RepositoryItem = NumericSpinEdit
                Case FilterBy.BatchCreationDateTime
                    e.RepositoryItem = DateTimeEdit
                Case FilterBy.Status
                    e.RepositoryItem = LookUpEditStatus
                Case FilterBy.HasError
                    e.RepositoryItem = LookUpEditValueBoolean
            End Select
        End If
    End Sub

    Private Sub GridViewCondition_CellValueChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs) Handles GridViewCondition.CellValueChanged
        If Not _IsLoaded Then Return
        If e.Column IsNot GridConditionName Then Return

        Dim bfi As BatchFilterConditionItem = CType(GridViewCondition.GetRow(e.RowHandle), BatchFilterConditionItem)
        bfi.ConditionBatchField = String.Empty

        Select Case e.Value.ToString
            Case FilterBy.BatchClass
                bfi.ConditionRelation = FilterRelation.Contains
                bfi.ConditionValue = String.Empty
            Case FilterBy.BatchCreationDateTime
                bfi.ConditionRelation = FilterRelation.LessThan
                bfi.ConditionValue = DateTime.Now.ToString(Constant.BatchFilterDateFormat)
            Case FilterBy.BatchField
                bfi.ConditionRelation = FilterRelation.Contains
                bfi.ConditionValue = String.Empty
            Case FilterBy.BatchName
                bfi.ConditionRelation = FilterRelation.StartsWith
                bfi.ConditionValue = String.Empty
            Case FilterBy.HasError
                bfi.ConditionRelation = FilterRelation.EqualTo
                bfi.ConditionValue = FilterValue.FalseValue
            Case FilterBy.Priority
                bfi.ConditionRelation = FilterRelation.EqualTo
                bfi.ConditionValue = 5
            Case FilterBy.ScanStationID, FilterBy.ScanUser, FilterBy.StationID
                bfi.ConditionRelation = FilterRelation.EqualTo
                bfi.ConditionValue = String.Empty
            Case FilterBy.Status
                bfi.ConditionRelation = FilterRelation.EqualTo
                bfi.ConditionValue = FilterValue.Ready
        End Select
    End Sub

    Private Sub GridViewCondition_CustomRowCellEditForEditing(sender As Object, e As DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs) Handles GridViewCondition.CustomRowCellEditForEditing
        If e.Column IsNot GridConditionRelation Then Return

        Dim bfi As BatchFilterConditionItem = CType(GridViewCondition.GetRow(e.RowHandle), BatchFilterConditionItem)
        Dim editor As RepositoryItemLookUpEdit = CType(GridConditionRelation.ColumnEdit, RepositoryItemLookUpEdit)
        Dim oldDataSource As List(Of FilterRelationItem) = CType(editor.DataSource, List(Of FilterRelationItem))
        Dim ds As New List(Of FilterRelationItem)(oldDataSource)

        For i As Integer = ds.Count - 1 To 0 Step -1
            If Not FilterRelation.GetSupportedFilterRelations(bfi.ConditionFilterBy).Contains(ds(i).Value) Then
                ds.RemoveAt(i)
            End If
        Next

        e.RepositoryItem = LookUpEditFilterRelationTemporary
        LookUpEditFilterRelationTemporary.DataSource = ds
        LookUpEditFilterRelationTemporary.DropDownRows = ds.Count
    End Sub

    Private Sub TBFiltersDelete_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBFilterDelete.ItemClick
        Dim batchFilter As BatchFilter = CType(GridViewFilters.GetRow(GridViewFilters.FocusedRowHandle), BatchFilter)
        If batchFilter Is Nothing Then Return

        If MsgBox(Messages.Ask_Delete_Saved_Filter, MsgBoxStyle.Question + MsgBoxStyle.YesNo, My.Resources.MessageBoxTitle) = MsgBoxResult.No Then
            Return
        End If

        PDFSignerCurrentUserRegistry.DeleteBatchFilter(batchFilter.Name)
        _BatchFilterList.Remove(batchFilter)
    End Sub

    Private Sub TBFiltersDeleteAll_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles TBFilterDeleteAll.ItemClick
        If MsgBox(Messages.Ask_Delete_All_Saved_Filters, MsgBoxStyle.Question + MsgBoxStyle.YesNo, My.Resources.MessageBoxTitle) = MsgBoxResult.No Then
            Return
        End If

        PDFSignerCurrentUserRegistry.DeleteAllBatchFilters()
        _BatchFilterList.Clear()
    End Sub

    Private Sub GridViewFilters_FocusedRowChanged(sender As Object, e As DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs) Handles GridViewFilters.FocusedRowChanged
        If Not _IsLoaded Then Return

        Dim selectedFilter As BatchFilter = CType(GridViewFilters.GetFocusedRow, BatchFilter)

        If selectedFilter Is Nothing Then Return

        _CurrentBatchFilter = LoadBatchFilter(selectedFilter.Name)
        RefreshBatchList()
    End Sub

    Private Sub GridViewFilters_RowClick(sender As Object, e As DevExpress.XtraGrid.Views.Grid.RowClickEventArgs) Handles GridViewFilters.RowClick
        Dim selectedFilter As BatchFilter = CType(GridViewFilters.GetRow(e.RowHandle), BatchFilter)

        If selectedFilter Is Nothing Then Return

        _CurrentBatchFilter = LoadBatchFilter(selectedFilter.Name)
        RefreshBatchList()
    End Sub
End Class
