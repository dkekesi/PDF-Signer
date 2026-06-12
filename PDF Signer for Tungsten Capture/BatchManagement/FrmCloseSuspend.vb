Imports PDFSigner.My.Resources
Imports PDFSignerCommon

Friend Class FrmCloseSuspend
    Private ReadOnly _FormCSHIDString As String = "CLOSEBATCH"
    Friend Property Result As CloseSuspendResult

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub CloseSuspend_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = String.Format(GUIText.Header_Batch_In_Progress, My.Resources.MessageBoxTitle)
    End Sub

    Private Sub BtnSuspend_Click(sender As Object, e As EventArgs) Handles BtnSuspend.Click
        Result = CloseSuspendResult.Suspend
        Close()
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Result = CloseSuspendResult.Close
        Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Result = CloseSuspendResult.Cancel
        Close()
    End Sub
End Class