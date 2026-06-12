Imports System.ComponentModel
Imports System.Linq
Imports PDFSigner.My.Resources
Imports PDFSignerCommon

Friend Class FrmSaveFilter
    Private ReadOnly _FormCSHIDString As String = "SAVEFILTER"
    Friend Property SavedFilterList As BindingList(Of BatchFilter)
    Friend Property FilterName As String

    Private Sub FrmSaveFilter_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        ComboFilterName.Focus()
    End Sub

    Private Sub FrmSaveFilter_Load(sender As Object, e As EventArgs) Handles Me.Load
        If SavedFilterList IsNot Nothing Then
            For Each bf As BatchFilter In SavedFilterList
                ComboFilterName.Properties.Items.Add(bf.Name)
            Next
        End If
        ComboFilterName.Text = FilterName
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If String.IsNullOrWhiteSpace(ComboFilterName.Text) Then
            MsgBox(Messages.Filter_Name_Not_Provided, MsgBoxStyle.Exclamation, My.Resources.MessageBoxTitle)
            Return
        End If

        If SavedFilterList.Any(Function(f) f.Name = ComboFilterName.Text) Then
            If MsgBox(Messages.Filter_Already_Exists, MsgBoxStyle.Question + MsgBoxStyle.YesNo, My.Resources.MessageBoxTitle) = MsgBoxResult.No Then
                Return
            End If
        End If

        FilterName = ComboFilterName.Text
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        FilterName = String.Empty
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class
