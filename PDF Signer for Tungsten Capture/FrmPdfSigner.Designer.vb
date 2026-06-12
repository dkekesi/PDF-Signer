<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmPdfSigner
    Inherits DevExpress.XtraBars.Ribbon.RibbonForm

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmPdfSigner))
        Dim PushTransition1 As DevExpress.Utils.Animation.PushTransition = New DevExpress.Utils.Animation.PushTransition()
        Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
        Me.PanelControl2 = New DevExpress.XtraEditors.PanelControl()
        Me.BatchTree = New DevExpress.XtraTreeList.TreeList()
        Me.ColBatchItem = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.ColProgressBar = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.BatchTreeProgressBar = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.BatchContentsImages = New DevExpress.Utils.ImageCollection(Me.components)
        Me.BatchTreeToolTip = New DevExpress.Utils.ToolTipController(Me.components)
        Me.GrpPartialCopy = New DevExpress.XtraEditors.GroupControl()
        Me.ComboPartialCopy = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.DocumentItemBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.RibbonControl = New DevExpress.XtraBars.Ribbon.RibbonControl()
        Me.BtnBatchOpen = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnBatchClose = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnBatchSuspend = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnSign = New DevExpress.XtraBars.BarButtonItem()
        Me.BarComboCert = New DevExpress.XtraBars.BarEditItem()
        Me.ComboCert = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.BtnRemoveSignatures = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnReject = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnUnreject = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnRotateLeft = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnRotateRight = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnDocFirst = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnDocPrevious = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnDocNext = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnDocLast = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnPagePrevious = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnPageNext = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnZoomWhole = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnZoomWidth = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnZoomIn = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnZoomOut = New DevExpress.XtraBars.BarButtonItem()
        Me.BarLabelUser = New DevExpress.XtraBars.BarStaticItem()
        Me.BtnDeletePage = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnAbout = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnZoom1on1 = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnRefreshCertificates = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnViewOriginal = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnViewSigned = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnRegulation = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnLayoutSingle = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnLayoutDouble = New DevExpress.XtraBars.BarButtonItem()
        Me.BarWorkspaceMenuItem1 = New DevExpress.XtraBars.BarWorkspaceMenuItem()
        Me.WsManager = New DevExpress.Utils.WorkspaceManager(Me.components)
        Me.BtnWorkspaceReset = New DevExpress.XtraBars.BarButtonItem()
        Me.BtnSkipSign = New DevExpress.XtraBars.BarButtonItem()
        Me.BarComboProvider = New DevExpress.XtraBars.BarEditItem()
        Me.ComboProvider = New DevExpress.XtraEditors.Repository.RepositoryItemComboBox()
        Me.LargeRibbonImages = New DevExpress.Utils.ImageCollection(Me.components)
        Me.RibbonPageHome = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroupBatch = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupSignature = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupView = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupReject = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupEdit = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupAbout = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageNavigation = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroupNavigationPage = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupNavigationDocument = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageView = New DevExpress.XtraBars.Ribbon.RibbonPage()
        Me.RibbonPageGroupZoom = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupLayout = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.RibbonPageGroupWorkspace = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
        Me.ProgressBar = New DevExpress.XtraEditors.Repository.RepositoryItemProgressBar()
        Me.RibbonStatusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()
        Me.TabViewer = New DevExpress.XtraTab.XtraTabControl()
        Me.TabOriginal = New DevExpress.XtraTab.XtraTabPage()
        Me.PdfViewerOriginal = New PDFSigner.ViewerBase()
        Me.TabSigned = New DevExpress.XtraTab.XtraTabPage()
        Me.PdfViewerSigned = New PDFSigner.ViewerBase()
        Me.TabClause = New DevExpress.XtraTab.XtraTabPage()
        Me.MemoClause = New DevExpress.XtraEditors.MemoEdit()
        Me.TabLog = New DevExpress.XtraTab.XtraTabPage()
        Me.MemoLog = New DevExpress.XtraEditors.MemoEdit()
        Me.LblFileNotFound = New System.Windows.Forms.Label()
        Me.DefaultBarAndDockingController = New DevExpress.XtraBars.DefaultBarAndDockingController(Me.components)
        Me.BarEditItem1 = New DevExpress.XtraBars.BarEditItem()
        Me.BarEditItem2 = New DevExpress.XtraBars.BarEditItem()
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerControl1.Panel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.Panel1.SuspendLayout()
        CType(Me.SplitContainerControl1.Panel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerControl1.Panel2.SuspendLayout()
        Me.SplitContainerControl1.SuspendLayout()
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl2.SuspendLayout()
        CType(Me.BatchTree, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BatchTreeProgressBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BatchContentsImages, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.GrpPartialCopy, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GrpPartialCopy.SuspendLayout()
        CType(Me.ComboPartialCopy.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DocumentItemBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ComboCert, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ComboProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.LargeRibbonImages, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProgressBar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TabViewer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabViewer.SuspendLayout()
        Me.TabOriginal.SuspendLayout()
        Me.TabSigned.SuspendLayout()
        Me.TabClause.SuspendLayout()
        CType(Me.MemoClause.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabLog.SuspendLayout()
        CType(Me.MemoLog.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DefaultBarAndDockingController.Controller, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainerControl1
        '
        Me.SplitContainerControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.SplitContainerControl1.Appearance.Options.UseBackColor = True
        Me.SplitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1
        resources.ApplyResources(Me.SplitContainerControl1, "SplitContainerControl1")
        Me.SplitContainerControl1.Name = "SplitContainerControl1"
        '
        'SplitContainerControl1.Panel1
        '
        Me.SplitContainerControl1.Panel1.Controls.Add(Me.PanelControl2)
        Me.SplitContainerControl1.Panel1.MinSize = 220
        resources.ApplyResources(Me.SplitContainerControl1.Panel1, "SplitContainerControl1.Panel1")
        '
        'SplitContainerControl1.Panel2
        '
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.TabViewer)
        Me.SplitContainerControl1.Panel2.Controls.Add(Me.LblFileNotFound)
        Me.SplitContainerControl1.Panel2.MinSize = 300
        resources.ApplyResources(Me.SplitContainerControl1.Panel2, "SplitContainerControl1.Panel2")
        Me.SplitContainerControl1.SplitterPosition = 308
        '
        'PanelControl2
        '
        Me.PanelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl2.Controls.Add(Me.BatchTree)
        Me.PanelControl2.Controls.Add(Me.GrpPartialCopy)
        resources.ApplyResources(Me.PanelControl2, "PanelControl2")
        Me.PanelControl2.Name = "PanelControl2"
        '
        'BatchTree
        '
        Me.BatchTree.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.BatchTree.Appearance.Empty.Options.UseBackColor = True
        Me.BatchTree.Appearance.FocusedRow.BackColor = System.Drawing.Color.WhiteSmoke
        Me.BatchTree.Appearance.FocusedRow.BackColor2 = CType(resources.GetObject("BatchTree.Appearance.FocusedRow.BackColor2"), System.Drawing.Color)
        Me.BatchTree.Appearance.FocusedRow.Options.UseBackColor = True
        Me.BatchTree.Appearance.Row.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.BatchTree.Appearance.Row.Options.UseBackColor = True
        Me.BatchTree.Appearance.SelectedRow.BackColor = System.Drawing.SystemColors.Highlight
        Me.BatchTree.Appearance.SelectedRow.ForeColor = System.Drawing.SystemColors.HighlightText
        Me.BatchTree.Appearance.SelectedRow.Options.UseBackColor = True
        Me.BatchTree.Appearance.SelectedRow.Options.UseForeColor = True
        Me.BatchTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.BatchTree.Columns.AddRange(New DevExpress.XtraTreeList.Columns.TreeListColumn() {Me.ColBatchItem, Me.ColProgressBar})
        Me.BatchTree.Cursor = System.Windows.Forms.Cursors.Default
        resources.ApplyResources(Me.BatchTree, "BatchTree")
        Me.BatchTree.ImageIndexFieldName = "IconType"
        Me.BatchTree.Name = "BatchTree"
        Me.BatchTree.OptionsBehavior.Editable = False
        Me.BatchTree.OptionsBehavior.ReadOnly = True
        Me.BatchTree.OptionsBehavior.ResizeNodes = False
        Me.BatchTree.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.[False]
        Me.BatchTree.OptionsCustomization.AllowBandMoving = False
        Me.BatchTree.OptionsCustomization.AllowBandResizing = False
        Me.BatchTree.OptionsCustomization.AllowChangeBandParent = True
        Me.BatchTree.OptionsCustomization.AllowChangeColumnParent = True
        Me.BatchTree.OptionsCustomization.AllowFilter = False
        Me.BatchTree.OptionsLayout.AddNewColumns = False
        Me.BatchTree.OptionsMenu.EnableColumnMenu = False
        Me.BatchTree.OptionsMenu.EnableFooterMenu = False
        Me.BatchTree.OptionsMenu.ShowAutoFilterRowItem = False
        Me.BatchTree.OptionsMenu.ShowConditionalFormattingItem = True
        Me.BatchTree.OptionsSelection.EnableAppearanceFocusedCell = False
        Me.BatchTree.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus
        Me.BatchTree.OptionsView.ShowHorzLines = False
        Me.BatchTree.OptionsView.ShowIndicator = False
        Me.BatchTree.OptionsView.ShowVertLines = False
        Me.BatchTree.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.BatchTreeProgressBar})
        Me.BatchTree.SelectImageList = Me.BatchContentsImages
        Me.BatchTree.ToolTipController = Me.BatchTreeToolTip
        '
        'ColBatchItem
        '
        resources.ApplyResources(Me.ColBatchItem, "ColBatchItem")
        Me.ColBatchItem.FieldName = "DisplayName"
        Me.ColBatchItem.Name = "ColBatchItem"
        Me.ColBatchItem.OptionsColumn.AllowEdit = False
        Me.ColBatchItem.OptionsColumn.AllowMove = False
        Me.ColBatchItem.OptionsColumn.AllowSort = False
        Me.ColBatchItem.OptionsColumn.ReadOnly = True
        Me.ColBatchItem.OptionsColumn.ShowInCustomizationForm = False
        Me.ColBatchItem.OptionsColumn.ShowInExpressionEditor = False
        '
        'ColProgressBar
        '
        resources.ApplyResources(Me.ColProgressBar, "ColProgressBar")
        Me.ColProgressBar.ColumnEdit = Me.BatchTreeProgressBar
        Me.ColProgressBar.FieldName = "ProgressPercent"
        Me.ColProgressBar.Name = "ColProgressBar"
        Me.ColProgressBar.OptionsColumn.AllowEdit = False
        Me.ColProgressBar.OptionsColumn.AllowMove = False
        Me.ColProgressBar.OptionsColumn.AllowSort = False
        Me.ColProgressBar.OptionsColumn.ReadOnly = True
        Me.ColProgressBar.OptionsColumn.ShowInCustomizationForm = False
        Me.ColProgressBar.OptionsColumn.ShowInExpressionEditor = False
        '
        'BatchTreeProgressBar
        '
        Me.BatchTreeProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D
        Me.BatchTreeProgressBar.LookAndFeel.UseDefaultLookAndFeel = False
        Me.BatchTreeProgressBar.Name = "BatchTreeProgressBar"
        Me.BatchTreeProgressBar.PercentView = False
        Me.BatchTreeProgressBar.ProgressPadding = New System.Windows.Forms.Padding(2)
        Me.BatchTreeProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid
        Me.BatchTreeProgressBar.ReadOnly = True
        Me.BatchTreeProgressBar.ShowTitle = True
        Me.BatchTreeProgressBar.Step = 1
        '
        'BatchContentsImages
        '
        resources.ApplyResources(Me.BatchContentsImages, "BatchContentsImages")
        Me.BatchContentsImages.ImageStream = CType(resources.GetObject("BatchContentsImages.ImageStream"), DevExpress.Utils.ImageCollectionStreamer)
        Me.BatchContentsImages.Images.SetKeyName(0, "Batch.png")
        Me.BatchContentsImages.Images.SetKeyName(1, "Document.png")
        Me.BatchContentsImages.Images.SetKeyName(2, "Page.png")
        Me.BatchContentsImages.Images.SetKeyName(3, "DocumentRejected.png")
        Me.BatchContentsImages.Images.SetKeyName(4, "PageRejected.png")
        Me.BatchContentsImages.Images.SetKeyName(5, "DocumentSigned.png")
        Me.BatchContentsImages.Images.SetKeyName(6, "DocumentToBeSigned.png")
        Me.BatchContentsImages.Images.SetKeyName(7, "DocumentToBeSignedRejected.png")
        Me.BatchContentsImages.Images.SetKeyName(8, "DocumentNotSignable.png")
        Me.BatchContentsImages.Images.SetKeyName(9, "DocumentNotSignableRejected.png")
        Me.BatchContentsImages.Images.SetKeyName(10, "DocumentSkipped.png")
        '
        'BatchTreeToolTip
        '
        Me.BatchTreeToolTip.AutoPopDelay = 15000
        Me.BatchTreeToolTip.InitialDelay = 300
        '
        'GrpPartialCopy
        '
        Me.GrpPartialCopy.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.GrpPartialCopy.Appearance.BackColor2 = CType(resources.GetObject("GrpPartialCopy.Appearance.BackColor2"), System.Drawing.Color)
        Me.GrpPartialCopy.Appearance.Options.UseBackColor = True
        Me.GrpPartialCopy.Controls.Add(Me.ComboPartialCopy)
        resources.ApplyResources(Me.GrpPartialCopy, "GrpPartialCopy")
        Me.GrpPartialCopy.Name = "GrpPartialCopy"
        '
        'ComboPartialCopy
        '
        resources.ApplyResources(Me.ComboPartialCopy, "ComboPartialCopy")
        Me.ComboPartialCopy.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DocumentItemBindingSource, "PartialCopyData", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboPartialCopy.MenuManager = Me.RibbonControl
        Me.ComboPartialCopy.Name = "ComboPartialCopy"
        Me.ComboPartialCopy.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("ComboPartialCopy.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.ComboPartialCopy.Properties.Items.AddRange(New Object() {resources.GetString("ComboPartialCopy.Properties.Items"), resources.GetString("ComboPartialCopy.Properties.Items1"), resources.GetString("ComboPartialCopy.Properties.Items2")})
        Me.ComboPartialCopy.TabStop = False
        '
        'DocumentItemBindingSource
        '
        Me.DocumentItemBindingSource.DataSource = GetType(PDFSigner.DocumentItem)
        '
        'RibbonControl
        '
        Me.RibbonControl.AllowMinimizeRibbon = False
        Me.RibbonControl.ApplicationButtonImageOptions.Image = Global.PDFSigner.My.Resources.Resources.PDFSigner
        Me.RibbonControl.ExpandCollapseItem.Id = 0
        Me.RibbonControl.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.RibbonControl.ExpandCollapseItem, Me.BtnBatchOpen, Me.BtnBatchClose, Me.BtnBatchSuspend, Me.BtnSign, Me.BarComboCert, Me.BtnRemoveSignatures, Me.BtnReject, Me.BtnUnreject, Me.BtnRotateLeft, Me.BtnRotateRight, Me.BtnDocFirst, Me.BtnDocPrevious, Me.BtnDocNext, Me.BtnDocLast, Me.BtnPagePrevious, Me.BtnPageNext, Me.BtnZoomWhole, Me.BtnZoomWidth, Me.BtnZoomIn, Me.BtnZoomOut, Me.BarLabelUser, Me.BtnDeletePage, Me.BtnAbout, Me.BtnZoom1on1, Me.BtnRefreshCertificates, Me.BtnViewOriginal, Me.BtnViewSigned, Me.BtnRegulation, Me.BtnLayoutSingle, Me.BtnLayoutDouble, Me.BarWorkspaceMenuItem1, Me.BtnWorkspaceReset, Me.BtnSkipSign, Me.BarComboProvider})
        Me.RibbonControl.LargeImages = Me.LargeRibbonImages
        resources.ApplyResources(Me.RibbonControl, "RibbonControl")
        Me.RibbonControl.MaxItemId = 45
        Me.RibbonControl.Name = "RibbonControl"
        Me.RibbonControl.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.RibbonPageHome, Me.RibbonPageNavigation, Me.RibbonPageView})
        Me.RibbonControl.RepositoryItems.AddRange(New DevExpress.XtraEditors.Repository.RepositoryItem() {Me.ComboCert, Me.ProgressBar, Me.ComboProvider})
        Me.RibbonControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013
        Me.RibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonControl.ShowToolbarCustomizeItem = False
        Me.RibbonControl.StatusBar = Me.RibbonStatusBar
        Me.RibbonControl.Toolbar.ShowCustomizeItem = False
        '
        'BtnBatchOpen
        '
        resources.ApplyResources(Me.BtnBatchOpen, "BtnBatchOpen")
        Me.BtnBatchOpen.Id = 2
        Me.BtnBatchOpen.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnBatchOpen.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnBatchOpen.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.O))
        Me.BtnBatchOpen.Name = "BtnBatchOpen"
        Me.BtnBatchOpen.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnBatchClose
        '
        resources.ApplyResources(Me.BtnBatchClose, "BtnBatchClose")
        Me.BtnBatchClose.Id = 4
        Me.BtnBatchClose.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnBatchClose.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnBatchClose.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.L))
        Me.BtnBatchClose.Name = "BtnBatchClose"
        Me.BtnBatchClose.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnBatchSuspend
        '
        resources.ApplyResources(Me.BtnBatchSuspend, "BtnBatchSuspend")
        Me.BtnBatchSuspend.Id = 5
        Me.BtnBatchSuspend.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnBatchSuspend.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnBatchSuspend.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.S))
        Me.BtnBatchSuspend.Name = "BtnBatchSuspend"
        Me.BtnBatchSuspend.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnSign
        '
        resources.ApplyResources(Me.BtnSign, "BtnSign")
        Me.BtnSign.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnSign.Id = 7
        Me.BtnSign.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnSign.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnSign.ItemAppearance.Hovered.Font = CType(resources.GetObject("BtnSign.ItemAppearance.Hovered.Font"), System.Drawing.Font)
        Me.BtnSign.ItemAppearance.Hovered.Options.UseFont = True
        Me.BtnSign.ItemAppearance.Normal.Font = CType(resources.GetObject("BtnSign.ItemAppearance.Normal.Font"), System.Drawing.Font)
        Me.BtnSign.ItemAppearance.Normal.Options.UseFont = True
        Me.BtnSign.ItemAppearance.Pressed.Font = CType(resources.GetObject("BtnSign.ItemAppearance.Pressed.Font"), System.Drawing.Font)
        Me.BtnSign.ItemAppearance.Pressed.Options.UseFont = True
        Me.BtnSign.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F9)
        Me.BtnSign.Name = "BtnSign"
        Me.BtnSign.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BarComboCert
        '
        resources.ApplyResources(Me.BarComboCert, "BarComboCert")
        Me.BarComboCert.CaptionToEditorIndent = 5
        Me.BarComboCert.Edit = Me.ComboCert
        Me.BarComboCert.Id = 11
        Me.BarComboCert.Name = "BarComboCert"
        '
        'ComboCert
        '
        resources.ApplyResources(Me.ComboCert, "ComboCert")
        Me.ComboCert.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("ComboCert.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.ComboCert.Name = "ComboCert"
        Me.ComboCert.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        '
        'BtnRemoveSignatures
        '
        resources.ApplyResources(Me.BtnRemoveSignatures, "BtnRemoveSignatures")
        Me.BtnRemoveSignatures.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnRemoveSignatures.Id = 13
        Me.BtnRemoveSignatures.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnRemoveSignatures.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnRemoveSignatures.Name = "BtnRemoveSignatures"
        Me.BtnRemoveSignatures.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnReject
        '
        resources.ApplyResources(Me.BtnReject, "BtnReject")
        Me.BtnReject.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnReject.Id = 14
        Me.BtnReject.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnReject.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnReject.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F5)
        Me.BtnReject.Name = "BtnReject"
        Me.BtnReject.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnUnreject
        '
        resources.ApplyResources(Me.BtnUnreject, "BtnUnreject")
        Me.BtnUnreject.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnUnreject.Id = 15
        Me.BtnUnreject.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnUnreject.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnUnreject.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F6)
        Me.BtnUnreject.Name = "BtnUnreject"
        Me.BtnUnreject.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnRotateLeft
        '
        resources.ApplyResources(Me.BtnRotateLeft, "BtnRotateLeft")
        Me.BtnRotateLeft.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnRotateLeft.Id = 16
        Me.BtnRotateLeft.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnRotateLeft.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnRotateLeft.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Oem4))
        Me.BtnRotateLeft.Name = "BtnRotateLeft"
        Me.BtnRotateLeft.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnRotateRight
        '
        resources.ApplyResources(Me.BtnRotateRight, "BtnRotateRight")
        Me.BtnRotateRight.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnRotateRight.Id = 17
        Me.BtnRotateRight.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnRotateRight.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnRotateRight.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Oem6))
        Me.BtnRotateRight.Name = "BtnRotateRight"
        Me.BtnRotateRight.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnDocFirst
        '
        resources.ApplyResources(Me.BtnDocFirst, "BtnDocFirst")
        Me.BtnDocFirst.Id = 18
        Me.BtnDocFirst.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnDocFirst.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnDocFirst.Name = "BtnDocFirst"
        Me.BtnDocFirst.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnDocPrevious
        '
        resources.ApplyResources(Me.BtnDocPrevious, "BtnDocPrevious")
        Me.BtnDocPrevious.Id = 19
        Me.BtnDocPrevious.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnDocPrevious.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnDocPrevious.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F3)
        Me.BtnDocPrevious.Name = "BtnDocPrevious"
        Me.BtnDocPrevious.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnDocNext
        '
        resources.ApplyResources(Me.BtnDocNext, "BtnDocNext")
        Me.BtnDocNext.Id = 20
        Me.BtnDocNext.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnDocNext.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnDocNext.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F4)
        Me.BtnDocNext.Name = "BtnDocNext"
        Me.BtnDocNext.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnDocLast
        '
        resources.ApplyResources(Me.BtnDocLast, "BtnDocLast")
        Me.BtnDocLast.Id = 21
        Me.BtnDocLast.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnDocLast.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnDocLast.Name = "BtnDocLast"
        Me.BtnDocLast.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnPagePrevious
        '
        resources.ApplyResources(Me.BtnPagePrevious, "BtnPagePrevious")
        Me.BtnPagePrevious.Id = 22
        Me.BtnPagePrevious.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnPagePrevious.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnPagePrevious.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F7)
        Me.BtnPagePrevious.Name = "BtnPagePrevious"
        Me.BtnPagePrevious.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnPageNext
        '
        resources.ApplyResources(Me.BtnPageNext, "BtnPageNext")
        Me.BtnPageNext.Id = 23
        Me.BtnPageNext.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnPageNext.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnPageNext.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F8)
        Me.BtnPageNext.Name = "BtnPageNext"
        Me.BtnPageNext.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnZoomWhole
        '
        resources.ApplyResources(Me.BtnZoomWhole, "BtnZoomWhole")
        Me.BtnZoomWhole.Id = 26
        Me.BtnZoomWhole.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnZoomWhole.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnZoomWhole.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D0))
        Me.BtnZoomWhole.Name = "BtnZoomWhole"
        Me.BtnZoomWhole.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnZoomWidth
        '
        resources.ApplyResources(Me.BtnZoomWidth, "BtnZoomWidth")
        Me.BtnZoomWidth.Id = 27
        Me.BtnZoomWidth.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnZoomWidth.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnZoomWidth.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D1))
        Me.BtnZoomWidth.Name = "BtnZoomWidth"
        Me.BtnZoomWidth.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnZoomIn
        '
        resources.ApplyResources(Me.BtnZoomIn, "BtnZoomIn")
        Me.BtnZoomIn.Id = 28
        Me.BtnZoomIn.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnZoomIn.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnZoomIn.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Add))
        Me.BtnZoomIn.Name = "BtnZoomIn"
        Me.BtnZoomIn.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnZoomOut
        '
        resources.ApplyResources(Me.BtnZoomOut, "BtnZoomOut")
        Me.BtnZoomOut.Id = 29
        Me.BtnZoomOut.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnZoomOut.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnZoomOut.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Subtract))
        Me.BtnZoomOut.Name = "BtnZoomOut"
        Me.BtnZoomOut.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BarLabelUser
        '
        Me.BarLabelUser.Id = 1
        Me.BarLabelUser.Name = "BarLabelUser"
        '
        'BtnDeletePage
        '
        resources.ApplyResources(Me.BtnDeletePage, "BtnDeletePage")
        Me.BtnDeletePage.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnDeletePage.Id = 7
        Me.BtnDeletePage.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnDeletePage.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnDeletePage.ItemShortcut = New DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.Delete)
        Me.BtnDeletePage.Name = "BtnDeletePage"
        Me.BtnDeletePage.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnAbout
        '
        resources.ApplyResources(Me.BtnAbout, "BtnAbout")
        Me.BtnAbout.Id = 11
        Me.BtnAbout.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnAbout.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnAbout.Name = "BtnAbout"
        Me.BtnAbout.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnZoom1on1
        '
        resources.ApplyResources(Me.BtnZoom1on1, "BtnZoom1on1")
        Me.BtnZoom1on1.Id = 19
        Me.BtnZoom1on1.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnZoom1on1.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnZoom1on1.ItemShortcut = New DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.D2))
        Me.BtnZoom1on1.Name = "BtnZoom1on1"
        '
        'BtnRefreshCertificates
        '
        resources.ApplyResources(Me.BtnRefreshCertificates, "BtnRefreshCertificates")
        Me.BtnRefreshCertificates.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnRefreshCertificates.Id = 20
        Me.BtnRefreshCertificates.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnRefreshCertificates.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnRefreshCertificates.Name = "BtnRefreshCertificates"
        Me.BtnRefreshCertificates.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large
        '
        'BtnViewOriginal
        '
        resources.ApplyResources(Me.BtnViewOriginal, "BtnViewOriginal")
        Me.BtnViewOriginal.Id = 27
        Me.BtnViewOriginal.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnViewOriginal.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnViewOriginal.Name = "BtnViewOriginal"
        '
        'BtnViewSigned
        '
        resources.ApplyResources(Me.BtnViewSigned, "BtnViewSigned")
        Me.BtnViewSigned.Id = 28
        Me.BtnViewSigned.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnViewSigned.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnViewSigned.Name = "BtnViewSigned"
        '
        'BtnRegulation
        '
        resources.ApplyResources(Me.BtnRegulation, "BtnRegulation")
        Me.BtnRegulation.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnRegulation.Id = 34
        Me.BtnRegulation.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnRegulation.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnRegulation.Name = "BtnRegulation"
        '
        'BtnLayoutSingle
        '
        resources.ApplyResources(Me.BtnLayoutSingle, "BtnLayoutSingle")
        Me.BtnLayoutSingle.Id = 38
        Me.BtnLayoutSingle.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnLayoutSingle.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnLayoutSingle.Name = "BtnLayoutSingle"
        '
        'BtnLayoutDouble
        '
        resources.ApplyResources(Me.BtnLayoutDouble, "BtnLayoutDouble")
        Me.BtnLayoutDouble.Id = 39
        Me.BtnLayoutDouble.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnLayoutDouble.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnLayoutDouble.Name = "BtnLayoutDouble"
        '
        'BarWorkspaceMenuItem1
        '
        resources.ApplyResources(Me.BarWorkspaceMenuItem1, "BarWorkspaceMenuItem1")
        Me.BarWorkspaceMenuItem1.Id = 41
        Me.BarWorkspaceMenuItem1.Name = "BarWorkspaceMenuItem1"
        Me.BarWorkspaceMenuItem1.WorkspaceManager = Me.WsManager
        '
        'WsManager
        '
        Me.WsManager.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.[False]
        Me.WsManager.TargetControl = Me
        Me.WsManager.TransitionType = PushTransition1
        '
        'BtnWorkspaceReset
        '
        resources.ApplyResources(Me.BtnWorkspaceReset, "BtnWorkspaceReset")
        Me.BtnWorkspaceReset.Id = 42
        Me.BtnWorkspaceReset.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnWorkspaceReset.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnWorkspaceReset.Name = "BtnWorkspaceReset"
        '
        'BtnSkipSign
        '
        resources.ApplyResources(Me.BtnSkipSign, "BtnSkipSign")
        Me.BtnSkipSign.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BtnSkipSign.Id = 43
        Me.BtnSkipSign.ImageOptions.LargeImageIndex = CType(resources.GetObject("BtnSkipSign.ImageOptions.LargeImageIndex"), Integer)
        Me.BtnSkipSign.Name = "BtnSkipSign"
        '
        'BarComboProvider
        '
        resources.ApplyResources(Me.BarComboProvider, "BarComboProvider")
        Me.BarComboProvider.CaptionToEditorIndent = 5
        Me.BarComboProvider.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.DocumentItemBindingSource, "Available", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.BarComboProvider.Edit = Me.ComboProvider
        Me.BarComboProvider.Id = 44
        Me.BarComboProvider.Name = "BarComboProvider"
        '
        'ComboProvider
        '
        resources.ApplyResources(Me.ComboProvider, "ComboProvider")
        Me.ComboProvider.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("ComboProvider.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.ComboProvider.Name = "ComboProvider"
        Me.ComboProvider.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        '
        'LargeRibbonImages
        '
        resources.ApplyResources(Me.LargeRibbonImages, "LargeRibbonImages")
        Me.LargeRibbonImages.ImageStream = CType(resources.GetObject("LargeRibbonImages.ImageStream"), DevExpress.Utils.ImageCollectionStreamer)
        Me.LargeRibbonImages.Images.SetKeyName(0, "BatchClose.png")
        Me.LargeRibbonImages.Images.SetKeyName(1, "BatchOpen.png")
        Me.LargeRibbonImages.Images.SetKeyName(2, "BatchSuspend.png")
        Me.LargeRibbonImages.Images.SetKeyName(3, "RemoveSignature.png")
        Me.LargeRibbonImages.Images.SetKeyName(4, "Sign.png")
        Me.LargeRibbonImages.Images.SetKeyName(5, "Reject.png")
        Me.LargeRibbonImages.Images.SetKeyName(6, "RemoveSignature.png")
        Me.LargeRibbonImages.Images.SetKeyName(7, "RotateLeft.png")
        Me.LargeRibbonImages.Images.SetKeyName(8, "RotateRight.png")
        Me.LargeRibbonImages.Images.SetKeyName(9, "Unreject.png")
        Me.LargeRibbonImages.Images.SetKeyName(10, "ZoomIn.png")
        Me.LargeRibbonImages.Images.SetKeyName(11, "ZoomOut.png")
        Me.LargeRibbonImages.Images.SetKeyName(12, "ZoomWhole.png")
        Me.LargeRibbonImages.Images.SetKeyName(13, "ZoomWidth.png")
        Me.LargeRibbonImages.Images.SetKeyName(14, "DocumentFirst.png")
        Me.LargeRibbonImages.Images.SetKeyName(15, "DocumentLast.png")
        Me.LargeRibbonImages.Images.SetKeyName(16, "DocumentNext.png")
        Me.LargeRibbonImages.Images.SetKeyName(17, "DocumentPrevious.png")
        Me.LargeRibbonImages.Images.SetKeyName(18, "PageNext.png")
        Me.LargeRibbonImages.Images.SetKeyName(19, "PagePrevious.png")
        Me.LargeRibbonImages.Images.SetKeyName(20, "Delete.png")
        Me.LargeRibbonImages.Images.SetKeyName(21, "Info.png")
        Me.LargeRibbonImages.Images.SetKeyName(22, "Zoom1on1.png")
        Me.LargeRibbonImages.Images.SetKeyName(23, "Refresh.png")
        Me.LargeRibbonImages.Images.SetKeyName(24, "ViewDocument.png")
        Me.LargeRibbonImages.Images.SetKeyName(25, "ViewSignedDocument.png")
        Me.LargeRibbonImages.Images.SetKeyName(26, "RuleBook.png")
        Me.LargeRibbonImages.Images.SetKeyName(27, "LayoutSinglePage.png")
        Me.LargeRibbonImages.Images.SetKeyName(28, "LayoutDoublePage.png")
        Me.LargeRibbonImages.Images.SetKeyName(29, "ResetLayout.png")
        Me.LargeRibbonImages.Images.SetKeyName(30, "SkipSigning.png")
        '
        'RibbonPageHome
        '
        Me.RibbonPageHome.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroupBatch, Me.RibbonPageGroupSignature, Me.RibbonPageGroupView, Me.RibbonPageGroupReject, Me.RibbonPageGroupEdit, Me.RibbonPageGroupAbout})
        Me.RibbonPageHome.Name = "RibbonPageHome"
        resources.ApplyResources(Me.RibbonPageHome, "RibbonPageHome")
        '
        'RibbonPageGroupBatch
        '
        Me.RibbonPageGroupBatch.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupBatch.ItemLinks.Add(Me.BtnBatchOpen)
        Me.RibbonPageGroupBatch.ItemLinks.Add(Me.BtnBatchClose)
        Me.RibbonPageGroupBatch.ItemLinks.Add(Me.BtnBatchSuspend)
        Me.RibbonPageGroupBatch.Name = "RibbonPageGroupBatch"
        resources.ApplyResources(Me.RibbonPageGroupBatch, "RibbonPageGroupBatch")
        '
        'RibbonPageGroupSignature
        '
        Me.RibbonPageGroupSignature.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BtnSign)
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BtnRemoveSignatures)
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BtnSkipSign)
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BarComboProvider)
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BarComboCert)
        Me.RibbonPageGroupSignature.ItemLinks.Add(Me.BtnRefreshCertificates)
        Me.RibbonPageGroupSignature.Name = "RibbonPageGroupSignature"
        resources.ApplyResources(Me.RibbonPageGroupSignature, "RibbonPageGroupSignature")
        '
        'RibbonPageGroupView
        '
        Me.RibbonPageGroupView.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupView.ItemLinks.Add(Me.BtnViewOriginal)
        Me.RibbonPageGroupView.ItemLinks.Add(Me.BtnViewSigned)
        Me.RibbonPageGroupView.Name = "RibbonPageGroupView"
        resources.ApplyResources(Me.RibbonPageGroupView, "RibbonPageGroupView")
        '
        'RibbonPageGroupReject
        '
        Me.RibbonPageGroupReject.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupReject.ItemLinks.Add(Me.BtnReject)
        Me.RibbonPageGroupReject.ItemLinks.Add(Me.BtnUnreject)
        Me.RibbonPageGroupReject.Name = "RibbonPageGroupReject"
        resources.ApplyResources(Me.RibbonPageGroupReject, "RibbonPageGroupReject")
        '
        'RibbonPageGroupEdit
        '
        Me.RibbonPageGroupEdit.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupEdit.ItemLinks.Add(Me.BtnRotateLeft)
        Me.RibbonPageGroupEdit.ItemLinks.Add(Me.BtnRotateRight)
        Me.RibbonPageGroupEdit.ItemLinks.Add(Me.BtnDeletePage)
        Me.RibbonPageGroupEdit.Name = "RibbonPageGroupEdit"
        resources.ApplyResources(Me.RibbonPageGroupEdit, "RibbonPageGroupEdit")
        Me.RibbonPageGroupEdit.Visible = False
        '
        'RibbonPageGroupAbout
        '
        Me.RibbonPageGroupAbout.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupAbout.ItemLinks.Add(Me.BtnRegulation)
        Me.RibbonPageGroupAbout.ItemLinks.Add(Me.BtnAbout)
        Me.RibbonPageGroupAbout.Name = "RibbonPageGroupAbout"
        resources.ApplyResources(Me.RibbonPageGroupAbout, "RibbonPageGroupAbout")
        '
        'RibbonPageNavigation
        '
        Me.RibbonPageNavigation.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroupNavigationPage, Me.RibbonPageGroupNavigationDocument})
        Me.RibbonPageNavigation.Name = "RibbonPageNavigation"
        resources.ApplyResources(Me.RibbonPageNavigation, "RibbonPageNavigation")
        '
        'RibbonPageGroupNavigationPage
        '
        Me.RibbonPageGroupNavigationPage.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupNavigationPage.ItemLinks.Add(Me.BtnPagePrevious)
        Me.RibbonPageGroupNavigationPage.ItemLinks.Add(Me.BtnPageNext)
        Me.RibbonPageGroupNavigationPage.Name = "RibbonPageGroupNavigationPage"
        resources.ApplyResources(Me.RibbonPageGroupNavigationPage, "RibbonPageGroupNavigationPage")
        '
        'RibbonPageGroupNavigationDocument
        '
        Me.RibbonPageGroupNavigationDocument.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupNavigationDocument.ItemLinks.Add(Me.BtnDocPrevious)
        Me.RibbonPageGroupNavigationDocument.ItemLinks.Add(Me.BtnDocNext)
        Me.RibbonPageGroupNavigationDocument.ItemLinks.Add(Me.BtnDocFirst)
        Me.RibbonPageGroupNavigationDocument.ItemLinks.Add(Me.BtnDocLast)
        Me.RibbonPageGroupNavigationDocument.Name = "RibbonPageGroupNavigationDocument"
        resources.ApplyResources(Me.RibbonPageGroupNavigationDocument, "RibbonPageGroupNavigationDocument")
        '
        'RibbonPageView
        '
        Me.RibbonPageView.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.RibbonPageGroupZoom, Me.RibbonPageGroupLayout, Me.RibbonPageGroupWorkspace})
        Me.RibbonPageView.Name = "RibbonPageView"
        resources.ApplyResources(Me.RibbonPageView, "RibbonPageView")
        '
        'RibbonPageGroupZoom
        '
        Me.RibbonPageGroupZoom.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.[False]
        Me.RibbonPageGroupZoom.ItemLinks.Add(Me.BtnZoomWhole)
        Me.RibbonPageGroupZoom.ItemLinks.Add(Me.BtnZoomWidth)
        Me.RibbonPageGroupZoom.ItemLinks.Add(Me.BtnZoom1on1)
        Me.RibbonPageGroupZoom.ItemLinks.Add(Me.BtnZoomIn)
        Me.RibbonPageGroupZoom.ItemLinks.Add(Me.BtnZoomOut)
        Me.RibbonPageGroupZoom.Name = "RibbonPageGroupZoom"
        resources.ApplyResources(Me.RibbonPageGroupZoom, "RibbonPageGroupZoom")
        '
        'RibbonPageGroupLayout
        '
        Me.RibbonPageGroupLayout.ItemLinks.Add(Me.BtnLayoutSingle)
        Me.RibbonPageGroupLayout.ItemLinks.Add(Me.BtnLayoutDouble)
        Me.RibbonPageGroupLayout.Name = "RibbonPageGroupLayout"
        resources.ApplyResources(Me.RibbonPageGroupLayout, "RibbonPageGroupLayout")
        '
        'RibbonPageGroupWorkspace
        '
        Me.RibbonPageGroupWorkspace.ItemLinks.Add(Me.BtnWorkspaceReset)
        Me.RibbonPageGroupWorkspace.Name = "RibbonPageGroupWorkspace"
        resources.ApplyResources(Me.RibbonPageGroupWorkspace, "RibbonPageGroupWorkspace")
        '
        'ProgressBar
        '
        Me.ProgressBar.Name = "ProgressBar"
        Me.ProgressBar.Step = 1
        '
        'RibbonStatusBar
        '
        Me.RibbonStatusBar.ItemLinks.Add(Me.BarLabelUser)
        resources.ApplyResources(Me.RibbonStatusBar, "RibbonStatusBar")
        Me.RibbonStatusBar.Name = "RibbonStatusBar"
        Me.RibbonStatusBar.Ribbon = Me.RibbonControl
        '
        'TabViewer
        '
        Me.TabViewer.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.TabViewer.Appearance.Options.UseBackColor = True
        resources.ApplyResources(Me.TabViewer, "TabViewer")
        Me.TabViewer.Name = "TabViewer"
        Me.TabViewer.SelectedTabPage = Me.TabOriginal
        Me.TabViewer.TabPages.AddRange(New DevExpress.XtraTab.XtraTabPage() {Me.TabOriginal, Me.TabSigned, Me.TabClause, Me.TabLog})
        '
        'TabOriginal
        '
        Me.TabOriginal.Appearance.PageClient.BackColor = System.Drawing.Color.Lime
        Me.TabOriginal.Appearance.PageClient.Options.UseBackColor = True
        Me.TabOriginal.Controls.Add(Me.PdfViewerOriginal)
        Me.TabOriginal.Name = "TabOriginal"
        resources.ApplyResources(Me.TabOriginal, "TabOriginal")
        '
        'PdfViewerOriginal
        '
        Me.PdfViewerOriginal.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        resources.ApplyResources(Me.PdfViewerOriginal, "PdfViewerOriginal")
        Me.PdfViewerOriginal.Name = "PdfViewerOriginal"
        Me.PdfViewerOriginal.ViewerDocType = PDFSignerCommon.DocumentType.UnsignedDocument
        '
        'TabSigned
        '
        Me.TabSigned.Appearance.PageClient.BackColor = System.Drawing.Color.DimGray
        Me.TabSigned.Appearance.PageClient.Options.UseBackColor = True
        Me.TabSigned.Controls.Add(Me.PdfViewerSigned)
        Me.TabSigned.Name = "TabSigned"
        resources.ApplyResources(Me.TabSigned, "TabSigned")
        '
        'PdfViewerSigned
        '
        Me.PdfViewerSigned.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        resources.ApplyResources(Me.PdfViewerSigned, "PdfViewerSigned")
        Me.PdfViewerSigned.Name = "PdfViewerSigned"
        Me.PdfViewerSigned.ViewerDocType = PDFSignerCommon.DocumentType.SignedDocument
        '
        'TabClause
        '
        Me.TabClause.Controls.Add(Me.MemoClause)
        Me.TabClause.Name = "TabClause"
        resources.ApplyResources(Me.TabClause, "TabClause")
        '
        'MemoClause
        '
        Me.MemoClause.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DocumentItemBindingSource, "ClauseContent", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.MemoClause, "MemoClause")
        Me.MemoClause.MenuManager = Me.RibbonControl
        Me.MemoClause.Name = "MemoClause"
        Me.MemoClause.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.MemoClause.Properties.Appearance.Font = CType(resources.GetObject("MemoClause.Properties.Appearance.Font"), System.Drawing.Font)
        Me.MemoClause.Properties.Appearance.ForeColor = System.Drawing.Color.Navy
        Me.MemoClause.Properties.Appearance.Options.UseBackColor = True
        Me.MemoClause.Properties.Appearance.Options.UseFont = True
        Me.MemoClause.Properties.Appearance.Options.UseForeColor = True
        Me.MemoClause.Properties.ReadOnly = True
        Me.MemoClause.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.MemoClause.Properties.WordWrap = False
        '
        'TabLog
        '
        Me.TabLog.Controls.Add(Me.MemoLog)
        Me.TabLog.Name = "TabLog"
        resources.ApplyResources(Me.TabLog, "TabLog")
        '
        'MemoLog
        '
        Me.MemoLog.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.DocumentItemBindingSource, "SignatureLogContent", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.MemoLog, "MemoLog")
        Me.MemoLog.MenuManager = Me.RibbonControl
        Me.MemoLog.Name = "MemoLog"
        Me.MemoLog.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.MemoLog.Properties.Appearance.Font = CType(resources.GetObject("MemoLog.Properties.Appearance.Font"), System.Drawing.Font)
        Me.MemoLog.Properties.Appearance.ForeColor = System.Drawing.Color.Navy
        Me.MemoLog.Properties.Appearance.Options.UseBackColor = True
        Me.MemoLog.Properties.Appearance.Options.UseFont = True
        Me.MemoLog.Properties.Appearance.Options.UseForeColor = True
        Me.MemoLog.Properties.ReadOnly = True
        Me.MemoLog.Properties.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.MemoLog.Properties.WordWrap = False
        '
        'LblFileNotFound
        '
        resources.ApplyResources(Me.LblFileNotFound, "LblFileNotFound")
        Me.LblFileNotFound.Name = "LblFileNotFound"
        '
        'DefaultBarAndDockingController
        '
        Me.DefaultBarAndDockingController.Controller.LookAndFeel.SkinName = "Office 2010 Blue"
        Me.DefaultBarAndDockingController.Controller.LookAndFeel.UseDefaultLookAndFeel = False
        Me.DefaultBarAndDockingController.Controller.PropertiesDocking.ViewStyle = DevExpress.XtraBars.Docking2010.Views.DockingViewStyle.Classic
        '
        'BarEditItem1
        '
        resources.ApplyResources(Me.BarEditItem1, "BarEditItem1")
        Me.BarEditItem1.Edit = Me.ComboCert
        Me.BarEditItem1.Id = 11
        Me.BarEditItem1.Name = "BarEditItem1"
        '
        'BarEditItem2
        '
        resources.ApplyResources(Me.BarEditItem2, "BarEditItem2")
        Me.BarEditItem2.Edit = Me.ComboCert
        Me.BarEditItem2.Id = 11
        Me.BarEditItem2.Name = "BarEditItem2"
        '
        'FrmPdfSigner
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SplitContainerControl1)
        Me.Controls.Add(Me.RibbonStatusBar)
        Me.Controls.Add(Me.RibbonControl)
        Me.IconOptions.Icon = CType(resources.GetObject("FrmPdfSigner.IconOptions.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Name = "FrmPdfSigner"
        Me.Ribbon = Me.RibbonControl
        Me.RibbonVisibility = DevExpress.XtraBars.Ribbon.RibbonVisibility.Visible
        Me.StatusBar = Me.RibbonStatusBar
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        CType(Me.SplitContainerControl1.Panel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.Panel1.ResumeLayout(False)
        CType(Me.SplitContainerControl1.Panel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerControl1.ResumeLayout(False)
        CType(Me.PanelControl2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl2.ResumeLayout(False)
        CType(Me.BatchTree, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BatchTreeProgressBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BatchContentsImages, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.GrpPartialCopy, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GrpPartialCopy.ResumeLayout(False)
        CType(Me.ComboPartialCopy.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DocumentItemBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RibbonControl, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ComboCert, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ComboProvider, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.LargeRibbonImages, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProgressBar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TabViewer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabViewer.ResumeLayout(False)
        Me.TabOriginal.ResumeLayout(False)
        Me.TabSigned.ResumeLayout(False)
        Me.TabClause.ResumeLayout(False)
        CType(Me.MemoClause.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabLog.ResumeLayout(False)
        CType(Me.MemoLog.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DefaultBarAndDockingController.Controller, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents RibbonPageGroupSignature As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnBatchOpen As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnBatchClose As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnBatchSuspend As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageHome As DevExpress.XtraBars.Ribbon.RibbonPage
    Private WithEvents RibbonPageGroupBatch As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnSign As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonControl As DevExpress.XtraBars.Ribbon.RibbonControl
    Private WithEvents BarComboCert As DevExpress.XtraBars.BarEditItem
    Private WithEvents BtnRemoveSignatures As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageNavigation As DevExpress.XtraBars.Ribbon.RibbonPage
    Private WithEvents RibbonPageGroupNavigationDocument As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnReject As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnUnreject As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupReject As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnRotateLeft As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnRotateRight As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupEdit As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnDocFirst As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnDocPrevious As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnDocNext As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnDocLast As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupNavigationPage As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents DefaultBarAndDockingController As DevExpress.XtraBars.DefaultBarAndDockingController
    Private WithEvents BtnPagePrevious As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnPageNext As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageView As DevExpress.XtraBars.Ribbon.RibbonPage
    Private WithEvents ComboCert As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    Private WithEvents RibbonPageGroupZoom As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnZoomWhole As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnZoomWidth As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnZoomIn As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnZoomOut As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnDeletePage As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BatchContentsImages As DevExpress.Utils.ImageCollection
    Private WithEvents BatchTree As DevExpress.XtraTreeList.TreeList
    Private WithEvents PanelControl2 As DevExpress.XtraEditors.PanelControl
    Private WithEvents BarLabelUser As DevExpress.XtraBars.BarStaticItem
    Private WithEvents ProgressBar As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Private WithEvents ColBatchItem As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents BtnAbout As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupAbout As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BtnZoom1on1 As DevExpress.XtraBars.BarButtonItem
    Private WithEvents TabViewer As DevExpress.XtraTab.XtraTabControl
    Private WithEvents TabSigned As DevExpress.XtraTab.XtraTabPage
    Private WithEvents TabOriginal As DevExpress.XtraTab.XtraTabPage
    Private WithEvents TabClause As DevExpress.XtraTab.XtraTabPage
    Private WithEvents TabLog As DevExpress.XtraTab.XtraTabPage
    Private WithEvents MemoClause As DevExpress.XtraEditors.MemoEdit
    Private WithEvents MemoLog As DevExpress.XtraEditors.MemoEdit
    Private WithEvents BtnViewOriginal As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnViewSigned As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupView As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents LblFileNotFound As System.Windows.Forms.Label
    Private WithEvents GrpPartialCopy As DevExpress.XtraEditors.GroupControl
    Private WithEvents ComboPartialCopy As DevExpress.XtraEditors.ComboBoxEdit
    Private WithEvents BtnRegulation As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnRefreshCertificates As DevExpress.XtraBars.BarButtonItem
    Private WithEvents ColProgressBar As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents BatchTreeProgressBar As DevExpress.XtraEditors.Repository.RepositoryItemProgressBar
    Private WithEvents RibbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
    Private WithEvents BatchTreeToolTip As DevExpress.Utils.ToolTipController
    Private WithEvents BtnLayoutSingle As DevExpress.XtraBars.BarButtonItem
    Private WithEvents BtnLayoutDouble As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupLayout As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents BarWorkspaceMenuItem1 As DevExpress.XtraBars.BarWorkspaceMenuItem
    Private WithEvents WsManager As DevExpress.Utils.WorkspaceManager
    Private WithEvents BtnWorkspaceReset As DevExpress.XtraBars.BarButtonItem
    Private WithEvents RibbonPageGroupWorkspace As DevExpress.XtraBars.Ribbon.RibbonPageGroup
    Private WithEvents LargeRibbonImages As DevExpress.Utils.ImageCollection
    Private WithEvents DocumentItemBindingSource As System.Windows.Forms.BindingSource
    Private WithEvents PdfViewerOriginal As PDFSigner.ViewerBase
    Private WithEvents PdfViewerSigned As PDFSigner.ViewerBase
    Private WithEvents BtnSkipSign As DevExpress.XtraBars.BarButtonItem
    Friend WithEvents ComboProvider As DevExpress.XtraEditors.Repository.RepositoryItemComboBox
    Private WithEvents BarEditItem1 As DevExpress.XtraBars.BarEditItem
    Private WithEvents BarEditItem2 As DevExpress.XtraBars.BarEditItem
    Private WithEvents BarComboProvider As DevExpress.XtraBars.BarEditItem
End Class
