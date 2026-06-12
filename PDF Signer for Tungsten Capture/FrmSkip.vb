Imports PDFSignerCommon
Imports PDFSigner.My.Resources

Friend Class FrmSkip
    Private ReadOnly _FormCSHIDString As String = "SKIPDOCUMENT"
    Friend Property SkipNote As String

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub FrmSkip_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MemoNote.Text = SkipNote
    End Sub

    Private Sub Reject_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        ComboNote.Focus()
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If Not String.IsNullOrEmpty(MemoNote.Text.Trim) Then
            SkipNote = MemoNote.Text.Trim
        ElseIf ComboNote.SelectedItem IsNot Nothing Then
            SkipNote = ComboNote.Text.Trim
        End If

        If String.IsNullOrWhiteSpace(SkipNote) Then
            MsgBox(Messages.Skip_Note_Required, MsgBoxStyle.Exclamation, My.Resources.MessageBoxTitle)
            DialogResult = Windows.Forms.DialogResult.None
            Return
        End If

        Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Close()
    End Sub

End Class