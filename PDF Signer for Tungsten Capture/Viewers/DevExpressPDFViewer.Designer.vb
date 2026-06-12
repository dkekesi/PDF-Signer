<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DevExpressPDFViewer
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
        Me.SplitAttachment = New DevExpress.XtraEditors.SplitContainerControl()
        Me.PageViewer = New DevExpress.XtraPdfViewer.PdfViewer()
        Me.PanelControl1 = New DevExpress.XtraEditors.PanelControl()
        Me.ListAttachments = New DevExpress.XtraTreeList.TreeList()
        Me.FileName = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.FileSize = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.Delete = New DevExpress.XtraTreeList.Columns.TreeListColumn()
        Me.BtnAddAttachment = New DevExpress.XtraEditors.SimpleButton()
        CType(Me.SplitAttachment, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitAttachment.Panel1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitAttachment.Panel1.SuspendLayout()
        CType(Me.SplitAttachment.Panel2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitAttachment.Panel2.SuspendLayout()
        Me.SplitAttachment.SuspendLayout()
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelControl1.SuspendLayout()
        CType(Me.ListAttachments, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitAttachment
        '
        Me.SplitAttachment.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2
        Me.SplitAttachment.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitAttachment.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2
        Me.SplitAttachment.Horizontal = False
        Me.SplitAttachment.Location = New System.Drawing.Point(0, 0)
        Me.SplitAttachment.Name = "SplitAttachment"
        '
        'SplitAttachment.Panel1
        '
        Me.SplitAttachment.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitAttachment.Panel1.Controls.Add(Me.PageViewer)
        Me.SplitAttachment.Panel1.MinSize = 150
        Me.SplitAttachment.Panel1.Text = "Panel1"
        '
        'SplitAttachment.Panel2
        '
        Me.SplitAttachment.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.SplitAttachment.Panel2.Controls.Add(Me.PanelControl1)
        Me.SplitAttachment.Panel2.MinSize = 60
        Me.SplitAttachment.Panel2.Text = "Panel2"
        Me.SplitAttachment.Size = New System.Drawing.Size(1084, 710)
        Me.SplitAttachment.SplitterPosition = 63
        Me.SplitAttachment.TabIndex = 4
        Me.SplitAttachment.Text = "SplitContainerControl2"
        '
        'PageViewer
        '
        Me.PageViewer.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.PageViewer.Appearance.Options.UseBackColor = True
        Me.PageViewer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PageViewer.Location = New System.Drawing.Point(0, 0)
        Me.PageViewer.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat
        Me.PageViewer.Name = "PageViewer"
        Me.PageViewer.NavigationPaneInitialSelectedPage = DevExpress.XtraPdfViewer.PdfNavigationPanePage.Thumbnails
        Me.PageViewer.NavigationPaneInitialVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Expanded
        Me.PageViewer.NavigationPanePageVisibility = DevExpress.XtraPdfViewer.PdfNavigationPanePageVisibility.Thumbnails
        Me.PageViewer.Size = New System.Drawing.Size(1080, 637)
        Me.PageViewer.TabIndex = 0
        '
        'PanelControl1
        '
        Me.PanelControl1.Appearance.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.PanelControl1.Appearance.Options.UseBackColor = True
        Me.PanelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        Me.PanelControl1.Controls.Add(Me.ListAttachments)
        Me.PanelControl1.Controls.Add(Me.BtnAddAttachment)
        Me.PanelControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelControl1.Location = New System.Drawing.Point(0, 0)
        Me.PanelControl1.Name = "PanelControl1"
        Me.PanelControl1.Size = New System.Drawing.Size(1080, 59)
        Me.PanelControl1.TabIndex = 0
        '
        'ListAttachments
        '
        Me.ListAttachments.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ListAttachments.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.ListAttachments.Appearance.Empty.Options.UseBackColor = True
        Me.ListAttachments.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        Me.ListAttachments.Columns.AddRange(New DevExpress.XtraTreeList.Columns.TreeListColumn() {Me.FileName, Me.FileSize, Me.Delete})
        Me.ListAttachments.Cursor = System.Windows.Forms.Cursors.Default
        Me.ListAttachments.Location = New System.Drawing.Point(55, 0)
        Me.ListAttachments.Name = "ListAttachments"
        Me.ListAttachments.OptionsBehavior.Editable = False
        Me.ListAttachments.OptionsBehavior.ReadOnly = True
        Me.ListAttachments.Size = New System.Drawing.Size(1025, 59)
        Me.ListAttachments.TabIndex = 0
        '
        'FileName
        '
        Me.FileName.Caption = "File name"
        Me.FileName.FieldName = "FileName"
        Me.FileName.MinWidth = 300
        Me.FileName.Name = "FileName"
        Me.FileName.OptionsColumn.AllowMove = False
        Me.FileName.OptionsColumn.AllowMoveToCustomizationForm = False
        Me.FileName.OptionsColumn.ReadOnly = True
        Me.FileName.OptionsFilter.AllowAutoFilter = False
        Me.FileName.OptionsFilter.AllowFilter = False
        Me.FileName.Visible = True
        Me.FileName.VisibleIndex = 0
        Me.FileName.Width = 416
        '
        'FileSize
        '
        Me.FileSize.Caption = "File size"
        Me.FileSize.FieldName = "FileSize"
        Me.FileSize.MinWidth = 100
        Me.FileSize.Name = "FileSize"
        Me.FileSize.OptionsColumn.AllowMove = False
        Me.FileSize.OptionsColumn.AllowMoveToCustomizationForm = False
        Me.FileSize.OptionsColumn.ReadOnly = True
        Me.FileSize.OptionsFilter.AllowAutoFilter = False
        Me.FileSize.OptionsFilter.AllowFilter = False
        Me.FileSize.Visible = True
        Me.FileSize.VisibleIndex = 1
        Me.FileSize.Width = 100
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
        Me.Delete.Visible = True
        Me.Delete.VisibleIndex = 2
        Me.Delete.Width = 24
        '
        'BtnAddAttachment
        '
        Me.BtnAddAttachment.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtnAddAttachment.ImageOptions.Image = Global.PDFSigner.My.Resources.Resources.AddAttachment
        Me.BtnAddAttachment.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter
        Me.BtnAddAttachment.Location = New System.Drawing.Point(0, 0)
        Me.BtnAddAttachment.Name = "BtnAddAttachment"
        Me.BtnAddAttachment.Size = New System.Drawing.Size(53, 59)
        Me.BtnAddAttachment.TabIndex = 1
        Me.BtnAddAttachment.Text = "Add..."
        Me.BtnAddAttachment.ToolTip = "Add attachment"
        '
        'DevExpressPDFViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(221, Byte), Integer), CType(CType(238, Byte), Integer))
        Me.Controls.Add(Me.SplitAttachment)
        Me.Name = "DevExpressPDFViewer"
        Me.Size = New System.Drawing.Size(1084, 710)
        CType(Me.SplitAttachment.Panel1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitAttachment.Panel1.ResumeLayout(False)
        CType(Me.SplitAttachment.Panel2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitAttachment.Panel2.ResumeLayout(False)
        CType(Me.SplitAttachment, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitAttachment.ResumeLayout(False)
        CType(Me.PanelControl1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelControl1.ResumeLayout(False)
        CType(Me.ListAttachments, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents SplitAttachment As DevExpress.XtraEditors.SplitContainerControl
    Private WithEvents PanelControl1 As DevExpress.XtraEditors.PanelControl
    Private WithEvents ListAttachments As DevExpress.XtraTreeList.TreeList
    Private WithEvents FileName As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents FileSize As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents Delete As DevExpress.XtraTreeList.Columns.TreeListColumn
    Private WithEvents BtnAddAttachment As DevExpress.XtraEditors.SimpleButton
    Private WithEvents PageViewer As DevExpress.XtraPdfViewer.PdfViewer

End Class
