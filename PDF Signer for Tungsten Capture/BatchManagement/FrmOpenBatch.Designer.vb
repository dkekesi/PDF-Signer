<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmOpenBatch
    Inherits DevExpress.XtraEditors.XtraForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmOpenBatch))
        Dim PushTransition1 As DevExpress.Utils.Animation.PushTransition = New DevExpress.Utils.Animation.PushTransition()
        Me.SplitContainer = New DevExpress.XtraEditors.SplitContainerControl()
        Me.GridBatch = New DevExpress.XtraGrid.GridControl()
        Me.GridViewBatch = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.GridBatchName = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchClass = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchDateTime = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchStatus = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchDocCount = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchPageCount = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridBatchPriority = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.SplitContainerFilters = New DevExpress.XtraEditors.SplitContainerControl()
        Me.GridFilter = New DevExpress.XtraGrid.GridControl()
        Me.GridViewFilters = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.GridFilterName = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.StandaloneBarDockControl1 = New DevExpress.XtraBars.StandaloneBarDockControl()
        Me.BarManager1 = New DevExpress.XtraBars.BarManager(Me.components)
        Me.ToolBarCondition = New DevExpress.XtraBars.Bar()
        Me.TBConditionsText = New DevExpress.XtraBars.BarStaticItem()
        Me.TBConditionsEnabled = New DevExpress.XtraBars.BarCheckItem()
        Me.TBConditionAdd = New DevExpress.XtraBars.BarButtonItem()
        Me.TBConditionDelete = New DevExpress.XtraBars.BarButtonItem()
        Me.TBConditionDeleteAll = New DevExpress.XtraBars.BarButtonItem()
        Me.TBConditionSave = New DevExpress.XtraBars.BarButtonItem()
        Me.BarDockControl = New DevExpress.XtraBars.StandaloneBarDockControl()
        Me.ToolBarFilter = New DevExpress.XtraBars.Bar()
        Me.TBFiltersText = New DevExpress.XtraBars.BarStaticItem()
        Me.TBFilterDelete = New DevExpress.XtraBars.BarButtonItem()
        Me.TBFilterDeleteAll = New DevExpress.XtraBars.BarButtonItem()
        Me.barDockControlTop = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlBottom = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlLeft = New DevExpress.XtraBars.BarDockControl()
        Me.barDockControlRight = New DevExpress.XtraBars.BarDockControl()
        Me.GridCondition = New DevExpress.XtraGrid.GridControl()
        Me.GridViewCondition = New DevExpress.XtraGrid.Views.Grid.GridView()
        Me.GridConditionName = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.LookUpEditFilterBy = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.GridConditionBatchField = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.GridConditionRelation = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.LookUpEditFilterRelation = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.GridConditionValue = New DevExpress.XtraGrid.Columns.GridColumn()
        Me.NumericSpinEdit = New DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit()
        Me.DateTimeEdit = New DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit()
        Me.LookUpEditValueBoolean = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.LookUpEditStatus = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.LookUpEditFilterRelationTemporary = New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit()
        Me.BtnCancel = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnOK = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnRefresh = New DevExpress.XtraEditors.SimpleButton()
        Me.WsManager = New DevExpress.Utils.WorkspaceManager(Me.components)
        Me.BarStaticItem2 = New DevExpress.XtraBars.BarStaticItem()
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer.SuspendLayout()
        CType(Me.GridBatch, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridViewBatch, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerFilters, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerFilters.SuspendLayout()
        CType(Me.GridFilter, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridViewFilters, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BarManager1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridCondition, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GridViewCondition, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LookUpEditFilterBy, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LookUpEditFilterRelation, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumericSpinEdit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DateTimeEdit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LookUpEditValueBoolean, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LookUpEditStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LookUpEditFilterRelationTemporary, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer
        '
        resources.ApplyResources(Me.SplitContainer, "SplitContainer")
        Me.SplitContainer.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        Me.SplitContainer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2
        Me.SplitContainer.Horizontal = False
        Me.SplitContainer.Name = "SplitContainer"
        Me.SplitContainer.Panel1.Controls.Add(Me.GridBatch)
        Me.SplitContainer.Panel1.MinSize = 150
        resources.ApplyResources(Me.SplitContainer.Panel1, "SplitContainer.Panel1")
        Me.SplitContainer.Panel2.Controls.Add(Me.SplitContainerFilters)
        Me.SplitContainer.Panel2.MinSize = 150
        resources.ApplyResources(Me.SplitContainer.Panel2, "SplitContainer.Panel2")
        Me.SplitContainer.SplitterPosition = 152
        '
        'GridBatch
        '
        resources.ApplyResources(Me.GridBatch, "GridBatch")
        Me.GridBatch.MainView = Me.GridViewBatch
        Me.GridBatch.Name = "GridBatch"
        Me.GridBatch.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridViewBatch})
        '
        'GridViewBatch
        '
        Me.GridViewBatch.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.Highlight
        Me.GridViewBatch.Appearance.FocusedRow.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.GridViewBatch.Appearance.FocusedRow.Options.UseBackColor = True
        Me.GridViewBatch.Appearance.FocusedRow.Options.UseForeColor = True
        Me.GridViewBatch.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.GridBatchName, Me.GridBatchClass, Me.GridBatchDateTime, Me.GridBatchStatus, Me.GridBatchDocCount, Me.GridBatchPageCount, Me.GridBatchPriority})
        Me.GridViewBatch.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None
        Me.GridViewBatch.GridControl = Me.GridBatch
        Me.GridViewBatch.Name = "GridViewBatch"
        Me.GridViewBatch.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.[False]
        Me.GridViewBatch.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[False]
        Me.GridViewBatch.OptionsBehavior.AllowIncrementalSearch = True
        Me.GridViewBatch.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.[True]
        Me.GridViewBatch.OptionsBehavior.AutoSelectAllInEditor = False
        Me.GridViewBatch.OptionsBehavior.Editable = False
        Me.GridViewBatch.OptionsBehavior.ReadOnly = True
        Me.GridViewBatch.OptionsCustomization.AllowColumnMoving = False
        Me.GridViewBatch.OptionsCustomization.AllowFilter = False
        Me.GridViewBatch.OptionsCustomization.AllowGroup = False
        Me.GridViewBatch.OptionsCustomization.AllowQuickHideColumns = False
        Me.GridViewBatch.OptionsDetail.EnableMasterViewMode = False
        Me.GridViewBatch.OptionsFilter.AllowFilterEditor = False
        Me.GridViewBatch.OptionsFind.AllowFindPanel = False
        Me.GridViewBatch.OptionsMenu.EnableColumnMenu = False
        Me.GridViewBatch.OptionsMenu.EnableFooterMenu = False
        Me.GridViewBatch.OptionsMenu.EnableGroupPanelMenu = False
        Me.GridViewBatch.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.GridViewBatch.OptionsSelection.EnableAppearanceHideSelection = False
        Me.GridViewBatch.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.GridViewBatch.OptionsView.ShowGroupPanel = False
        Me.GridViewBatch.OptionsView.ShowIndicator = False
        Me.GridViewBatch.SortInfo.AddRange(New DevExpress.XtraGrid.Columns.GridColumnSortInfo() {New DevExpress.XtraGrid.Columns.GridColumnSortInfo(Me.GridBatchPriority, DevExpress.Data.ColumnSortOrder.Ascending)})
        '
        'GridBatchName
        '
        resources.ApplyResources(Me.GridBatchName, "GridBatchName")
        Me.GridBatchName.FieldName = "Name"
        Me.GridBatchName.MinWidth = 200
        Me.GridBatchName.Name = "GridBatchName"
        '
        'GridBatchClass
        '
        resources.ApplyResources(Me.GridBatchClass, "GridBatchClass")
        Me.GridBatchClass.FieldName = "BatchClass"
        Me.GridBatchClass.MinWidth = 150
        Me.GridBatchClass.Name = "GridBatchClass"
        '
        'GridBatchDateTime
        '
        resources.ApplyResources(Me.GridBatchDateTime, "GridBatchDateTime")
        Me.GridBatchDateTime.FieldName = "BatchDateTime"
        Me.GridBatchDateTime.MinWidth = 100
        Me.GridBatchDateTime.Name = "GridBatchDateTime"
        '
        'GridBatchStatus
        '
        resources.ApplyResources(Me.GridBatchStatus, "GridBatchStatus")
        Me.GridBatchStatus.FieldName = "Status"
        Me.GridBatchStatus.MinWidth = 80
        Me.GridBatchStatus.Name = "GridBatchStatus"
        '
        'GridBatchDocCount
        '
        resources.ApplyResources(Me.GridBatchDocCount, "GridBatchDocCount")
        Me.GridBatchDocCount.FieldName = "DocumentCount"
        Me.GridBatchDocCount.MinWidth = 50
        Me.GridBatchDocCount.Name = "GridBatchDocCount"
        '
        'GridBatchPageCount
        '
        resources.ApplyResources(Me.GridBatchPageCount, "GridBatchPageCount")
        Me.GridBatchPageCount.FieldName = "PageCount"
        Me.GridBatchPageCount.MinWidth = 40
        Me.GridBatchPageCount.Name = "GridBatchPageCount"
        '
        'GridBatchPriority
        '
        resources.ApplyResources(Me.GridBatchPriority, "GridBatchPriority")
        Me.GridBatchPriority.FieldName = "Priority"
        Me.GridBatchPriority.MinWidth = 50
        Me.GridBatchPriority.Name = "GridBatchPriority"
        '
        'SplitContainerFilters
        '
        resources.ApplyResources(Me.SplitContainerFilters, "SplitContainerFilters")
        Me.SplitContainerFilters.Name = "SplitContainerFilters"
        Me.SplitContainerFilters.Panel1.Controls.Add(Me.GridFilter)
        Me.SplitContainerFilters.Panel1.Controls.Add(Me.StandaloneBarDockControl1)
        Me.SplitContainerFilters.Panel1.MinSize = 180
        resources.ApplyResources(Me.SplitContainerFilters.Panel1, "SplitContainerFilters.Panel1")
        Me.SplitContainerFilters.Panel2.Controls.Add(Me.GridCondition)
        Me.SplitContainerFilters.Panel2.Controls.Add(Me.BarDockControl)
        Me.SplitContainerFilters.Panel2.MinSize = 450
        resources.ApplyResources(Me.SplitContainerFilters.Panel2, "SplitContainerFilters.Panel2")
        Me.SplitContainerFilters.SplitterPosition = 209
        '
        'GridFilter
        '
        resources.ApplyResources(Me.GridFilter, "GridFilter")
        Me.GridFilter.MainView = Me.GridViewFilters
        Me.GridFilter.Name = "GridFilter"
        Me.GridFilter.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridViewFilters})
        '
        'GridViewFilters
        '
        Me.GridViewFilters.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.Highlight
        Me.GridViewFilters.Appearance.FocusedRow.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.GridViewFilters.Appearance.FocusedRow.Options.UseBackColor = True
        Me.GridViewFilters.Appearance.FocusedRow.Options.UseForeColor = True
        Me.GridViewFilters.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.GridFilterName})
        Me.GridViewFilters.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None
        Me.GridViewFilters.GridControl = Me.GridFilter
        Me.GridViewFilters.Name = "GridViewFilters"
        Me.GridViewFilters.OptionsBehavior.AllowIncrementalSearch = True
        Me.GridViewFilters.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.[True]
        Me.GridViewFilters.OptionsBehavior.AutoSelectAllInEditor = False
        Me.GridViewFilters.OptionsCustomization.AllowColumnMoving = False
        Me.GridViewFilters.OptionsCustomization.AllowFilter = False
        Me.GridViewFilters.OptionsCustomization.AllowGroup = False
        Me.GridViewFilters.OptionsCustomization.AllowQuickHideColumns = False
        Me.GridViewFilters.OptionsDetail.EnableMasterViewMode = False
        Me.GridViewFilters.OptionsFilter.AllowFilterEditor = False
        Me.GridViewFilters.OptionsFind.AllowFindPanel = False
        Me.GridViewFilters.OptionsMenu.EnableColumnMenu = False
        Me.GridViewFilters.OptionsMenu.EnableFooterMenu = False
        Me.GridViewFilters.OptionsMenu.EnableGroupPanelMenu = False
        Me.GridViewFilters.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.GridViewFilters.OptionsSelection.EnableAppearanceHideSelection = False
        Me.GridViewFilters.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.GridViewFilters.OptionsView.ShowGroupPanel = False
        Me.GridViewFilters.OptionsView.ShowIndicator = False
        Me.GridViewFilters.SortInfo.AddRange(New DevExpress.XtraGrid.Columns.GridColumnSortInfo() {New DevExpress.XtraGrid.Columns.GridColumnSortInfo(Me.GridFilterName, DevExpress.Data.ColumnSortOrder.Ascending)})
        '
        'GridFilterName
        '
        resources.ApplyResources(Me.GridFilterName, "GridFilterName")
        Me.GridFilterName.FieldName = "Name"
        Me.GridFilterName.MinWidth = 50
        Me.GridFilterName.Name = "GridFilterName"
        Me.GridFilterName.OptionsColumn.AllowEdit = False
        Me.GridFilterName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.[True]
        Me.GridFilterName.OptionsColumn.ReadOnly = True
        '
        'StandaloneBarDockControl1
        '
        Me.StandaloneBarDockControl1.CausesValidation = False
        resources.ApplyResources(Me.StandaloneBarDockControl1, "StandaloneBarDockControl1")
        Me.StandaloneBarDockControl1.Manager = Me.BarManager1
        Me.StandaloneBarDockControl1.Name = "StandaloneBarDockControl1"
        '
        'BarManager1
        '
        Me.BarManager1.Bars.AddRange(New DevExpress.XtraBars.Bar() {Me.ToolBarCondition, Me.ToolBarFilter})
        Me.BarManager1.DockControls.Add(Me.barDockControlTop)
        Me.BarManager1.DockControls.Add(Me.barDockControlBottom)
        Me.BarManager1.DockControls.Add(Me.barDockControlLeft)
        Me.BarManager1.DockControls.Add(Me.barDockControlRight)
        Me.BarManager1.DockControls.Add(Me.BarDockControl)
        Me.BarManager1.DockControls.Add(Me.StandaloneBarDockControl1)
        Me.BarManager1.Form = Me
        Me.BarManager1.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.TBConditionsEnabled, Me.TBConditionDeleteAll, Me.TBConditionsText, Me.TBConditionDelete, Me.TBConditionAdd, Me.TBConditionSave, Me.TBFiltersText, Me.TBFilterDelete, Me.TBFilterDeleteAll})
        Me.BarManager1.MaxItemId = 10
        '
        'ToolBarCondition
        '
        Me.ToolBarCondition.BarName = "Conditions"
        Me.ToolBarCondition.DockCol = 0
        Me.ToolBarCondition.DockRow = 0
        Me.ToolBarCondition.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone
        Me.ToolBarCondition.FloatLocation = New System.Drawing.Point(440, 416)
        Me.ToolBarCondition.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionsText, True), New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionsEnabled), New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionAdd), New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionDelete), New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionDeleteAll), New DevExpress.XtraBars.LinkPersistInfo(Me.TBConditionSave, True)})
        Me.ToolBarCondition.OptionsBar.AllowQuickCustomization = False
        Me.ToolBarCondition.OptionsBar.DisableCustomization = True
        Me.ToolBarCondition.OptionsBar.DrawDragBorder = False
        Me.ToolBarCondition.OptionsBar.UseWholeRow = True
        Me.ToolBarCondition.StandaloneBarDockControl = Me.BarDockControl
        resources.ApplyResources(Me.ToolBarCondition, "ToolBarCondition")
        '
        'TBConditionsText
        '
        Me.TBConditionsText.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        resources.ApplyResources(Me.TBConditionsText, "TBConditionsText")
        Me.TBConditionsText.Id = 3
        Me.TBConditionsText.Name = "TBConditionsText"
        '
        'TBConditionsEnabled
        '
        resources.ApplyResources(Me.TBConditionsEnabled, "TBConditionsEnabled")
        Me.TBConditionsEnabled.CausesValidation = True
        Me.TBConditionsEnabled.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText
        Me.TBConditionsEnabled.Id = 1
        Me.TBConditionsEnabled.Name = "TBConditionsEnabled"
        '
        'TBConditionAdd
        '
        resources.ApplyResources(Me.TBConditionAdd, "TBConditionAdd")
        Me.TBConditionAdd.Id = 5
        Me.TBConditionAdd.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.Add
        Me.TBConditionAdd.Name = "TBConditionAdd"
        '
        'TBConditionDelete
        '
        resources.ApplyResources(Me.TBConditionDelete, "TBConditionDelete")
        Me.TBConditionDelete.Id = 4
        Me.TBConditionDelete.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.Delete
        Me.TBConditionDelete.Name = "TBConditionDelete"
        '
        'TBConditionDeleteAll
        '
        resources.ApplyResources(Me.TBConditionDeleteAll, "TBConditionDeleteAll")
        Me.TBConditionDeleteAll.Id = 2
        Me.TBConditionDeleteAll.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.DeleteAll
        Me.TBConditionDeleteAll.Name = "TBConditionDeleteAll"
        '
        'TBConditionSave
        '
        resources.ApplyResources(Me.TBConditionSave, "TBConditionSave")
        Me.TBConditionSave.CausesValidation = True
        Me.TBConditionSave.Id = 6
        Me.TBConditionSave.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.Save
        Me.TBConditionSave.Name = "TBConditionSave"
        Me.TBConditionSave.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph
        '
        'BarDockControl
        '
        Me.BarDockControl.CausesValidation = False
        resources.ApplyResources(Me.BarDockControl, "BarDockControl")
        Me.BarDockControl.Manager = Me.BarManager1
        Me.BarDockControl.Name = "BarDockControl"
        '
        'ToolBarFilter
        '
        Me.ToolBarFilter.BarName = "Filters"
        Me.ToolBarFilter.DockCol = 0
        Me.ToolBarFilter.DockRow = 0
        Me.ToolBarFilter.DockStyle = DevExpress.XtraBars.BarDockStyle.Standalone
        Me.ToolBarFilter.FloatLocation = New System.Drawing.Point(439, 602)
        Me.ToolBarFilter.FloatSize = New System.Drawing.Size(46, 29)
        Me.ToolBarFilter.LinksPersistInfo.AddRange(New DevExpress.XtraBars.LinkPersistInfo() {New DevExpress.XtraBars.LinkPersistInfo(Me.TBFiltersText), New DevExpress.XtraBars.LinkPersistInfo(Me.TBFilterDelete), New DevExpress.XtraBars.LinkPersistInfo(Me.TBFilterDeleteAll)})
        Me.ToolBarFilter.OptionsBar.AllowQuickCustomization = False
        Me.ToolBarFilter.OptionsBar.DisableCustomization = True
        Me.ToolBarFilter.OptionsBar.DrawDragBorder = False
        Me.ToolBarFilter.OptionsBar.UseWholeRow = True
        Me.ToolBarFilter.StandaloneBarDockControl = Me.StandaloneBarDockControl1
        resources.ApplyResources(Me.ToolBarFilter, "ToolBarFilter")
        '
        'TBFiltersText
        '
        Me.TBFiltersText.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        resources.ApplyResources(Me.TBFiltersText, "TBFiltersText")
        Me.TBFiltersText.Id = 7
        Me.TBFiltersText.Name = "TBFiltersText"
        '
        'TBFilterDelete
        '
        resources.ApplyResources(Me.TBFilterDelete, "TBFilterDelete")
        Me.TBFilterDelete.Id = 8
        Me.TBFilterDelete.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.Delete
        Me.TBFilterDelete.Name = "TBFilterDelete"
        '
        'TBFilterDeleteAll
        '
        resources.ApplyResources(Me.TBFilterDeleteAll, "TBFilterDeleteAll")
        Me.TBFilterDeleteAll.Id = 9
        Me.TBFilterDeleteAll.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.DeleteAll
        Me.TBFilterDeleteAll.Name = "TBFilterDeleteAll"
        '
        'barDockControlTop
        '
        Me.barDockControlTop.CausesValidation = False
        resources.ApplyResources(Me.barDockControlTop, "barDockControlTop")
        Me.barDockControlTop.Manager = Me.BarManager1
        '
        'barDockControlBottom
        '
        Me.barDockControlBottom.CausesValidation = False
        resources.ApplyResources(Me.barDockControlBottom, "barDockControlBottom")
        Me.barDockControlBottom.Manager = Me.BarManager1
        '
        'barDockControlLeft
        '
        Me.barDockControlLeft.CausesValidation = False
        resources.ApplyResources(Me.barDockControlLeft, "barDockControlLeft")
        Me.barDockControlLeft.Manager = Me.BarManager1
        '
        'barDockControlRight
        '
        Me.barDockControlRight.CausesValidation = False
        resources.ApplyResources(Me.barDockControlRight, "barDockControlRight")
        Me.barDockControlRight.Manager = Me.BarManager1
        '
        'GridCondition
        '
        resources.ApplyResources(Me.GridCondition, "GridCondition")
        Me.GridCondition.MainView = Me.GridViewCondition
        Me.GridCondition.Name = "GridCondition"
        Me.GridCondition.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.LookUpEditFilterBy, Me.LookUpEditFilterRelation, Me.NumericSpinEdit, Me.DateTimeEdit, Me.LookUpEditValueBoolean, Me.LookUpEditStatus, Me.LookUpEditFilterRelationTemporary})
        Me.GridCondition.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.GridViewCondition})
        '
        'GridViewCondition
        '
        Me.GridViewCondition.Appearance.FocusedRow.BackColor = System.Drawing.SystemColors.Highlight
        Me.GridViewCondition.Appearance.FocusedRow.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.GridViewCondition.Appearance.FocusedRow.Options.UseBackColor = True
        Me.GridViewCondition.Appearance.FocusedRow.Options.UseForeColor = True
        Me.GridViewCondition.Columns.AddRange(New DevExpress.XtraGrid.Columns.GridColumn() {Me.GridConditionName, Me.GridConditionBatchField, Me.GridConditionRelation, Me.GridConditionValue})
        Me.GridViewCondition.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None
        Me.GridViewCondition.GridControl = Me.GridCondition
        Me.GridViewCondition.Name = "GridViewCondition"
        Me.GridViewCondition.OptionsBehavior.AllowIncrementalSearch = True
        Me.GridViewCondition.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.[True]
        Me.GridViewCondition.OptionsBehavior.AutoSelectAllInEditor = False
        Me.GridViewCondition.OptionsCustomization.AllowColumnMoving = False
        Me.GridViewCondition.OptionsCustomization.AllowFilter = False
        Me.GridViewCondition.OptionsCustomization.AllowGroup = False
        Me.GridViewCondition.OptionsCustomization.AllowQuickHideColumns = False
        Me.GridViewCondition.OptionsDetail.EnableMasterViewMode = False
        Me.GridViewCondition.OptionsFilter.AllowFilterEditor = False
        Me.GridViewCondition.OptionsFind.AllowFindPanel = False
        Me.GridViewCondition.OptionsMenu.EnableColumnMenu = False
        Me.GridViewCondition.OptionsMenu.EnableFooterMenu = False
        Me.GridViewCondition.OptionsMenu.EnableGroupPanelMenu = False
        Me.GridViewCondition.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.GridViewCondition.OptionsSelection.EnableAppearanceHideSelection = False
        Me.GridViewCondition.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never
        Me.GridViewCondition.OptionsView.ShowGroupPanel = False
        Me.GridViewCondition.OptionsView.ShowIndicator = False
        '
        'GridConditionName
        '
        resources.ApplyResources(Me.GridConditionName, "GridConditionName")
        Me.GridConditionName.ColumnEdit = Me.LookUpEditFilterBy
        Me.GridConditionName.FieldName = "ConditionFilterBy"
        Me.GridConditionName.MinWidth = 50
        Me.GridConditionName.Name = "GridConditionName"
        Me.GridConditionName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor
        '
        'LookUpEditFilterBy
        '
        resources.ApplyResources(Me.LookUpEditFilterBy, "LookUpEditFilterBy")
        Me.LookUpEditFilterBy.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("LookUpEditFilterBy.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.LookUpEditFilterBy.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("LookUpEditFilterBy.Columns"), resources.GetString("LookUpEditFilterBy.Columns1"), CType(resources.GetObject("LookUpEditFilterBy.Columns2"), Integer), CType(resources.GetObject("LookUpEditFilterBy.Columns3"), DevExpress.Utils.FormatType), resources.GetString("LookUpEditFilterBy.Columns4"), CType(resources.GetObject("LookUpEditFilterBy.Columns5"), Boolean), CType(resources.GetObject("LookUpEditFilterBy.Columns6"), DevExpress.Utils.HorzAlignment), CType(resources.GetObject("LookUpEditFilterBy.Columns7"), DevExpress.Data.ColumnSortOrder), CType(resources.GetObject("LookUpEditFilterBy.Columns8"), DevExpress.Utils.DefaultBoolean))})
        Me.LookUpEditFilterBy.DisplayMember = "DisplayName"
        Me.LookUpEditFilterBy.DropDownRows = 10
        Me.LookUpEditFilterBy.Name = "LookUpEditFilterBy"
        Me.LookUpEditFilterBy.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth
        Me.LookUpEditFilterBy.ShowFooter = False
        Me.LookUpEditFilterBy.ShowHeader = False
        Me.LookUpEditFilterBy.ValueMember = "Value"
        '
        'GridConditionBatchField
        '
        resources.ApplyResources(Me.GridConditionBatchField, "GridConditionBatchField")
        Me.GridConditionBatchField.FieldName = "ConditionBatchField"
        Me.GridConditionBatchField.MinWidth = 50
        Me.GridConditionBatchField.Name = "GridConditionBatchField"
        '
        'GridConditionRelation
        '
        resources.ApplyResources(Me.GridConditionRelation, "GridConditionRelation")
        Me.GridConditionRelation.ColumnEdit = Me.LookUpEditFilterRelation
        Me.GridConditionRelation.FieldName = "ConditionRelation"
        Me.GridConditionRelation.MinWidth = 50
        Me.GridConditionRelation.Name = "GridConditionRelation"
        Me.GridConditionRelation.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor
        '
        'LookUpEditFilterRelation
        '
        resources.ApplyResources(Me.LookUpEditFilterRelation, "LookUpEditFilterRelation")
        Me.LookUpEditFilterRelation.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("LookUpEditFilterRelation.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.LookUpEditFilterRelation.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("LookUpEditFilterRelation.Columns"), resources.GetString("LookUpEditFilterRelation.Columns1"), CType(resources.GetObject("LookUpEditFilterRelation.Columns2"), Integer), CType(resources.GetObject("LookUpEditFilterRelation.Columns3"), DevExpress.Utils.FormatType), resources.GetString("LookUpEditFilterRelation.Columns4"), CType(resources.GetObject("LookUpEditFilterRelation.Columns5"), Boolean), CType(resources.GetObject("LookUpEditFilterRelation.Columns6"), DevExpress.Utils.HorzAlignment), CType(resources.GetObject("LookUpEditFilterRelation.Columns7"), DevExpress.Data.ColumnSortOrder), CType(resources.GetObject("LookUpEditFilterRelation.Columns8"), DevExpress.Utils.DefaultBoolean))})
        Me.LookUpEditFilterRelation.DisplayMember = "DisplayName"
        Me.LookUpEditFilterRelation.DropDownRows = 6
        Me.LookUpEditFilterRelation.Name = "LookUpEditFilterRelation"
        Me.LookUpEditFilterRelation.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth
        Me.LookUpEditFilterRelation.ShowFooter = False
        Me.LookUpEditFilterRelation.ShowHeader = False
        Me.LookUpEditFilterRelation.ValueMember = "Value"
        '
        'GridConditionValue
        '
        resources.ApplyResources(Me.GridConditionValue, "GridConditionValue")
        Me.GridConditionValue.FieldName = "ConditionValue"
        Me.GridConditionValue.MinWidth = 50
        Me.GridConditionValue.Name = "GridConditionValue"
        '
        'NumericSpinEdit
        '
        resources.ApplyResources(Me.NumericSpinEdit, "NumericSpinEdit")
        Me.NumericSpinEdit.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("NumericSpinEdit.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.NumericSpinEdit.IsFloatValue = False
        Me.NumericSpinEdit.Mask.EditMask = resources.GetString("NumericSpinEdit.Mask.EditMask")
        Me.NumericSpinEdit.MaxValue = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericSpinEdit.Name = "NumericSpinEdit"
        '
        'DateTimeEdit
        '
        resources.ApplyResources(Me.DateTimeEdit, "DateTimeEdit")
        Me.DateTimeEdit.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("DateTimeEdit.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.DateTimeEdit.Mask.EditMask = resources.GetString("DateTimeEdit.Mask.EditMask")
        Me.DateTimeEdit.Name = "DateTimeEdit"
        '
        'LookUpEditValueBoolean
        '
        resources.ApplyResources(Me.LookUpEditValueBoolean, "LookUpEditValueBoolean")
        Me.LookUpEditValueBoolean.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("LookUpEditValueBoolean.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.LookUpEditValueBoolean.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("LookUpEditValueBoolean.Columns"), resources.GetString("LookUpEditValueBoolean.Columns1"))})
        Me.LookUpEditValueBoolean.DisplayMember = "DisplayName"
        Me.LookUpEditValueBoolean.DropDownRows = 2
        Me.LookUpEditValueBoolean.Name = "LookUpEditValueBoolean"
        Me.LookUpEditValueBoolean.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth
        Me.LookUpEditValueBoolean.ShowFooter = False
        Me.LookUpEditValueBoolean.ShowHeader = False
        Me.LookUpEditValueBoolean.ValueMember = "Value"
        '
        'LookUpEditStatus
        '
        resources.ApplyResources(Me.LookUpEditStatus, "LookUpEditStatus")
        Me.LookUpEditStatus.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("LookUpEditStatus.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.LookUpEditStatus.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("LookUpEditStatus.Columns"), resources.GetString("LookUpEditStatus.Columns1"))})
        Me.LookUpEditStatus.DisplayMember = "DisplayName"
        Me.LookUpEditStatus.DropDownRows = 2
        Me.LookUpEditStatus.Name = "LookUpEditStatus"
        Me.LookUpEditStatus.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth
        Me.LookUpEditStatus.ShowFooter = False
        Me.LookUpEditStatus.ShowHeader = False
        Me.LookUpEditStatus.ValueMember = "Value"
        '
        'LookUpEditFilterRelationTemporary
        '
        resources.ApplyResources(Me.LookUpEditFilterRelationTemporary, "LookUpEditFilterRelationTemporary")
        Me.LookUpEditFilterRelationTemporary.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("LookUpEditFilterRelationTemporary.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.LookUpEditFilterRelationTemporary.Columns.AddRange(New DevExpress.XtraEditors.Controls.LookUpColumnInfo() {New DevExpress.XtraEditors.Controls.LookUpColumnInfo(resources.GetString("LookUpEditFilterRelationTemporary.Columns"), resources.GetString("LookUpEditFilterRelationTemporary.Columns1"))})
        Me.LookUpEditFilterRelationTemporary.DisplayMember = "DisplayName"
        Me.LookUpEditFilterRelationTemporary.Name = "LookUpEditFilterRelationTemporary"
        Me.LookUpEditFilterRelationTemporary.PopupWidthMode = DevExpress.XtraEditors.PopupWidthMode.UseEditorWidth
        Me.LookUpEditFilterRelationTemporary.ShowFooter = False
        Me.LookUpEditFilterRelationTemporary.ShowHeader = False
        Me.LookUpEditFilterRelationTemporary.ValueMember = "Value"
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.Name = "BtnOK"
        '
        'BtnRefresh
        '
        resources.ApplyResources(Me.BtnRefresh, "BtnRefresh")
        Me.BtnRefresh.Name = "BtnRefresh"
        '
        'WsManager
        '
        Me.WsManager.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.[False]
        Me.WsManager.TargetControl = Me
        Me.WsManager.TransitionType = PushTransition1
        '
        'BarStaticItem2
        '
        Me.BarStaticItem2.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        resources.ApplyResources(Me.BarStaticItem2, "BarStaticItem2")
        Me.BarStaticItem2.Id = 3
        Me.BarStaticItem2.Name = "BarStaticItem2"
        '
        'FrmOpenBatch
        '
        Me.AcceptButton = Me.BtnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.SplitContainer)
        Me.Controls.Add(Me.BtnRefresh)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.barDockControlLeft)
        Me.Controls.Add(Me.barDockControlRight)
        Me.Controls.Add(Me.barDockControlBottom)
        Me.Controls.Add(Me.barDockControlTop)
        Me.LookAndFeel.SkinName = "Office 2010 Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.Name = "FrmOpenBatch"
        Me.ShowInTaskbar = False
        CType(Me.SplitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer.ResumeLayout(False)
        CType(Me.GridBatch, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridViewBatch, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.SplitContainerFilters, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerFilters.ResumeLayout(False)
        CType(Me.GridFilter, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridViewFilters, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BarManager1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridCondition, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GridViewCondition, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LookUpEditFilterBy, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LookUpEditFilterRelation, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumericSpinEdit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DateTimeEdit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LookUpEditValueBoolean, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LookUpEditStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LookUpEditFilterRelationTemporary, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents BtnCancel As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnOK As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnRefresh As DevExpress.XtraEditors.SimpleButton
    Private WithEvents SplitContainer As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents GridCondition As DevExpress.XtraGrid.GridControl
    Private WithEvents GridViewCondition As DevExpress.XtraGrid.Views.Grid.GridView
    Private WithEvents GridConditionName As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridConditionBatchField As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridConditionRelation As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridConditionValue As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents BarDockControl As DevExpress.XtraBars.StandaloneBarDockControl
    Private WithEvents ToolBarCondition As DevExpress.XtraBars.Bar
    Private WithEvents TBConditionsEnabled As DevExpress.XtraBars.BarCheckItem
    Private WithEvents TBConditionDeleteAll As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents barDockControlTop As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlBottom As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlLeft As DevExpress.XtraBars.BarDockControl
    Friend WithEvents barDockControlRight As DevExpress.XtraBars.BarDockControl
    Private WithEvents TBConditionsText As DevExpress.XtraBars.BarStaticItem
    Private WithEvents TBConditionDelete As DevExpress.XtraBars.BarButtonItem
    Private WithEvents TBConditionAdd As DevExpress.XtraBars.BarButtonItem
    Private WithEvents LookUpEditFilterBy As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Private WithEvents LookUpEditFilterRelation As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Private WithEvents NumericSpinEdit As DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
    Private WithEvents DateTimeEdit As DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit
    Private WithEvents LookUpEditValueBoolean As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Private WithEvents LookUpEditStatus As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Private WithEvents LookUpEditFilterRelationTemporary As DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
    Private WithEvents GridBatch As DevExpress.XtraGrid.GridControl
    Private WithEvents GridViewBatch As DevExpress.XtraGrid.Views.Grid.GridView
    Private WithEvents GridBatchName As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridBatchClass As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridBatchDateTime As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridBatchStatus As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridBatchDocCount As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents GridBatchPriority As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents WsManager As DevExpress.Utils.WorkspaceManager
    Private WithEvents BarManager1 As DevExpress.XtraBars.BarManager
    Private WithEvents StandaloneBarDockControl1 As DevExpress.XtraBars.StandaloneBarDockControl
    Private WithEvents ToolBarFilter As DevExpress.XtraBars.Bar
    Private WithEvents TBConditionSave As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BarStaticItem2 As DevExpress.XtraBars.BarStaticItem
    Private WithEvents TBFiltersText As DevExpress.XtraBars.BarStaticItem
    Private WithEvents TBFilterDelete As DevExpress.XtraBars.BarButtonItem
    Private WithEvents TBFilterDeleteAll As DevExpress.XtraBars.BarButtonItem
    Private WithEvents GridFilter As DevExpress.XtraGrid.GridControl
    Private WithEvents GridViewFilters As DevExpress.XtraGrid.Views.Grid.GridView
    Private WithEvents GridFilterName As DevExpress.XtraGrid.Columns.GridColumn
    Private WithEvents SplitContainerFilters As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents GridBatchPageCount As DevExpress.XtraGrid.Columns.GridColumn
End Class
