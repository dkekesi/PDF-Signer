<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TallPDFViewer
    Inherits PDFSigner.ViewerBase

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TallPDFViewer))
        Dim Spacing1 As TallComponents.PDF.Spacing = New TallComponents.PDF.Spacing()
        Dim Spacing2 As TallComponents.PDF.Spacing = New TallComponents.PDF.Spacing()
        Me.SplitThumbViewer = New DevExpress.XtraEditors.SplitContainerControl()
        Me.SplitAttachment = New DevExpress.XtraEditors.SplitContainerControl()
        Me.PageViewer = New TallComponents.Interaction.WinForms.Controls.StandardPagesViewer()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.ListAttachments = New DevExpress.XtraTreeList.TreeList()
        Me.FileName = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.FileSize = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.Delete = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.BtnAddAttachment = New DevExpress.XtraEditors.SimpleButton()
        Me.ThumbnailViewer = New TallComponents.Interaction.WinForms.Controls.ThumbnailsViewer()
        CType(Me.SplitThumbViewer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitThumbViewer.SuspendLayout()
        CType(Me.SplitAttachment, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitAttachment.SuspendLayout()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.ListAttachments, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitThumbViewer
        '
        Me.SplitThumbViewer.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        resources.ApplyResources(Me.SplitThumbViewer, "SplitThumbViewer")
        Me.SplitThumbViewer.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2
        Me.SplitThumbViewer.Name = "SplitThumbViewer"
        Me.SplitThumbViewer.Panel1.Controls.Add(Me.SplitAttachment)
        Me.SplitThumbViewer.Panel1.MinSize = 400
        resources.ApplyResources(Me.SplitThumbViewer.Panel1, "SplitThumbViewer.SplitThumbViewer_Panel1")
        Me.SplitThumbViewer.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitThumbViewer.Panel2.Controls.Add(Me.ThumbnailViewer)
        Me.SplitThumbViewer.Panel2.MinSize = 120
        resources.ApplyResources(Me.SplitThumbViewer.Panel2, "SplitThumbViewer.SplitThumbViewer_Panel2")
        Me.SplitThumbViewer.SplitterPosition = 200
        '
        'SplitAttachment
        '
        Me.SplitAttachment.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        resources.ApplyResources(Me.SplitAttachment, "SplitAttachment")
        Me.SplitAttachment.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2
        Me.SplitAttachment.Horizontal = False
        Me.SplitAttachment.Name = "SplitAttachment"
        Me.SplitAttachment.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitAttachment.Panel1.Controls.Add(Me.PageViewer)
        Me.SplitAttachment.Panel1.MinSize = 150
        resources.ApplyResources(Me.SplitAttachment.Panel1, "SplitAttachment.SplitAttachment_Panel1")
        Me.SplitAttachment.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitAttachment.Panel2.Controls.Add(Me.PanelControl1)
        Me.SplitAttachment.Panel2.MinSize = 60
        resources.ApplyResources(Me.SplitAttachment.Panel2, "SplitAttachment.SplitAttachment_Panel2")
        Me.SplitAttachment.SplitterPosition = 63
        '
        'PageViewer
        '
        Me.PageViewer.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.PageViewer.CaretBlinkTime = 530
        Me.PageViewer.CurrentPageIndex = -1
        Me.PageViewer.CursorMode = TallComponents.Interaction.WinForms.CursorMode.Normal
        resources.ApplyResources(Me.PageViewer, "PageViewer")
        Me.PageViewer.Document = Nothing
        Spacing1.Bottom = 4.0R
        Spacing1.Left = 4.0R
        Spacing1.Right = 4.0R
        Spacing1.Top = 4.0R
        Me.PageViewer.DocumentMargin = Spacing1
        Me.PageViewer.EnableAnnotations = True
        Me.PageViewer.HideHorizontalScrollbar = False
        Me.PageViewer.HideVerticalScrollbar = False
        Me.PageViewer.HighlightAnnotations = False
        Me.PageViewer.HighlightFields = False
        Me.PageViewer.HorizontalAlignment = TallComponents.PDF.HorizontalAlignment.Left
        Me.PageViewer.HoverCursor = Nothing
        Me.PageViewer.MaxZoom = 64.0R
        Me.PageViewer.MinZoom = 0.125R
        Me.PageViewer.Name = "PageViewer"
        Me.PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.TopToBottom
        Spacing2.Bottom = 6.0R
        Spacing2.Left = 6.0R
        Spacing2.Right = 6.0R
        Spacing2.Top = 6.0R
        Me.PageViewer.PageSpacing = Spacing2
        Me.PageViewer.ShowOverflowMarker = True
        Me.PageViewer.TextSelectMode = TallComponents.Interaction.WinForms.TextSelectMode.SelectWholeWords
        Me.PageViewer.VerticalAlignment = TallComponents.PDF.VerticalAlignment.Top
        Me.PageViewer.VisibleLeft = 0.0R
        Me.PageViewer.VisibleTop = 0.0R
        Me.PageViewer.ZoomFactor = 1.0R
        Me.PageViewer.ZoomMode = TallComponents.Interaction.WinForms.ZoomMode.ActualSize
        '
        'PanelControl1
        '
        Me.PanelControl1.Appearance.BackColor = CType(resources.GetObject("PanelControl1.Appearance.BackColor"), System.Drawing.Color)
        Me.PanelControl1.Appearance.Options.UseBackColor = True
        Me.PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl1.Controls.Add(Me.ListAttachments)
        Me.PanelControl1.Controls.Add(Me.BtnAddAttachment)
        resources.ApplyResources(Me.PanelControl1, "PanelControl1")
        Me.PanelControl1.Name = "PanelControl1"
        '
        'ListAttachments
        '
        resources.ApplyResources(Me.ListAttachments, "ListAttachments")
        Me.ListAttachments.Appearance.Empty.BackColor = CType(resources.GetObject("ListAttachments.Appearance.Empty.BackColor"), System.Drawing.Color)
        Me.ListAttachments.Appearance.Empty.Options.UseBackColor = True
        Me.ListAttachments.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.ListAttachments.Columns.AddRange(New DevExpress.XtraTreeList.Columns.TreeListColumn() {Me.FileName, Me.FileSize, Me.Delete})
        Me.ListAttachments.Cursor = System.Windows.Forms.Cursors.Default
        Me.ListAttachments.DataSource = Nothing
        Me.ListAttachments.Name = "ListAttachments"
        Me.ListAttachments.OptionsBehavior.Editable = False
        Me.ListAttachments.OptionsBehavior.ReadOnly = True
        '
        'FileName
        '
        resources.ApplyResources(Me.FileName, "FileName")
        Me.FileName.FieldName = "FileName"
        Me.FileName.Name = "FileName"
        Me.FileName.OptionsColumn.AllowMove = False
        Me.FileName.OptionsColumn.AllowMoveToCustomizationForm = False
        Me.FileName.OptionsColumn.ReadOnly = True
        Me.FileName.OptionsFilter.AllowAutoFilter = False
        Me.FileName.OptionsFilter.AllowFilter = False
        '
        'FileSize
        '
        resources.ApplyResources(Me.FileSize, "FileSize")
        Me.FileSize.FieldName = "FileSize"
        Me.FileSize.Name = "FileSize"
        Me.FileSize.OptionsColumn.AllowMove = False
        Me.FileSize.OptionsColumn.AllowMoveToCustomizationForm = False
        Me.FileSize.OptionsColumn.ReadOnly = True
        Me.FileSize.OptionsFilter.AllowAutoFilter = False
        Me.FileSize.OptionsFilter.AllowFilter = False
        '
        'Delete
        '
        Me.Delete.Name = "Delete"
        Me.Delete.OptionsColumn.AllowMove = False
        Me.Delete.OptionsColumn.AllowMoveToCustomizationForm = False
        Me.Delete.OptionsColumn.AllowSize = False
        Me.Delete.OptionsColumn.FixedWidth = True
        Me.Delete.OptionsColumn.ReadOnly = True
        Me.Delete.OptionsFilter.AllowAutoFilter = False
        Me.Delete.OptionsFilter.AllowFilter = False
        resources.ApplyResources(Me.Delete, "Delete")
        '
        'BtnAddAttachment
        '
        resources.ApplyResources(Me.BtnAddAttachment, "BtnAddAttachment")
        Me.BtnAddAttachment.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.AddAttachment
        Me.BtnAddAttachment.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter
        Me.BtnAddAttachment.Name = "BtnAddAttachment"
        '
        'ThumbnailViewer
        '
        Me.ThumbnailViewer.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.ThumbnailViewer.CaretBlinkTime = 530
        resources.ApplyResources(Me.ThumbnailViewer, "ThumbnailViewer")
        Me.ThumbnailViewer.Document = Nothing
        Me.ThumbnailViewer.DrawPageNumbers = True
        Me.ThumbnailViewer.DrawPagesViewerRectangles = True
        Me.ThumbnailViewer.HideHorizontalScrollbar = True
        Me.ThumbnailViewer.HideVerticalScrollbar = False
        Me.ThumbnailViewer.HorizontalAlignment = TallComponents.PDF.HorizontalAlignment.Left
        Me.ThumbnailViewer.HoverCursor = Nothing
        Me.ThumbnailViewer.MaxZoom = 0.8R
        Me.ThumbnailViewer.MinZoom = 0.02R
        Me.ThumbnailViewer.Name = "ThumbnailViewer"
        Me.ThumbnailViewer.PagesViewer = Me.PageViewer
        Me.ThumbnailViewer.VerticalAlignment = TallComponents.PDF.VerticalAlignment.Top
        Me.ThumbnailViewer.VisibleLeft = 0.0R
        Me.ThumbnailViewer.VisibleTop = 0.0R
        Me.ThumbnailViewer.ZoomFactor = 0.125R
        '
        'PDFViewer
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.Controls.Add(Me.SplitThumbViewer)
        Me.Name = "PDFViewer"
        CType(Me.SplitThumbViewer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitThumbViewer.ResumeLayout(False)
        CType(Me.SplitAttachment, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitAttachment.ResumeLayout(False)
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.ListAttachments, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents SplitThumbViewer As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents PageViewer As TallComponents.Interaction.WinForms.Controls.StandardPagesViewer
    Private WithEvents ThumbnailViewer As TallComponents.Interaction.WinForms.Controls.ThumbnailsViewer
    Private WithEvents SplitAttachment As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents FileName As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents FileSize As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents Delete As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Private WithEvents ListAttachments As DevExpress.XtraTreeList.TreeList
    Private WithEvents BtnAddAttachment As DevExpress.XtraEditors.SimpleButton

End Class
