Imports PDFSigner.My.Resources
Imports PDFSignerCommon

Friend Class FrmAbout
    Private Const _FormCSHIDString As String = "ABOUT"

    Friend Property LicenseInfo As LicenseInfo

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub FrmAboutPDFSigner_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lbl1.Text = String.Format(GUIText.About_Version, ProductVersion.ToString)
        lbl2.Text = String.Format(GUIText.About_End_User, LicenseInfo.Owner)
        lbl3.Text = String.Format(GUIText.About_Serial_Number, LicenseInfo.KofaxSerial)
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Close()
    End Sub

    Private Sub Lbl1_Click(sender As Object, e As EventArgs) Handles lbl1.Click
        Close()
    End Sub

    Private Sub Lbl2_Click(sender As Object, e As EventArgs) Handles lbl2.Click
        Close()
    End Sub

    Private Sub Lbl3_Click(sender As Object, e As EventArgs) Handles lbl3.Click
        Close()
    End Sub

End Class