Imports System.IO
Imports System.Text
Imports PDFSignerSetup.My.Resources
Imports PDFSignerCommon
Imports Kofax.Capture.AdminModule.InteropServices

Friend Class FrmSetup
    Friend AdminApp As AdminApplication
    Private ReadOnly _FormCSHIDString As String = "SETUPDOCUMENTCLASS"
    Private Settings As Setup
    Private DocClass As DocumentClass
    ' Setting values as bound at load time; closing only asks to save when the current values differ.
    Private LoadedValueSnapshot As String

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub FrmSetup_Load(sender As Object, e As EventArgs) Handles Me.Load
        DocClass = CType(AdminApp.ActiveDocumentClass, DocumentClass)

        ' Fill index drop down lists
        Dim filler As New ComboFiller
        With filler
            .FillComboWithIndexes(DocClass, ComboBarCode)
            .FillComboWithIndexes(DocClass, ComboIsSigned)
            .FillComboWithIndexes(DocClass, ComboSignedBy)
            .FillComboWithIndexes(DocClass, ComboSignedAt)
            .FillComboWithIndexes(DocClass, ComboExpiry)
            .FillComboWithIndexes(DocClass, ComboSkipNote)
            .FillComboWithIndexes(DocClass, ComboDocumentName)
            .FillComboWithIndexes(DocClass, ComboMQDocUID)

            .FillComboWithMetadataTypes(ComboClauseFormat)
            .FillComboWithViewers(ComboDocumentViewer)
            .FillComboWithCryptoProviders(ComboCryptoProvider)

            .FillComboWithHashMethods(ComboSignatureHashMethod)
            .FillComboWithHashMethods(ComboTimeStampHashMethod)
            .FillComboWithRevocationMethods(ComboRevocationCheck)
            .FillComboWithProxyAuthMethods(ComboProxyAuthMethod)
        End With

        Settings = New Setup(DocClass)

        Try
            Settings.LoadValues()

            If Settings.DefaultCryptographicProvider = CryptoProviderType.PDFSigner Then
                TabCryptoProviders.SelectedTab = TabPDFSigner
            End If
            If Settings.DefaultCryptographicProvider = CryptoProviderType.PDFStreamer Then
                TabCryptoProviders.SelectedTab = TabPDFStreamer
            End If
            If Settings.DefaultCryptographicProvider = CryptoProviderType.MQFTP Then
                TabCryptoProviders.SelectedTab = TabMQ
            End If
            If Settings.DefaultCryptographicProvider = CryptoProviderType.MNBSigner Then
                TabCryptoProviders.SelectedTab = TabMNBSigner
            End If

        Catch ex As Exception
            MsgBox(String.Format(Messages.Exception_Loading_Setup_Values, ex.Message), MsgBoxStyle.Critical, Messages.Header_Error)
        End Try

        BsSetup.DataSource = Settings
        BsPDFSignerCryptoProvider.DataSource = Settings.PDFSignerProvider
        BsPDFStreamerCryptoProvider.DataSource = Settings.PDFStreamerProvider
        BsMQFTPCryptoProvider.DataSource = Settings.MQFTPProvider
        BsMNBSignerCryptoProvider.DataSource = Settings.MNBSignerProvider

        LoadedValueSnapshot = Settings.GetValueSnapshot()
    End Sub

    Private Sub FrmSetup_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        If DialogResult = DialogResult.OK Then Return

        ' Some bindings write to the model only on validation, so push the focused control's pending edit first.
        Validate()
        If Settings.GetValueSnapshot() = LoadedValueSnapshot Then Return

        Select Case MsgBox(Messages.Ask_Save_Changes, MsgBoxStyle.YesNoCancel + MsgBoxStyle.Question, Messages.Header_Save_Changes)
            Case vbYes
                If Not ValidateSettings() Then
                    e.Cancel = True
                    Return
                End If

                Settings.SaveValues()
            Case vbNo
                DialogResult = DialogResult.No
            Case vbCancel
                e.Cancel = True
        End Select
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs) Handles BtnOK.Click
        If Not ValidateSettings() Then
            DialogResult = DialogResult.None
            Return
        End If

        Settings.SaveValues()

        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Function ValidateSettings() As Boolean
        If String.IsNullOrEmpty(ComboBarCode.Text) Then Settings.IndexBarCode = Nothing
        If String.IsNullOrEmpty(ComboDocumentName.Text) Then Settings.IndexDocumentName = Nothing
        If String.IsNullOrEmpty(ComboIsSigned.Text) Then Settings.IndexIsSigned = Nothing
        If String.IsNullOrEmpty(ComboExpiry.Text) Then Settings.IndexSignatureValidUntil = Nothing
        If String.IsNullOrEmpty(ComboSignedBy.Text) Then Settings.IndexSignedBy = Nothing
        If String.IsNullOrEmpty(ComboSignedAt.Text) Then Settings.IndexSignedDateTime = Nothing
        If String.IsNullOrEmpty(ComboSkipNote.Text) Then Settings.IndexSkipNote = Nothing
        If String.IsNullOrEmpty(ComboMQDocUID.Text) Then Settings.MQFTPProvider.IndexMQDocUID = Nothing

        Dim errMsg = Settings.Validate

        If String.IsNullOrEmpty(errMsg) Then
            Return True
        Else
            MsgBox(errMsg, MsgBoxStyle.Exclamation, Messages.Header_Error)
            Return False
        End If
    End Function

    Private Sub BtnRemoveIndexIsSigned_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexIsSigned.Click
        ComboIsSigned.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexSignDateTime_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexSignDateTime.Click
        ComboSignedAt.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexSigningUser_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexSigningUser.Click
        ComboSignedBy.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexSignatureExpiry_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexSignatureExpiry.Click
        ComboExpiry.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexSkipNote_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexSkipNote.Click
        ComboSkipNote.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexBarCode_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexBarCode.Click
        ComboBarCode.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveDocNameIndexAssignment_Click(sender As Object, e As EventArgs) Handles BtnRemoveDocNameIndexAssignment.Click
        ComboDocumentName.SelectedIndex = -1
    End Sub

    Private Sub BtnRemoveIndexDocumentUID_Click(sender As Object, e As EventArgs) Handles BtnRemoveIndexDocumentUID.Click
        ComboMQDocUID.SelectedIndex = -1
    End Sub

