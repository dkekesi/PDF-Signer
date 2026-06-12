<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmCloseSuspend
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmCloseSuspend))
        Me.BtnSuspend = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnClose = New DevExpress.XtraEditors.SimpleButton()
        Me.BtnCancel = New DevExpress.XtraEditors.SimpleButton()
        Me.SuspendLayout()
        '
        'BtnSuspend
        '
        resources.ApplyResources(Me.BtnSuspend, "BtnSuspend")
        Me.BtnSuspend.Name = "BtnSuspend"
        '
        'BtnClose
        '
        resources.ApplyResources(Me.BtnClose, "BtnClose")
        Me.BtnClose.Name = "BtnClose"
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        '
        'FrmCloseSuspend
        '
        Me.AcceptButton = Me.BtnSuspend
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.ControlBox = False
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnClose)
        Me.Controls.Add(Me.BtnSuspend)
        Me.LookAndFeel.SkinName = "Seven Classic"
        Me.LookAndFeel.UseDefaultLookAndFeel = False
        Me.Name = "FrmCloseSuspend"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents BtnSuspend As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnClose As DevExpress.XtraEditors.SimpleButton
    Private WithEvents BtnCancel As DevExpress.XtraEditors.SimpleButton
End Class
