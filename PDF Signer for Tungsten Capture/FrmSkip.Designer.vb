<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSkip
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSkip))
        Me.LabelControl1 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl2 = New DevExpress.XtraEditors.LabelControl()
        Me.LabelControl3 = New DevExpress.XtraEditors.LabelControl()
        Me.ComboNote = New DevExpress.XtraEditors.ComboBoxEdit()
        Me.BtnOK = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnCancel = New DevExpress.XtraEditors.SimpleButton()
        Me.MemoNote = New DevExpress.XtraEditors.MemoEdit()
        CType(Me.ComboNote.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MemoNote.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'LabelControl1
        '
        resources.ApplyResources(Me.LabelControl1, "LabelControl1")
        Me.LabelControl1.Appearance.Font = CType(resources.GetObject("LabelControl1.Appearance.Font"), System.Drawing.Font)
        Me.LabelControl1.Appearance.Options.UseFont = True
        Me.LabelControl1.Name = "LabelControl1"
        '
        'LabelControl2
        '
        resources.ApplyResources(Me.LabelControl2, "LabelControl2")
        Me.LabelControl2.Name = "LabelControl2"
        '
        'LabelControl3
        '
        resources.ApplyResources(Me.LabelControl3, "LabelControl3")
        Me.LabelControl3.Name = "LabelControl3"
        '
        'ComboNote
        '
        resources.ApplyResources(Me.ComboNote, "ComboNote")
        Me.ComboNote.Name = "ComboNote"
        Me.ComboNote.Properties.Buttons.AddRange(New DevExpress.XtraEditors.Controls.EditorButton() {New DevExpress.XtraEditors.Controls.EditorButton(CType(resources.GetObject("ComboNote.Properties.Buttons"), DevExpress.XtraEditors.Controls.ButtonPredefines))})
        Me.ComboNote.Properties.DropDownRows = 15
        Me.ComboNote.Properties.Items.AddRange(New Object() {resources.GetString("ComboNote.Properties.Items"), resources.GetString("ComboNote.Properties.Items1"), resources.GetString("ComboNote.Properties.Items2")})
        Me.ComboNote.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.BtnOK.Name = "BtnOK"
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        '
        'MemoNote
        '
        resources.ApplyResources(Me.MemoNote, "MemoNote")
        Me.MemoNote.Name = "MemoNote"
        Me.MemoNote.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None
        '
        'FrmSkip
        '
        Me.AcceptButton = Me.BtnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.MemoNote)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.ComboNote)
        Me.Controls.Add(Me.LabelControl3)
        Me.Controls.Add(Me.LabelControl2)
        Me.Controls.Add(Me.LabelControl1)
        Me.LookAndFeel.SkinName = "Office 2010 Blue"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FrmSkip"
        Me.ShowInTaskbar = False
        CType(Me.ComboNote.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MemoNote.Properties, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents LabelControl1 As DevExpress.XtraEditors.LabelControl
    Private WithEvents LabelControl2 As DevExpress.XtraEditors.LabelControl
    Private WithEvents LabelControl3 As DevExpress.XtraEditors.LabelControl
    Private WithEvents ComboNote As DevExpress.XtraEditors.ComboBoxEdit
    Private WithEvents BtnOK As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnCancel As DevExpress.XtraEditors.SimpleButton
    Private WithEvents MemoNote As DevExpress.XtraEditors.MemoEdit
End Class