#Region "Import/Export/Reset settings"

    Private Sub BtnImportSettings_Click(sender As Object, e As EventArgs) Handles BtnImportSettings.Click
        OpenFileDialog.ShowDialog()
    End Sub

    Private Sub OpenFileDialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog.FileOk
        Try
            Dim XmlText = File.ReadAllText(OpenFileDialog.FileName)
            Settings.FromXml(XmlText)
            BsSetup.ResetBindings(False)
            BsPDFSignerCryptoProvider.ResetBindings(False)
            BsPDFStreamerCryptoProvider.ResetBindings(False)
            BsMQFTPCryptoProvider.ResetBindings(False)
            BsMNBSignerCryptoProvider.ResetBindings(False)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, Messages.Header_File_Open_Error)
        End Try
    End Sub

    Private Sub BtnExportSettings_Click(sender As Object, e As EventArgs) Handles BtnExportSettings.Click
        If Not ValidateSettings() Then
            If MsgBox("Settings are invalid. Do you want to export them anyway?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Invalid settings") = MsgBoxResult.No Then Return
        End If

        SaveFileDialog.ValidateNames = True
        SaveFileDialog.ShowDialog()
    End Sub

    Private Sub SaveFileDialog_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles SaveFileDialog.FileOk
        Try
            File.WriteAllText(SaveFileDialog.FileName, Settings.ToXml, Encoding.UTF8)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OkOnly + MsgBoxStyle.Exclamation, Messages.Header_File_Save_Error)
        End Try
    End Sub

    Private Sub BtnResetSettings_Click(sender As Object, e As EventArgs) Handles BtnResetSettings.Click
        Dim ctrlDown = My.Computer.Keyboard.CtrlKeyDown

        If MsgBox(Messages.Reset_Settings, MsgBoxStyle.Question + MsgBoxStyle.YesNo, Messages.Header_Reset_Settings) = MsgBoxResult.No Then Return

        BsSetup.SuspendBinding()
        BsPDFSignerCryptoProvider.SuspendBinding()
        BsPDFStreamerCryptoProvider.SuspendBinding()
        BsMQFTPCryptoProvider.SuspendBinding()
        BsMNBSignerCryptoProvider.SuspendBinding()

        Settings = New Setup(DocClass)
        BsSetup.DataSource = Settings
        BsSetup.ResetBindings(False)
        BsSetup.ResumeBinding()

        BsPDFSignerCryptoProvider.DataSource = Settings.PDFSignerProvider
        BsPDFSignerCryptoProvider.ResetBindings(False)
        BsPDFSignerCryptoProvider.ResumeBinding()

        BsPDFStreamerCryptoProvider.DataSource = Settings.PDFStreamerProvider
        BsPDFStreamerCryptoProvider.ResetBindings(False)
        BsPDFStreamerCryptoProvider.ResumeBinding()

        BsMQFTPCryptoProvider.DataSource = Settings.MQFTPProvider
        BsMQFTPCryptoProvider.ResetBindings(False)
        BsMQFTPCryptoProvider.ResumeBinding()

        BsMNBSignerCryptoProvider.DataSource = Settings.MNBSignerProvider
        BsMNBSignerCryptoProvider.ResetBindings(False)
        BsMNBSignerCryptoProvider.ResumeBinding()

        If ctrlDown Then
            If MsgBox(String.Format(Messages.Reset_CSS_In_Kofax_Admin, CSS.BaseNamespace), MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, Messages.Header_Reset_Settings) = MsgBoxResult.No Then Return

            ' delete all CSS that starts with the base CSS name
            ' CSS deletion is not implemented in Kofax API yet
        End If
    End Sub

#End Region

End Class