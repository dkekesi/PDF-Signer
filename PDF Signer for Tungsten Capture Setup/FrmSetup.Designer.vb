<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FrmSetup
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSetup))
        Me.BtnOK = New System.Windows.Forms.Button()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.ChkSigningEnabled = New System.Windows.Forms.CheckBox()
        Me.BsSetup = New System.Windows.Forms.BindingSource(Me.components)
        Me.GrpIndexes = New System.Windows.Forms.GroupBox()
        Me.BtnRemoveIndexSkipNote = New System.Windows.Forms.Button()
        Me.ComboSkipNote = New System.Windows.Forms.ComboBox()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.BtnRemoveIndexSignatureExpiry = New System.Windows.Forms.Button()
        Me.BtnRemoveIndexSigningUser = New System.Windows.Forms.Button()
        Me.BtnRemoveIndexSignDateTime = New System.Windows.Forms.Button()
        Me.BtnRemoveIndexIsSigned = New System.Windows.Forms.Button()
        Me.ComboExpiry = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ComboSignedBy = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.ComboSignedAt = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ComboIsSigned = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBarCode = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.GrpBarCode = New System.Windows.Forms.GroupBox()
        Me.ChkAllowSkip = New System.Windows.Forms.CheckBox()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.BtnRemoveIndexBarCode = New System.Windows.Forms.Button()
        Me.NumericSignatureMarkerPosition = New System.Windows.Forms.NumericUpDown()
        Me.TxtSignatureMarker = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ChkAllDocumentsNeedSigning = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ComboSignatureHashMethod = New System.Windows.Forms.ComboBox()
        Me.BsPDFSignerCryptoProvider = New System.Windows.Forms.BindingSource(Me.components)
        Me.Label11 = New System.Windows.Forms.Label()
        Me.ComboRevocationCheck = New System.Windows.Forms.ComboBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtSigningReason = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TxtSigningOrganization = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.TxtSignaturePolicyOID = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TxtSignaturePolicyHash = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TxtSignaturePolicyURL = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.GrpTimeStamp = New System.Windows.Forms.GroupBox()
        Me.ChkSinglePassPadesLTA = New System.Windows.Forms.CheckBox()
        Me.ComboTimeStampHashMethod = New System.Windows.Forms.ComboBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.ChkIsDocumentTimeStamp = New System.Windows.Forms.CheckBox()
        Me.TxtTSAPassword = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.TxtTSAUser = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.TxtTSAURL = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.ChkTimeStampingEnabled = New System.Windows.Forms.CheckBox()
        Me.GrpClause = New System.Windows.Forms.GroupBox()
        Me.BtnRemoveDocNameIndexAssignment = New System.Windows.Forms.Button()
        Me.ChkDocNameFromFormType = New System.Windows.Forms.CheckBox()
        Me.ComboClauseFormat = New System.Windows.Forms.ComboBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.TxtCopyingRegulationVersion = New System.Windows.Forms.TextBox()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.TxtCopyingRegulationURL = New System.Windows.Forms.TextBox()
        Me.TxtCopyingRegulationName = New System.Windows.Forms.TextBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.TxtCopyingOrganizationName = New System.Windows.Forms.TextBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.TxtDocumentNameDefault = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.ComboDocumentName = New System.Windows.Forms.ComboBox()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.BtnImportSettings = New System.Windows.Forms.Button()
        Me.BtnExportSettings = New System.Windows.Forms.Button()
        Me.LblCryptoProvider = New System.Windows.Forms.Label()
        Me.ComboCryptoProvider = New System.Windows.Forms.ComboBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.BtnRemoveIndexDocumentUID = New System.Windows.Forms.Button()
        Me.BtnResetSettings = New System.Windows.Forms.Button()
        Me.TabCryptoProviders = New System.Windows.Forms.TabControl()
        Me.TabPDFSigner = New System.Windows.Forms.TabPage()
        Me.ChkPDFSignerEnabled = New System.Windows.Forms.CheckBox()
        Me.PanelPDFSigner = New System.Windows.Forms.Panel()
        Me.ChkAllowQualifiedCertificatesOnly = New System.Windows.Forms.CheckBox()
        Me.ChkProxyEnabled = New System.Windows.Forms.CheckBox()
        Me.GrpProxy = New System.Windows.Forms.GroupBox()
        Me.ComboProxyAuthMethod = New System.Windows.Forms.ComboBox()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.NumPort = New System.Windows.Forms.NumericUpDown()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.TxtProxyPassword = New System.Windows.Forms.TextBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.TxtProxyUser = New System.Windows.Forms.TextBox()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.TxtProxyServer = New System.Windows.Forms.TextBox()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.TabPDFStreamer = New System.Windows.Forms.TabPage()
        Me.ChkPDFStreamerEnabled = New System.Windows.Forms.CheckBox()
        Me.BsPDFStreamerCryptoProvider = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelPDFStreamer = New System.Windows.Forms.Panel()
        Me.GrpSignaturePDFStreamer = New System.Windows.Forms.GroupBox()
        Me.TxtPDFStreamerAuthorizationCode = New System.Windows.Forms.TextBox()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.TxtPDFStreamerConfigurationFileName = New System.Windows.Forms.TextBox()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.TxtPDFStreamerURL = New System.Windows.Forms.TextBox()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.TabMQ = New System.Windows.Forms.TabPage()
        Me.ChkMQEnabled = New System.Windows.Forms.CheckBox()
        Me.BsMQFTPCryptoProvider = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelMQFTP = New System.Windows.Forms.Panel()
        Me.GrpSignatureMQFTP = New System.Windows.Forms.GroupBox()
        Me.ComboMQDocUID = New System.Windows.Forms.ComboBox()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.TxtMQDomain = New System.Windows.Forms.TextBox()
        Me.TxtMQUserName = New System.Windows.Forms.TextBox()
        Me.TxtMQFolderIn = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.TxtMQPassword = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.TxtMQFolderOut = New System.Windows.Forms.TextBox()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.TabMNBSigner = New System.Windows.Forms.TabPage()
        Me.ChkMNBSignerEnabled = New System.Windows.Forms.CheckBox()
        Me.BsMNBSignerCryptoProvider = New System.Windows.Forms.BindingSource(Me.components)
        Me.PanelMNBSigner = New System.Windows.Forms.Panel()
        Me.GrpMNBSignerAuthentication = New System.Windows.Forms.GroupBox()
        Me.RadioMNBSignerPasswordAuthentication = New System.Windows.Forms.RadioButton()
        Me.RadioMNBSignerWindowsAuthentication = New System.Windows.Forms.RadioButton()
        Me.PanelMNBSignerAuthentication = New System.Windows.Forms.Panel()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.TxtMNBSignerDomain = New System.Windows.Forms.TextBox()
        Me.TxtMNBSignerUserName = New System.Windows.Forms.TextBox()
        Me.TxtMNBSignerPassword = New System.Windows.Forms.TextBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.GrpMNBSignerWCFSettings = New System.Windows.Forms.GroupBox()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.NumMNBSignerChunkSize = New System.Windows.Forms.NumericUpDown()
        Me.Label46 = New System.Windows.Forms.Label()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.NumMNBSignerSignTimeout = New System.Windows.Forms.NumericUpDown()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.NumMNBSignerWCFTimeout = New System.Windows.Forms.NumericUpDown()
        Me.TxtMNBSignerURL = New System.Windows.Forms.TextBox()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.GrpFileNaming = New System.Windows.Forms.GroupBox()
        Me.TxtFileExtension = New System.Windows.Forms.TextBox()
        Me.LblFileExtension = New System.Windows.Forms.Label()
        Me.TxtFileAppend = New System.Windows.Forms.TextBox()
        Me.LblFileAppend = New System.Windows.Forms.Label()
        Me.RadioCreateNewFile = New System.Windows.Forms.RadioButton()
        Me.RadioOverwriteOriginal = New System.Windows.Forms.RadioButton()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.ComboDocumentViewer = New System.Windows.Forms.ComboBox()
        Me.PanelMain = New System.Windows.Forms.Panel()
        Me.OpenFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFileDialog = New System.Windows.Forms.SaveFileDialog()
        CType(Me.BsSetup, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GrpIndexes.SuspendLayout()
        Me.GrpBarCode.SuspendLayout()
        CType(Me.NumericSignatureMarkerPosition, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.BsPDFSignerCryptoProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GrpTimeStamp.SuspendLayout()
        Me.GrpClause.SuspendLayout()
        Me.TabCryptoProviders.SuspendLayout()
        Me.TabPDFSigner.SuspendLayout()
        Me.PanelPDFSigner.SuspendLayout()
        Me.GrpProxy.SuspendLayout()
        CType(Me.NumPort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPDFStreamer.SuspendLayout()
        CType(Me.BsPDFStreamerCryptoProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelPDFStreamer.SuspendLayout()
        Me.GrpSignaturePDFStreamer.SuspendLayout()
        Me.TabMQ.SuspendLayout()
        CType(Me.BsMQFTPCryptoProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMQFTP.SuspendLayout()
        Me.GrpSignatureMQFTP.SuspendLayout()
        Me.TabMNBSigner.SuspendLayout()
        CType(Me.BsMNBSignerCryptoProvider, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelMNBSigner.SuspendLayout()
        Me.GrpMNBSignerAuthentication.SuspendLayout()
        Me.PanelMNBSignerAuthentication.SuspendLayout()
        Me.GrpMNBSignerWCFSettings.SuspendLayout()
        CType(Me.NumMNBSignerChunkSize, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumMNBSignerSignTimeout, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NumMNBSignerWCFTimeout, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GrpFileNaming.SuspendLayout()
        Me.PanelMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'BtnOK
        '
        resources.ApplyResources(Me.BtnOK, "BtnOK")
        Me.BtnOK.Name = "BtnOK"
        Me.BtnOK.UseVisualStyleBackColor = True
        '
        'BtnCancel
        '
        resources.ApplyResources(Me.BtnCancel, "BtnCancel")
        Me.BtnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.UseVisualStyleBackColor = True
        '
        'ChkSigningEnabled
        '
        resources.ApplyResources(Me.ChkSigningEnabled, "ChkSigningEnabled")
        Me.ChkSigningEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "IsSigningEnabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkSigningEnabled.Name = "ChkSigningEnabled"
        Me.ChkSigningEnabled.UseVisualStyleBackColor = True
        '
        'BsSetup
        '
        Me.BsSetup.DataSource = GetType(PDFSignerCommon.SetupModel)
        '
        'GrpIndexes
        '
        resources.ApplyResources(Me.GrpIndexes, "GrpIndexes")
        Me.GrpIndexes.Controls.Add(Me.BtnRemoveIndexSkipNote)
        Me.GrpIndexes.Controls.Add(Me.ComboSkipNote)
        Me.GrpIndexes.Controls.Add(Me.Label42)
        Me.GrpIndexes.Controls.Add(Me.BtnRemoveIndexSignatureExpiry)
        Me.GrpIndexes.Controls.Add(Me.BtnRemoveIndexSigningUser)
        Me.GrpIndexes.Controls.Add(Me.BtnRemoveIndexSignDateTime)
        Me.GrpIndexes.Controls.Add(Me.BtnRemoveIndexIsSigned)
        Me.GrpIndexes.Controls.Add(Me.ComboExpiry)
        Me.GrpIndexes.Controls.Add(Me.Label5)
        Me.GrpIndexes.Controls.Add(Me.ComboSignedBy)
        Me.GrpIndexes.Controls.Add(Me.Label4)
        Me.GrpIndexes.Controls.Add(Me.ComboSignedAt)
        Me.GrpIndexes.Controls.Add(Me.Label3)
        Me.GrpIndexes.Controls.Add(Me.ComboIsSigned)
        Me.GrpIndexes.Controls.Add(Me.Label2)
        Me.GrpIndexes.Name = "GrpIndexes"
        Me.GrpIndexes.TabStop = False
        '
        'BtnRemoveIndexSkipNote
        '
        resources.ApplyResources(Me.BtnRemoveIndexSkipNote, "BtnRemoveIndexSkipNote")
        Me.BtnRemoveIndexSkipNote.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexSkipNote.Name = "BtnRemoveIndexSkipNote"
        Me.BtnRemoveIndexSkipNote.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexSkipNote, resources.GetString("BtnRemoveIndexSkipNote.ToolTip"))
        Me.BtnRemoveIndexSkipNote.UseVisualStyleBackColor = True
        '
        'ComboSkipNote
        '
        resources.ApplyResources(Me.ComboSkipNote, "ComboSkipNote")
        Me.ComboSkipNote.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexSkipNote", True))
        Me.ComboSkipNote.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboSkipNote.FormattingEnabled = True
        Me.ComboSkipNote.Name = "ComboSkipNote"
        '
        'Label42
        '
        resources.ApplyResources(Me.Label42, "Label42")
        Me.Label42.Name = "Label42"
        '
        'BtnRemoveIndexSignatureExpiry
        '
        resources.ApplyResources(Me.BtnRemoveIndexSignatureExpiry, "BtnRemoveIndexSignatureExpiry")
        Me.BtnRemoveIndexSignatureExpiry.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexSignatureExpiry.Name = "BtnRemoveIndexSignatureExpiry"
        Me.BtnRemoveIndexSignatureExpiry.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexSignatureExpiry, resources.GetString("BtnRemoveIndexSignatureExpiry.ToolTip"))
        Me.BtnRemoveIndexSignatureExpiry.UseVisualStyleBackColor = True
        '
        'BtnRemoveIndexSigningUser
        '
        resources.ApplyResources(Me.BtnRemoveIndexSigningUser, "BtnRemoveIndexSigningUser")
        Me.BtnRemoveIndexSigningUser.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexSigningUser.Name = "BtnRemoveIndexSigningUser"
        Me.BtnRemoveIndexSigningUser.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexSigningUser, resources.GetString("BtnRemoveIndexSigningUser.ToolTip"))
        Me.BtnRemoveIndexSigningUser.UseVisualStyleBackColor = True
        '
        'BtnRemoveIndexSignDateTime
        '
        resources.ApplyResources(Me.BtnRemoveIndexSignDateTime, "BtnRemoveIndexSignDateTime")
        Me.BtnRemoveIndexSignDateTime.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexSignDateTime.Name = "BtnRemoveIndexSignDateTime"
        Me.BtnRemoveIndexSignDateTime.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexSignDateTime, resources.GetString("BtnRemoveIndexSignDateTime.ToolTip"))
        Me.BtnRemoveIndexSignDateTime.UseVisualStyleBackColor = True
        '
        'BtnRemoveIndexIsSigned
        '
        resources.ApplyResources(Me.BtnRemoveIndexIsSigned, "BtnRemoveIndexIsSigned")
        Me.BtnRemoveIndexIsSigned.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexIsSigned.Name = "BtnRemoveIndexIsSigned"
        Me.BtnRemoveIndexIsSigned.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexIsSigned, resources.GetString("BtnRemoveIndexIsSigned.ToolTip"))
        Me.BtnRemoveIndexIsSigned.UseVisualStyleBackColor = True
        '
        'ComboExpiry
        '
        resources.ApplyResources(Me.ComboExpiry, "ComboExpiry")
        Me.ComboExpiry.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexSignatureValidUntil", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboExpiry.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboExpiry.FormattingEnabled = True
        Me.ComboExpiry.Name = "ComboExpiry"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'ComboSignedBy
        '
        resources.ApplyResources(Me.ComboSignedBy, "ComboSignedBy")
        Me.ComboSignedBy.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexSignedBy", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboSignedBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboSignedBy.FormattingEnabled = True
        Me.ComboSignedBy.Name = "ComboSignedBy"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'ComboSignedAt
        '
        resources.ApplyResources(Me.ComboSignedAt, "ComboSignedAt")
        Me.ComboSignedAt.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexSignedDateTime", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboSignedAt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboSignedAt.FormattingEnabled = True
        Me.ComboSignedAt.Name = "ComboSignedAt"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'ComboIsSigned
        '
        resources.ApplyResources(Me.ComboIsSigned, "ComboIsSigned")
        Me.ComboIsSigned.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexIsSigned", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboIsSigned.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboIsSigned.FormattingEnabled = True
        Me.ComboIsSigned.Name = "ComboIsSigned"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'ComboBarCode
        '
        resources.ApplyResources(Me.ComboBarCode, "ComboBarCode")
        Me.ComboBarCode.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexBarCode", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboBarCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBarCode.FormattingEnabled = True
        Me.ComboBarCode.Name = "ComboBarCode"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'GrpBarCode
        '
        resources.ApplyResources(Me.GrpBarCode, "GrpBarCode")
        Me.GrpBarCode.Controls.Add(Me.ChkAllowSkip)
        Me.GrpBarCode.Controls.Add(Me.Label35)
        Me.GrpBarCode.Controls.Add(Me.BtnRemoveIndexBarCode)
        Me.GrpBarCode.Controls.Add(Me.NumericSignatureMarkerPosition)
        Me.GrpBarCode.Controls.Add(Me.TxtSignatureMarker)
        Me.GrpBarCode.Controls.Add(Me.Label7)
        Me.GrpBarCode.Controls.Add(Me.ChkAllDocumentsNeedSigning)
        Me.GrpBarCode.Controls.Add(Me.Label6)
        Me.GrpBarCode.Controls.Add(Me.Label1)
        Me.GrpBarCode.Controls.Add(Me.ComboBarCode)
        Me.GrpBarCode.Name = "GrpBarCode"
        Me.GrpBarCode.TabStop = False
        '
        'ChkAllowSkip
        '
        resources.ApplyResources(Me.ChkAllowSkip, "ChkAllowSkip")
        Me.ChkAllowSkip.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "AllowSkipRequiredDocument", True))
        Me.ChkAllowSkip.Name = "ChkAllowSkip"
        Me.ChkAllowSkip.UseVisualStyleBackColor = True
        '
        'Label35
        '
        resources.ApplyResources(Me.Label35, "Label35")
        Me.Label35.Name = "Label35"
        '
        'BtnRemoveIndexBarCode
        '
        resources.ApplyResources(Me.BtnRemoveIndexBarCode, "BtnRemoveIndexBarCode")
        Me.BtnRemoveIndexBarCode.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexBarCode.Name = "BtnRemoveIndexBarCode"
        Me.BtnRemoveIndexBarCode.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexBarCode, resources.GetString("BtnRemoveIndexBarCode.ToolTip"))
        Me.BtnRemoveIndexBarCode.UseVisualStyleBackColor = True
        '
        'NumericSignatureMarkerPosition
        '
        Me.NumericSignatureMarkerPosition.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BsSetup, "SignatureMarkerPosition", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.NumericSignatureMarkerPosition, "NumericSignatureMarkerPosition")
        Me.NumericSignatureMarkerPosition.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.NumericSignatureMarkerPosition.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumericSignatureMarkerPosition.Name = "NumericSignatureMarkerPosition"
        Me.NumericSignatureMarkerPosition.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'TxtSignatureMarker
        '
        Me.TxtSignatureMarker.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "SignatureMarker", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.TxtSignatureMarker, "TxtSignatureMarker")
        Me.TxtSignatureMarker.Name = "TxtSignatureMarker"
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'ChkAllDocumentsNeedSigning
        '
        resources.ApplyResources(Me.ChkAllDocumentsNeedSigning, "ChkAllDocumentsNeedSigning")
        Me.ChkAllDocumentsNeedSigning.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "SignAllDocuments", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkAllDocumentsNeedSigning.Name = "ChkAllDocumentsNeedSigning"
        Me.ChkAllDocumentsNeedSigning.UseVisualStyleBackColor = True
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'ComboSignatureHashMethod
        '
        resources.ApplyResources(Me.ComboSignatureHashMethod, "ComboSignatureHashMethod")
        Me.ComboSignatureHashMethod.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsPDFSignerCryptoProvider, "SignatureHashMethod", True))
        Me.ComboSignatureHashMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboSignatureHashMethod.FormattingEnabled = True
        Me.ComboSignatureHashMethod.Name = "ComboSignatureHashMethod"
        '
        'BsPDFSignerCryptoProvider
        '
        Me.BsPDFSignerCryptoProvider.DataSource = GetType(PDFSignerCommon.PDFSignerCryptoProvider)
        '
        'Label11
        '
        resources.ApplyResources(Me.Label11, "Label11")
        Me.Label11.Name = "Label11"
        '
        'ComboRevocationCheck
        '
        resources.ApplyResources(Me.ComboRevocationCheck, "ComboRevocationCheck")
        Me.ComboRevocationCheck.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsPDFSignerCryptoProvider, "RevocationCheck", True))
        Me.ComboRevocationCheck.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboRevocationCheck.FormattingEnabled = True
        Me.ComboRevocationCheck.Name = "ComboRevocationCheck"
        '
        'Label10
        '
        resources.ApplyResources(Me.Label10, "Label10")
        Me.Label10.Name = "Label10"
        '
        'TxtSigningReason
        '
        resources.ApplyResources(Me.TxtSigningReason, "TxtSigningReason")
        Me.TxtSigningReason.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "SigningReason", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtSigningReason.Name = "TxtSigningReason"
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'TxtSigningOrganization
        '
        resources.ApplyResources(Me.TxtSigningOrganization, "TxtSigningOrganization")
        Me.TxtSigningOrganization.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "SigningOrganization", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtSigningOrganization.Name = "TxtSigningOrganization"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'TxtSignaturePolicyOID
        '
        resources.ApplyResources(Me.TxtSignaturePolicyOID, "TxtSignaturePolicyOID")
        Me.TxtSignaturePolicyOID.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "SignaturePolicyOID", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtSignaturePolicyOID.Name = "TxtSignaturePolicyOID"
        '
        'Label14
        '
        resources.ApplyResources(Me.Label14, "Label14")
        Me.Label14.Name = "Label14"
        '
        'TxtSignaturePolicyHash
        '
        resources.ApplyResources(Me.TxtSignaturePolicyHash, "TxtSignaturePolicyHash")
        Me.TxtSignaturePolicyHash.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "SignaturePolicyHash", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtSignaturePolicyHash.Name = "TxtSignaturePolicyHash"
        '
        'Label13
        '
        resources.ApplyResources(Me.Label13, "Label13")
        Me.Label13.Name = "Label13"
        '
        'TxtSignaturePolicyURL
        '
        resources.ApplyResources(Me.TxtSignaturePolicyURL, "TxtSignaturePolicyURL")
        Me.TxtSignaturePolicyURL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "SignaturePolicyURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtSignaturePolicyURL.Name = "TxtSignaturePolicyURL"
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'GrpTimeStamp
        '
        resources.ApplyResources(Me.GrpTimeStamp, "GrpTimeStamp")
        Me.GrpTimeStamp.Controls.Add(Me.ChkSinglePassPadesLTA)
        Me.GrpTimeStamp.Controls.Add(Me.ComboTimeStampHashMethod)
        Me.GrpTimeStamp.Controls.Add(Me.Label18)
        Me.GrpTimeStamp.Controls.Add(Me.ChkIsDocumentTimeStamp)
        Me.GrpTimeStamp.Controls.Add(Me.TxtTSAPassword)
        Me.GrpTimeStamp.Controls.Add(Me.Label16)
        Me.GrpTimeStamp.Controls.Add(Me.TxtTSAUser)
        Me.GrpTimeStamp.Controls.Add(Me.Label17)
        Me.GrpTimeStamp.Controls.Add(Me.TxtTSAURL)
        Me.GrpTimeStamp.Controls.Add(Me.Label15)
        Me.GrpTimeStamp.Name = "GrpTimeStamp"
        Me.GrpTimeStamp.TabStop = False
        '
        'ChkSinglePassPadesLTA
        '
        resources.ApplyResources(Me.ChkSinglePassPadesLTA, "ChkSinglePassPadesLTA")
        Me.ChkSinglePassPadesLTA.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "IsSinglePassPadesBLTA", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkSinglePassPadesLTA.Name = "ChkSinglePassPadesLTA"
        Me.ChkSinglePassPadesLTA.UseVisualStyleBackColor = True
        '
        'ComboTimeStampHashMethod
        '
        resources.ApplyResources(Me.ComboTimeStampHashMethod, "ComboTimeStampHashMethod")
        Me.ComboTimeStampHashMethod.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsPDFSignerCryptoProvider, "TimeStampHashMethod", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboTimeStampHashMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboTimeStampHashMethod.FormattingEnabled = True
        Me.ComboTimeStampHashMethod.Name = "ComboTimeStampHashMethod"
        '
        'Label18
        '
        resources.ApplyResources(Me.Label18, "Label18")
        Me.Label18.Name = "Label18"
        '
        'ChkIsDocumentTimeStamp
        '
        resources.ApplyResources(Me.ChkIsDocumentTimeStamp, "ChkIsDocumentTimeStamp")
        Me.ChkIsDocumentTimeStamp.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "IsDocumentTimeStamp", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkIsDocumentTimeStamp.Name = "ChkIsDocumentTimeStamp"
        Me.ChkIsDocumentTimeStamp.UseVisualStyleBackColor = True
        '
        'TxtTSAPassword
        '
        resources.ApplyResources(Me.TxtTSAPassword, "TxtTSAPassword")
        Me.TxtTSAPassword.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "TSAPassword", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtTSAPassword.Name = "TxtTSAPassword"
        '
        'Label16
        '
        resources.ApplyResources(Me.Label16, "Label16")
        Me.Label16.Name = "Label16"
        '
        'TxtTSAUser
        '
        resources.ApplyResources(Me.TxtTSAUser, "TxtTSAUser")
        Me.TxtTSAUser.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "TSAUserName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtTSAUser.Name = "TxtTSAUser"
        '
        'Label17
        '
        resources.ApplyResources(Me.Label17, "Label17")
        Me.Label17.Name = "Label17"
        '
        'TxtTSAURL
        '
        resources.ApplyResources(Me.TxtTSAURL, "TxtTSAURL")
        Me.TxtTSAURL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "TSAURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtTSAURL.Name = "TxtTSAURL"
        '
        'Label15
        '
        resources.ApplyResources(Me.Label15, "Label15")
        Me.Label15.Name = "Label15"
        '
        'ChkTimeStampingEnabled
        '
        resources.ApplyResources(Me.ChkTimeStampingEnabled, "ChkTimeStampingEnabled")
        Me.ChkTimeStampingEnabled.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ChkTimeStampingEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "IsTimeStampingEnabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkTimeStampingEnabled.Name = "ChkTimeStampingEnabled"
        Me.ChkTimeStampingEnabled.UseVisualStyleBackColor = False
        '
        'GrpClause
        '
        resources.ApplyResources(Me.GrpClause, "GrpClause")
        Me.GrpClause.Controls.Add(Me.TxtSignaturePolicyOID)
        Me.GrpClause.Controls.Add(Me.BtnRemoveDocNameIndexAssignment)
        Me.GrpClause.Controls.Add(Me.Label14)
        Me.GrpClause.Controls.Add(Me.ChkDocNameFromFormType)
        Me.GrpClause.Controls.Add(Me.TxtSignaturePolicyHash)
        Me.GrpClause.Controls.Add(Me.ComboClauseFormat)
        Me.GrpClause.Controls.Add(Me.Label13)
        Me.GrpClause.Controls.Add(Me.Label25)
        Me.GrpClause.Controls.Add(Me.TxtSignaturePolicyURL)
        Me.GrpClause.Controls.Add(Me.Label23)
        Me.GrpClause.Controls.Add(Me.Label12)
        Me.GrpClause.Controls.Add(Me.TxtCopyingRegulationVersion)
        Me.GrpClause.Controls.Add(Me.Label24)
        Me.GrpClause.Controls.Add(Me.TxtCopyingRegulationURL)
        Me.GrpClause.Controls.Add(Me.TxtCopyingRegulationName)
        Me.GrpClause.Controls.Add(Me.Label22)
        Me.GrpClause.Controls.Add(Me.TxtCopyingOrganizationName)
        Me.GrpClause.Controls.Add(Me.Label21)
        Me.GrpClause.Controls.Add(Me.TxtDocumentNameDefault)
        Me.GrpClause.Controls.Add(Me.Label20)
        Me.GrpClause.Controls.Add(Me.ComboDocumentName)
        Me.GrpClause.Controls.Add(Me.Label19)
        Me.GrpClause.Name = "GrpClause"
        Me.GrpClause.TabStop = False
        '
        'BtnRemoveDocNameIndexAssignment
        '
        resources.ApplyResources(Me.BtnRemoveDocNameIndexAssignment, "BtnRemoveDocNameIndexAssignment")
        Me.BtnRemoveDocNameIndexAssignment.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveDocNameIndexAssignment.Name = "BtnRemoveDocNameIndexAssignment"
        Me.BtnRemoveDocNameIndexAssignment.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveDocNameIndexAssignment, resources.GetString("BtnRemoveDocNameIndexAssignment.ToolTip"))
        Me.BtnRemoveDocNameIndexAssignment.UseVisualStyleBackColor = True
        '
        'ChkDocNameFromFormType
        '
        resources.ApplyResources(Me.ChkDocNameFromFormType, "ChkDocNameFromFormType")
        Me.ChkDocNameFromFormType.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "DocumentNameFromFormType", True))
        Me.ChkDocNameFromFormType.Name = "ChkDocNameFromFormType"
        Me.ChkDocNameFromFormType.UseVisualStyleBackColor = True
        '
        'ComboClauseFormat
        '
        resources.ApplyResources(Me.ComboClauseFormat, "ComboClauseFormat")
        Me.ComboClauseFormat.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsSetup, "MetadataFormat", True))
        Me.ComboClauseFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboClauseFormat.FormattingEnabled = True
        Me.ComboClauseFormat.Name = "ComboClauseFormat"
        '
        'Label25
        '
        resources.ApplyResources(Me.Label25, "Label25")
        Me.Label25.Name = "Label25"
        '
        'Label23
        '
        resources.ApplyResources(Me.Label23, "Label23")
        Me.Label23.Name = "Label23"
        '
        'TxtCopyingRegulationVersion
        '
        resources.ApplyResources(Me.TxtCopyingRegulationVersion, "TxtCopyingRegulationVersion")
        Me.TxtCopyingRegulationVersion.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "ConvertingRegulationVersion", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtCopyingRegulationVersion.Name = "TxtCopyingRegulationVersion"
        '
        'Label24
        '
        resources.ApplyResources(Me.Label24, "Label24")
        Me.Label24.Name = "Label24"
        '
        'TxtCopyingRegulationURL
        '
        resources.ApplyResources(Me.TxtCopyingRegulationURL, "TxtCopyingRegulationURL")
        Me.TxtCopyingRegulationURL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "ConvertingRegulationURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtCopyingRegulationURL.Name = "TxtCopyingRegulationURL"
        '
        'TxtCopyingRegulationName
        '
        resources.ApplyResources(Me.TxtCopyingRegulationName, "TxtCopyingRegulationName")
        Me.TxtCopyingRegulationName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "ConvertingRegulationName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtCopyingRegulationName.Name = "TxtCopyingRegulationName"
        '
        'Label22
        '
        resources.ApplyResources(Me.Label22, "Label22")
        Me.Label22.Name = "Label22"
        '
        'TxtCopyingOrganizationName
        '
        resources.ApplyResources(Me.TxtCopyingOrganizationName, "TxtCopyingOrganizationName")
        Me.TxtCopyingOrganizationName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "ConvertingOrganization", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtCopyingOrganizationName.Name = "TxtCopyingOrganizationName"
        '
        'Label21
        '
        resources.ApplyResources(Me.Label21, "Label21")
        Me.Label21.Name = "Label21"
        '
        'TxtDocumentNameDefault
        '
        resources.ApplyResources(Me.TxtDocumentNameDefault, "TxtDocumentNameDefault")
        Me.TxtDocumentNameDefault.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "DocumentNameDefault", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtDocumentNameDefault.Name = "TxtDocumentNameDefault"
        '
        'Label20
        '
        resources.ApplyResources(Me.Label20, "Label20")
        Me.Label20.Name = "Label20"
        '
        'ComboDocumentName
        '
        resources.ApplyResources(Me.ComboDocumentName, "ComboDocumentName")
        Me.ComboDocumentName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "IndexDocumentName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboDocumentName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDocumentName.FormattingEnabled = True
        Me.ComboDocumentName.Name = "ComboDocumentName"
        '
        'Label19
        '
        resources.ApplyResources(Me.Label19, "Label19")
        Me.Label19.Name = "Label19"
        '
        'BtnImportSettings
        '
        resources.ApplyResources(Me.BtnImportSettings, "BtnImportSettings")
        Me.BtnImportSettings.Name = "BtnImportSettings"
        Me.BtnImportSettings.UseVisualStyleBackColor = True
        '
        'BtnExportSettings
        '
        resources.ApplyResources(Me.BtnExportSettings, "BtnExportSettings")
        Me.BtnExportSettings.Name = "BtnExportSettings"
        Me.BtnExportSettings.UseVisualStyleBackColor = True
        '
        'LblCryptoProvider
        '
        resources.ApplyResources(Me.LblCryptoProvider, "LblCryptoProvider")
        Me.LblCryptoProvider.Name = "LblCryptoProvider"
        '
        'ComboCryptoProvider
        '
        resources.ApplyResources(Me.ComboCryptoProvider, "ComboCryptoProvider")
        Me.ComboCryptoProvider.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsSetup, "DefaultCryptographicProvider", True))
        Me.ComboCryptoProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboCryptoProvider.FormattingEnabled = True
        Me.ComboCryptoProvider.Items.AddRange(New Object() {resources.GetString("ComboCryptoProvider.Items"), resources.GetString("ComboCryptoProvider.Items1"), resources.GetString("ComboCryptoProvider.Items2"), resources.GetString("ComboCryptoProvider.Items3")})
        Me.ComboCryptoProvider.Name = "ComboCryptoProvider"
        '
        'BtnRemoveIndexDocumentUID
        '
        resources.ApplyResources(Me.BtnRemoveIndexDocumentUID, "BtnRemoveIndexDocumentUID")
        Me.BtnRemoveIndexDocumentUID.Image = Global.PDFSignerSetup.My.Resources.Resources.Delete16
        Me.BtnRemoveIndexDocumentUID.Name = "BtnRemoveIndexDocumentUID"
        Me.BtnRemoveIndexDocumentUID.TabStop = False
        Me.ToolTip1.SetToolTip(Me.BtnRemoveIndexDocumentUID, resources.GetString("BtnRemoveIndexDocumentUID.ToolTip"))
        Me.BtnRemoveIndexDocumentUID.UseVisualStyleBackColor = True
        '
        'BtnResetSettings
        '
        resources.ApplyResources(Me.BtnResetSettings, "BtnResetSettings")
        Me.BtnResetSettings.Name = "BtnResetSettings"
        Me.BtnResetSettings.UseVisualStyleBackColor = True
        '
        'TabCryptoProviders
        '
        resources.ApplyResources(Me.TabCryptoProviders, "TabCryptoProviders")
        Me.TabCryptoProviders.Controls.Add(Me.TabPDFSigner)
        Me.TabCryptoProviders.Controls.Add(Me.TabPDFStreamer)
        Me.TabCryptoProviders.Controls.Add(Me.TabMQ)
        Me.TabCryptoProviders.Controls.Add(Me.TabMNBSigner)
        Me.TabCryptoProviders.Name = "TabCryptoProviders"
        Me.TabCryptoProviders.SelectedIndex = 0
        '
        'TabPDFSigner
        '
        Me.TabPDFSigner.Controls.Add(Me.ChkPDFSignerEnabled)
        Me.TabPDFSigner.Controls.Add(Me.PanelPDFSigner)
        resources.ApplyResources(Me.TabPDFSigner, "TabPDFSigner")
        Me.TabPDFSigner.Name = "TabPDFSigner"
        Me.TabPDFSigner.UseVisualStyleBackColor = True
        '
        'ChkPDFSignerEnabled
        '
        resources.ApplyResources(Me.ChkPDFSignerEnabled, "ChkPDFSignerEnabled")
        Me.ChkPDFSignerEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "Enabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkPDFSignerEnabled.Name = "ChkPDFSignerEnabled"
        Me.ChkPDFSignerEnabled.UseVisualStyleBackColor = True
        '
        'PanelPDFSigner
        '
        Me.PanelPDFSigner.Controls.Add(Me.ChkAllowQualifiedCertificatesOnly)
        Me.PanelPDFSigner.Controls.Add(Me.ComboSignatureHashMethod)
        Me.PanelPDFSigner.Controls.Add(Me.ChkProxyEnabled)
        Me.PanelPDFSigner.Controls.Add(Me.Label11)
        Me.PanelPDFSigner.Controls.Add(Me.ComboRevocationCheck)
        Me.PanelPDFSigner.Controls.Add(Me.ChkTimeStampingEnabled)
        Me.PanelPDFSigner.Controls.Add(Me.Label10)
        Me.PanelPDFSigner.Controls.Add(Me.TxtSigningReason)
        Me.PanelPDFSigner.Controls.Add(Me.GrpTimeStamp)
        Me.PanelPDFSigner.Controls.Add(Me.GrpProxy)
        Me.PanelPDFSigner.Controls.Add(Me.Label9)
        Me.PanelPDFSigner.Controls.Add(Me.TxtSigningOrganization)
        Me.PanelPDFSigner.Controls.Add(Me.Label8)
        resources.ApplyResources(Me.PanelPDFSigner, "PanelPDFSigner")
        Me.PanelPDFSigner.Name = "PanelPDFSigner"
        '
        'ChkAllowQualifiedCertificatesOnly
        '
        resources.ApplyResources(Me.ChkAllowQualifiedCertificatesOnly, "ChkAllowQualifiedCertificatesOnly")
        Me.ChkAllowQualifiedCertificatesOnly.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "AllowQualifiedCertificatesOnly", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkAllowQualifiedCertificatesOnly.Name = "ChkAllowQualifiedCertificatesOnly"
        Me.ChkAllowQualifiedCertificatesOnly.UseVisualStyleBackColor = True
        '
        'ChkProxyEnabled
        '
        resources.ApplyResources(Me.ChkProxyEnabled, "ChkProxyEnabled")
        Me.ChkProxyEnabled.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.ChkProxyEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFSignerCryptoProvider, "IsProxyEnabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkProxyEnabled.Name = "ChkProxyEnabled"
        Me.ChkProxyEnabled.UseVisualStyleBackColor = False
        '
        'GrpProxy
        '
        resources.ApplyResources(Me.GrpProxy, "GrpProxy")
        Me.GrpProxy.Controls.Add(Me.ComboProxyAuthMethod)
        Me.GrpProxy.Controls.Add(Me.Label40)
        Me.GrpProxy.Controls.Add(Me.NumPort)
        Me.GrpProxy.Controls.Add(Me.Label39)
        Me.GrpProxy.Controls.Add(Me.TxtProxyPassword)
        Me.GrpProxy.Controls.Add(Me.Label36)
        Me.GrpProxy.Controls.Add(Me.TxtProxyUser)
        Me.GrpProxy.Controls.Add(Me.Label37)
        Me.GrpProxy.Controls.Add(Me.TxtProxyServer)
        Me.GrpProxy.Controls.Add(Me.Label38)
        Me.GrpProxy.Name = "GrpProxy"
        Me.GrpProxy.TabStop = False
        '
        'ComboProxyAuthMethod
        '
        Me.ComboProxyAuthMethod.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsPDFSignerCryptoProvider, "ProxyAuthMethod", True))
        Me.ComboProxyAuthMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        resources.ApplyResources(Me.ComboProxyAuthMethod, "ComboProxyAuthMethod")
        Me.ComboProxyAuthMethod.FormattingEnabled = True
        Me.ComboProxyAuthMethod.Name = "ComboProxyAuthMethod"
        '
        'Label40
        '
        resources.ApplyResources(Me.Label40, "Label40")
        Me.Label40.Name = "Label40"
        '
        'NumPort
        '
        resources.ApplyResources(Me.NumPort, "NumPort")
        Me.NumPort.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BsPDFSignerCryptoProvider, "ProxyPort", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.NumPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        Me.NumPort.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.NumPort.Name = "NumPort"
        Me.NumPort.Value = New Decimal(New Integer() {8080, 0, 0, 0})
        '
        'Label39
        '
        resources.ApplyResources(Me.Label39, "Label39")
        Me.Label39.Name = "Label39"
        '
        'TxtProxyPassword
        '
        resources.ApplyResources(Me.TxtProxyPassword, "TxtProxyPassword")
        Me.TxtProxyPassword.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "ProxyPassword", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtProxyPassword.Name = "TxtProxyPassword"
        '
        'Label36
        '
        resources.ApplyResources(Me.Label36, "Label36")
        Me.Label36.Name = "Label36"
        '
        'TxtProxyUser
        '
        resources.ApplyResources(Me.TxtProxyUser, "TxtProxyUser")
        Me.TxtProxyUser.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "ProxyUserName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtProxyUser.Name = "TxtProxyUser"
        '
        'Label37
        '
        resources.ApplyResources(Me.Label37, "Label37")
        Me.Label37.Name = "Label37"
        '
        'TxtProxyServer
        '
        resources.ApplyResources(Me.TxtProxyServer, "TxtProxyServer")
        Me.TxtProxyServer.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFSignerCryptoProvider, "ProxyServer", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtProxyServer.Name = "TxtProxyServer"
        '
        'Label38
        '
        resources.ApplyResources(Me.Label38, "Label38")
        Me.Label38.Name = "Label38"
        '
        'TabPDFStreamer
        '
        Me.TabPDFStreamer.Controls.Add(Me.ChkPDFStreamerEnabled)
        Me.TabPDFStreamer.Controls.Add(Me.PanelPDFStreamer)
        resources.ApplyResources(Me.TabPDFStreamer, "TabPDFStreamer")
        Me.TabPDFStreamer.Name = "TabPDFStreamer"
        Me.TabPDFStreamer.UseVisualStyleBackColor = True
        '
        'ChkPDFStreamerEnabled
        '
        resources.ApplyResources(Me.ChkPDFStreamerEnabled, "ChkPDFStreamerEnabled")
        Me.ChkPDFStreamerEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsPDFStreamerCryptoProvider, "Enabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkPDFStreamerEnabled.Name = "ChkPDFStreamerEnabled"
        Me.ChkPDFStreamerEnabled.UseVisualStyleBackColor = True
        '
        'BsPDFStreamerCryptoProvider
        '
        Me.BsPDFStreamerCryptoProvider.DataSource = GetType(PDFSignerCommon.PDFStreamerCryptoProvider)
        '
        'PanelPDFStreamer
        '
        Me.PanelPDFStreamer.Controls.Add(Me.GrpSignaturePDFStreamer)
        resources.ApplyResources(Me.PanelPDFStreamer, "PanelPDFStreamer")
        Me.PanelPDFStreamer.Name = "PanelPDFStreamer"
        '
        'GrpSignaturePDFStreamer
        '
        resources.ApplyResources(Me.GrpSignaturePDFStreamer, "GrpSignaturePDFStreamer")
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.TxtPDFStreamerAuthorizationCode)
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.Label29)
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.TxtPDFStreamerConfigurationFileName)
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.Label30)
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.TxtPDFStreamerURL)
        Me.GrpSignaturePDFStreamer.Controls.Add(Me.Label31)
        Me.GrpSignaturePDFStreamer.Name = "GrpSignaturePDFStreamer"
        Me.GrpSignaturePDFStreamer.TabStop = False
        '
        'TxtPDFStreamerAuthorizationCode
        '
        resources.ApplyResources(Me.TxtPDFStreamerAuthorizationCode, "TxtPDFStreamerAuthorizationCode")
        Me.TxtPDFStreamerAuthorizationCode.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFStreamerCryptoProvider, "PDFStreamerAuthorizationCode", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtPDFStreamerAuthorizationCode.Name = "TxtPDFStreamerAuthorizationCode"
        '
        'Label29
        '
        resources.ApplyResources(Me.Label29, "Label29")
        Me.Label29.Name = "Label29"
        '
        'TxtPDFStreamerConfigurationFileName
        '
        resources.ApplyResources(Me.TxtPDFStreamerConfigurationFileName, "TxtPDFStreamerConfigurationFileName")
        Me.TxtPDFStreamerConfigurationFileName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFStreamerCryptoProvider, "PDFStreamerConfigFile", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtPDFStreamerConfigurationFileName.Name = "TxtPDFStreamerConfigurationFileName"
        '
        'Label30
        '
        resources.ApplyResources(Me.Label30, "Label30")
        Me.Label30.Name = "Label30"
        '
        'TxtPDFStreamerURL
        '
        resources.ApplyResources(Me.TxtPDFStreamerURL, "TxtPDFStreamerURL")
        Me.TxtPDFStreamerURL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsPDFStreamerCryptoProvider, "PDFStreamerURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtPDFStreamerURL.Name = "TxtPDFStreamerURL"
        '
        'Label31
        '
        resources.ApplyResources(Me.Label31, "Label31")
        Me.Label31.Name = "Label31"
        '
        'TabMQ
        '
        Me.TabMQ.Controls.Add(Me.ChkMQEnabled)
        Me.TabMQ.Controls.Add(Me.PanelMQFTP)
        resources.ApplyResources(Me.TabMQ, "TabMQ")
        Me.TabMQ.Name = "TabMQ"
        Me.TabMQ.UseVisualStyleBackColor = True
        '
        'ChkMQEnabled
        '
        resources.ApplyResources(Me.ChkMQEnabled, "ChkMQEnabled")
        Me.ChkMQEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsMQFTPCryptoProvider, "Enabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkMQEnabled.Name = "ChkMQEnabled"
        Me.ChkMQEnabled.UseVisualStyleBackColor = True
        '
        'BsMQFTPCryptoProvider
        '
        Me.BsMQFTPCryptoProvider.DataSource = GetType(PDFSignerCommon.MQFTPCryptoProvider)
        '
        'PanelMQFTP
        '
        Me.PanelMQFTP.Controls.Add(Me.GrpSignatureMQFTP)
        resources.ApplyResources(Me.PanelMQFTP, "PanelMQFTP")
        Me.PanelMQFTP.Name = "PanelMQFTP"
        '
        'GrpSignatureMQFTP
        '
        resources.ApplyResources(Me.GrpSignatureMQFTP, "GrpSignatureMQFTP")
        Me.GrpSignatureMQFTP.Controls.Add(Me.BtnRemoveIndexDocumentUID)
        Me.GrpSignatureMQFTP.Controls.Add(Me.ComboMQDocUID)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label34)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label33)
        Me.GrpSignatureMQFTP.Controls.Add(Me.TxtMQDomain)
        Me.GrpSignatureMQFTP.Controls.Add(Me.TxtMQUserName)
        Me.GrpSignatureMQFTP.Controls.Add(Me.TxtMQFolderIn)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label32)
        Me.GrpSignatureMQFTP.Controls.Add(Me.TxtMQPassword)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label26)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label27)
        Me.GrpSignatureMQFTP.Controls.Add(Me.TxtMQFolderOut)
        Me.GrpSignatureMQFTP.Controls.Add(Me.Label28)
        Me.GrpSignatureMQFTP.Name = "GrpSignatureMQFTP"
        Me.GrpSignatureMQFTP.TabStop = False
        '
        'ComboMQDocUID
        '
        resources.ApplyResources(Me.ComboMQDocUID, "ComboMQDocUID")
        Me.ComboMQDocUID.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "IndexMQDocUID", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ComboMQDocUID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboMQDocUID.FormattingEnabled = True
        Me.ComboMQDocUID.Name = "ComboMQDocUID"
        '
        'Label34
        '
        resources.ApplyResources(Me.Label34, "Label34")
        Me.Label34.Name = "Label34"
        '
        'Label33
        '
        resources.ApplyResources(Me.Label33, "Label33")
        Me.Label33.Name = "Label33"
        '
        'TxtMQDomain
        '
        Me.TxtMQDomain.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "MQDomain", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.TxtMQDomain, "TxtMQDomain")
        Me.TxtMQDomain.Name = "TxtMQDomain"
        '
        'TxtMQUserName
        '
        resources.ApplyResources(Me.TxtMQUserName, "TxtMQUserName")
        Me.TxtMQUserName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "MQUserName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMQUserName.Name = "TxtMQUserName"
        '
        'TxtMQFolderIn
        '
        resources.ApplyResources(Me.TxtMQFolderIn, "TxtMQFolderIn")
        Me.TxtMQFolderIn.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "MQFolderIn", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMQFolderIn.Name = "TxtMQFolderIn"
        '
        'Label32
        '
        resources.ApplyResources(Me.Label32, "Label32")
        Me.Label32.Name = "Label32"
        '
        'TxtMQPassword
        '
        resources.ApplyResources(Me.TxtMQPassword, "TxtMQPassword")
        Me.TxtMQPassword.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "MQPassword", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMQPassword.Name = "TxtMQPassword"
        '
        'Label26
        '
        resources.ApplyResources(Me.Label26, "Label26")
        Me.Label26.Name = "Label26"
        '
        'Label27
        '
        resources.ApplyResources(Me.Label27, "Label27")
        Me.Label27.Name = "Label27"
        '
        'TxtMQFolderOut
        '
        resources.ApplyResources(Me.TxtMQFolderOut, "TxtMQFolderOut")
        Me.TxtMQFolderOut.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMQFTPCryptoProvider, "MQFolderOut", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMQFolderOut.Name = "TxtMQFolderOut"
        '
        'Label28
        '
        resources.ApplyResources(Me.Label28, "Label28")
        Me.Label28.Name = "Label28"
        '
        'TabMNBSigner
        '
        Me.TabMNBSigner.Controls.Add(Me.ChkMNBSignerEnabled)
        Me.TabMNBSigner.Controls.Add(Me.PanelMNBSigner)
        resources.ApplyResources(Me.TabMNBSigner, "TabMNBSigner")
        Me.TabMNBSigner.Name = "TabMNBSigner"
        Me.TabMNBSigner.UseVisualStyleBackColor = True
        '
        'ChkMNBSignerEnabled
        '
        resources.ApplyResources(Me.ChkMNBSignerEnabled, "ChkMNBSignerEnabled")
        Me.ChkMNBSignerEnabled.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsMNBSignerCryptoProvider, "Enabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.ChkMNBSignerEnabled.Name = "ChkMNBSignerEnabled"
        Me.ChkMNBSignerEnabled.UseVisualStyleBackColor = True
        '
        'BsMNBSignerCryptoProvider
        '
        Me.BsMNBSignerCryptoProvider.DataSource = GetType(PDFSignerCommon.MNBSignerCryptoProvider)
        '
        'PanelMNBSigner
        '
        Me.PanelMNBSigner.Controls.Add(Me.GrpMNBSignerAuthentication)
        Me.PanelMNBSigner.Controls.Add(Me.GrpMNBSignerWCFSettings)
        resources.ApplyResources(Me.PanelMNBSigner, "PanelMNBSigner")
        Me.PanelMNBSigner.Name = "PanelMNBSigner"
        '
        'GrpMNBSignerAuthentication
        '
        resources.ApplyResources(Me.GrpMNBSignerAuthentication, "GrpMNBSignerAuthentication")
        Me.GrpMNBSignerAuthentication.Controls.Add(Me.RadioMNBSignerPasswordAuthentication)
        Me.GrpMNBSignerAuthentication.Controls.Add(Me.RadioMNBSignerWindowsAuthentication)
        Me.GrpMNBSignerAuthentication.Controls.Add(Me.PanelMNBSignerAuthentication)
        Me.GrpMNBSignerAuthentication.Name = "GrpMNBSignerAuthentication"
        Me.GrpMNBSignerAuthentication.TabStop = False
        '
        'RadioMNBSignerPasswordAuthentication
        '
        resources.ApplyResources(Me.RadioMNBSignerPasswordAuthentication, "RadioMNBSignerPasswordAuthentication")
        Me.RadioMNBSignerPasswordAuthentication.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsMNBSignerCryptoProvider, "IsMNBSignerUserPasswordAuthentication", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.RadioMNBSignerPasswordAuthentication.Name = "RadioMNBSignerPasswordAuthentication"
        Me.RadioMNBSignerPasswordAuthentication.TabStop = True
        Me.RadioMNBSignerPasswordAuthentication.UseVisualStyleBackColor = True
        '
        'RadioMNBSignerWindowsAuthentication
        '
        resources.ApplyResources(Me.RadioMNBSignerWindowsAuthentication, "RadioMNBSignerWindowsAuthentication")
        Me.RadioMNBSignerWindowsAuthentication.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsMNBSignerCryptoProvider, "IsMNBSignerWindowsAuthentication", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.RadioMNBSignerWindowsAuthentication.Name = "RadioMNBSignerWindowsAuthentication"
        Me.RadioMNBSignerWindowsAuthentication.TabStop = True
        Me.RadioMNBSignerWindowsAuthentication.UseVisualStyleBackColor = True
        '
        'PanelMNBSignerAuthentication
        '
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.Label50)
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.TxtMNBSignerDomain)
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.TxtMNBSignerUserName)
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.TxtMNBSignerPassword)
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.Label51)
        Me.PanelMNBSignerAuthentication.Controls.Add(Me.Label52)
        resources.ApplyResources(Me.PanelMNBSignerAuthentication, "PanelMNBSignerAuthentication")
        Me.PanelMNBSignerAuthentication.Name = "PanelMNBSignerAuthentication"
        '
        'Label50
        '
        resources.ApplyResources(Me.Label50, "Label50")
        Me.Label50.Name = "Label50"
        '
        'TxtMNBSignerDomain
        '
        Me.TxtMNBSignerDomain.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMNBSignerCryptoProvider, "MNBSignerDomain", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.TxtMNBSignerDomain, "TxtMNBSignerDomain")
        Me.TxtMNBSignerDomain.Name = "TxtMNBSignerDomain"
        '
        'TxtMNBSignerUserName
        '
        resources.ApplyResources(Me.TxtMNBSignerUserName, "TxtMNBSignerUserName")
        Me.TxtMNBSignerUserName.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMNBSignerCryptoProvider, "MNBSignerUserName", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMNBSignerUserName.Name = "TxtMNBSignerUserName"
        '
        'TxtMNBSignerPassword
        '
        resources.ApplyResources(Me.TxtMNBSignerPassword, "TxtMNBSignerPassword")
        Me.TxtMNBSignerPassword.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMNBSignerCryptoProvider, "MNBSignerPassword", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMNBSignerPassword.Name = "TxtMNBSignerPassword"
        '
        'Label51
        '
        resources.ApplyResources(Me.Label51, "Label51")
        Me.Label51.Name = "Label51"
        '
        'Label52
        '
        resources.ApplyResources(Me.Label52, "Label52")
        Me.Label52.Name = "Label52"
        '
        'GrpMNBSignerWCFSettings
        '
        resources.ApplyResources(Me.GrpMNBSignerWCFSettings, "GrpMNBSignerWCFSettings")
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label48)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label49)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.NumMNBSignerChunkSize)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label46)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label47)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.NumMNBSignerSignTimeout)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label45)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label44)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.NumMNBSignerWCFTimeout)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.TxtMNBSignerURL)
        Me.GrpMNBSignerWCFSettings.Controls.Add(Me.Label43)
        Me.GrpMNBSignerWCFSettings.Name = "GrpMNBSignerWCFSettings"
        Me.GrpMNBSignerWCFSettings.TabStop = False
        '
        'Label48
        '
        resources.ApplyResources(Me.Label48, "Label48")
        Me.Label48.Name = "Label48"
        '
        'Label49
        '
        resources.ApplyResources(Me.Label49, "Label49")
        Me.Label49.Name = "Label49"
        '
        'NumMNBSignerChunkSize
        '
        Me.NumMNBSignerChunkSize.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BsMNBSignerCryptoProvider, "MNBSignerChunkSize", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.NumMNBSignerChunkSize, "NumMNBSignerChunkSize")
        Me.NumMNBSignerChunkSize.Maximum = New Decimal(New Integer() {2147483647, 0, 0, 0})
        Me.NumMNBSignerChunkSize.Name = "NumMNBSignerChunkSize"
        Me.NumMNBSignerChunkSize.Value = New Decimal(New Integer() {25000000, 0, 0, 0})
        '
        'Label46
        '
        resources.ApplyResources(Me.Label46, "Label46")
        Me.Label46.Name = "Label46"
        '
        'Label47
        '
        resources.ApplyResources(Me.Label47, "Label47")
        Me.Label47.Name = "Label47"
        '
        'NumMNBSignerSignTimeout
        '
        Me.NumMNBSignerSignTimeout.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BsMNBSignerCryptoProvider, "MNBSignerSigningTimeout", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.NumMNBSignerSignTimeout, "NumMNBSignerSignTimeout")
        Me.NumMNBSignerSignTimeout.Maximum = New Decimal(New Integer() {1200, 0, 0, 0})
        Me.NumMNBSignerSignTimeout.Name = "NumMNBSignerSignTimeout"
        Me.NumMNBSignerSignTimeout.Value = New Decimal(New Integer() {60, 0, 0, 0})
        '
        'Label45
        '
        resources.ApplyResources(Me.Label45, "Label45")
        Me.Label45.Name = "Label45"
        '
        'Label44
        '
        resources.ApplyResources(Me.Label44, "Label44")
        Me.Label44.Name = "Label44"
        '
        'NumMNBSignerWCFTimeout
        '
        Me.NumMNBSignerWCFTimeout.DataBindings.Add(New System.Windows.Forms.Binding("Value", Me.BsMNBSignerCryptoProvider, "MNBSignerWSTimeout", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.NumMNBSignerWCFTimeout, "NumMNBSignerWCFTimeout")
        Me.NumMNBSignerWCFTimeout.Maximum = New Decimal(New Integer() {1200, 0, 0, 0})
        Me.NumMNBSignerWCFTimeout.Name = "NumMNBSignerWCFTimeout"
        Me.NumMNBSignerWCFTimeout.Value = New Decimal(New Integer() {60, 0, 0, 0})
        '
        'TxtMNBSignerURL
        '
        resources.ApplyResources(Me.TxtMNBSignerURL, "TxtMNBSignerURL")
        Me.TxtMNBSignerURL.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsMNBSignerCryptoProvider, "MNBSignerURL", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.TxtMNBSignerURL.Name = "TxtMNBSignerURL"
        '
        'Label43
        '
        resources.ApplyResources(Me.Label43, "Label43")
        Me.Label43.Name = "Label43"
        '
        'GrpFileNaming
        '
        resources.ApplyResources(Me.GrpFileNaming, "GrpFileNaming")
        Me.GrpFileNaming.Controls.Add(Me.TxtFileExtension)
        Me.GrpFileNaming.Controls.Add(Me.LblFileExtension)
        Me.GrpFileNaming.Controls.Add(Me.TxtFileAppend)
        Me.GrpFileNaming.Controls.Add(Me.LblFileAppend)
        Me.GrpFileNaming.Controls.Add(Me.RadioCreateNewFile)
        Me.GrpFileNaming.Controls.Add(Me.RadioOverwriteOriginal)
        Me.GrpFileNaming.Name = "GrpFileNaming"
        Me.GrpFileNaming.TabStop = False
        '
        'TxtFileExtension
        '
        resources.ApplyResources(Me.TxtFileExtension, "TxtFileExtension")
        Me.TxtFileExtension.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "FileExtensionReplace", True))
        Me.TxtFileExtension.Name = "TxtFileExtension"
        '
        'LblFileExtension
        '
        resources.ApplyResources(Me.LblFileExtension, "LblFileExtension")
        Me.LblFileExtension.Name = "LblFileExtension"
        '
        'TxtFileAppend
        '
        resources.ApplyResources(Me.TxtFileAppend, "TxtFileAppend")
        Me.TxtFileAppend.DataBindings.Add(New System.Windows.Forms.Binding("Text", Me.BsSetup, "FileNameAppend", True))
        Me.TxtFileAppend.Name = "TxtFileAppend"
        '
        'LblFileAppend
        '
        resources.ApplyResources(Me.LblFileAppend, "LblFileAppend")
        Me.LblFileAppend.Name = "LblFileAppend"
        '
        'RadioCreateNewFile
        '
        resources.ApplyResources(Me.RadioCreateNewFile, "RadioCreateNewFile")
        Me.RadioCreateNewFile.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "FileCreateNew", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.RadioCreateNewFile.Name = "RadioCreateNewFile"
        Me.RadioCreateNewFile.UseVisualStyleBackColor = True
        '
        'RadioOverwriteOriginal
        '
        resources.ApplyResources(Me.RadioOverwriteOriginal, "RadioOverwriteOriginal")
        Me.RadioOverwriteOriginal.Checked = True
        Me.RadioOverwriteOriginal.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Me.BsSetup, "FileOverwriteOriginal", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.RadioOverwriteOriginal.Name = "RadioOverwriteOriginal"
        Me.RadioOverwriteOriginal.TabStop = True
        Me.RadioOverwriteOriginal.UseVisualStyleBackColor = True
        '
        'Label41
        '
        resources.ApplyResources(Me.Label41, "Label41")
        Me.Label41.Name = "Label41"
        '
        'ComboDocumentViewer
        '
        resources.ApplyResources(Me.ComboDocumentViewer, "ComboDocumentViewer")
        Me.ComboDocumentViewer.DataBindings.Add(New System.Windows.Forms.Binding("SelectedValue", Me.BsSetup, "DocumentViewer", True))
        Me.ComboDocumentViewer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboDocumentViewer.FormattingEnabled = True
        Me.ComboDocumentViewer.Items.AddRange(New Object() {resources.GetString("ComboDocumentViewer.Items"), resources.GetString("ComboDocumentViewer.Items1")})
        Me.ComboDocumentViewer.Name = "ComboDocumentViewer"
        '
        'PanelMain
        '
        Me.PanelMain.Controls.Add(Me.Label41)
        Me.PanelMain.Controls.Add(Me.GrpIndexes)
        Me.PanelMain.Controls.Add(Me.ComboDocumentViewer)
        Me.PanelMain.Controls.Add(Me.GrpBarCode)
        Me.PanelMain.Controls.Add(Me.GrpFileNaming)
        Me.PanelMain.Controls.Add(Me.GrpClause)
        Me.PanelMain.Controls.Add(Me.TabCryptoProviders)
        Me.PanelMain.Controls.Add(Me.LblCryptoProvider)
        Me.PanelMain.Controls.Add(Me.ComboCryptoProvider)
        Me.PanelMain.DataBindings.Add(New System.Windows.Forms.Binding("Enabled", Me.BsSetup, "IsSigningEnabled", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(Me.PanelMain, "PanelMain")
        Me.PanelMain.Name = "PanelMain"
        '
        'OpenFileDialog
        '
        Me.OpenFileDialog.DefaultExt = "xml"
        Me.OpenFileDialog.FileName = "PDFSignerConfig"
        resources.ApplyResources(Me.OpenFileDialog, "OpenFileDialog")
        '
        'SaveFileDialog
        '
        Me.SaveFileDialog.DefaultExt = "xml"
        Me.SaveFileDialog.FileName = "PDFSignerConfig"
        resources.ApplyResources(Me.SaveFileDialog, "SaveFileDialog")
        '
        'FrmSetup
        '
        Me.AcceptButton = Me.BtnOK
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.BtnCancel
        Me.Controls.Add(Me.BtnResetSettings)
        Me.Controls.Add(Me.BtnExportSettings)
        Me.Controls.Add(Me.BtnImportSettings)
        Me.Controls.Add(Me.ChkSigningEnabled)
        Me.Controls.Add(Me.BtnCancel)
        Me.Controls.Add(Me.BtnOK)
        Me.Controls.Add(Me.PanelMain)
        Me.MaximizeBox = False
        Me.Name = "FrmSetup"
        Me.ShowInTaskbar = False
        CType(Me.BsSetup, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GrpIndexes.ResumeLayout(False)
        Me.GrpIndexes.PerformLayout()
        Me.GrpBarCode.ResumeLayout(False)
        Me.GrpBarCode.PerformLayout()
        CType(Me.NumericSignatureMarkerPosition, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.BsPDFSignerCryptoProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GrpTimeStamp.ResumeLayout(False)
        Me.GrpTimeStamp.PerformLayout()
        Me.GrpClause.ResumeLayout(False)
        Me.GrpClause.PerformLayout()
        Me.TabCryptoProviders.ResumeLayout(False)
        Me.TabPDFSigner.ResumeLayout(False)
        Me.TabPDFSigner.PerformLayout()
        Me.PanelPDFSigner.ResumeLayout(False)
        Me.PanelPDFSigner.PerformLayout()
        Me.GrpProxy.ResumeLayout(False)
        Me.GrpProxy.PerformLayout()
        CType(Me.NumPort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPDFStreamer.ResumeLayout(False)
        Me.TabPDFStreamer.PerformLayout()
        CType(Me.BsPDFStreamerCryptoProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelPDFStreamer.ResumeLayout(False)
        Me.GrpSignaturePDFStreamer.ResumeLayout(False)
        Me.GrpSignaturePDFStreamer.PerformLayout()
        Me.TabMQ.ResumeLayout(False)
        Me.TabMQ.PerformLayout()
        CType(Me.BsMQFTPCryptoProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMQFTP.ResumeLayout(False)
        Me.GrpSignatureMQFTP.ResumeLayout(False)
        Me.GrpSignatureMQFTP.PerformLayout()
        Me.TabMNBSigner.ResumeLayout(False)
        Me.TabMNBSigner.PerformLayout()
        CType(Me.BsMNBSignerCryptoProvider, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelMNBSigner.ResumeLayout(False)
        Me.GrpMNBSignerAuthentication.ResumeLayout(False)
        Me.GrpMNBSignerAuthentication.PerformLayout()
        Me.PanelMNBSignerAuthentication.ResumeLayout(False)
        Me.PanelMNBSignerAuthentication.PerformLayout()
        Me.GrpMNBSignerWCFSettings.ResumeLayout(False)
        Me.GrpMNBSignerWCFSettings.PerformLayout()
        CType(Me.NumMNBSignerChunkSize, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumMNBSignerSignTimeout, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NumMNBSignerWCFTimeout, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GrpFileNaming.ResumeLayout(False)
        Me.GrpFileNaming.PerformLayout()
        Me.PanelMain.ResumeLayout(False)
        Me.PanelMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents ChkSigningEnabled As System.Windows.Forms.CheckBox
    Private WithEvents GrpIndexes As System.Windows.Forms.GroupBox
    Private WithEvents ComboExpiry As System.Windows.Forms.ComboBox
    Private WithEvents Label5 As System.Windows.Forms.Label
    Private WithEvents ComboSignedBy As System.Windows.Forms.ComboBox
    Private WithEvents Label4 As System.Windows.Forms.Label
    Private WithEvents ComboSignedAt As System.Windows.Forms.ComboBox
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents ComboIsSigned As System.Windows.Forms.ComboBox
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents ComboBarCode As System.Windows.Forms.ComboBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents TxtSignatureMarker As System.Windows.Forms.TextBox
    Private WithEvents Label7 As System.Windows.Forms.Label
    Private WithEvents ChkAllDocumentsNeedSigning As System.Windows.Forms.CheckBox
    Private WithEvents Label6 As System.Windows.Forms.Label
    Private WithEvents ComboSignatureHashMethod As System.Windows.Forms.ComboBox
    Private WithEvents Label11 As System.Windows.Forms.Label
    Private WithEvents ComboRevocationCheck As System.Windows.Forms.ComboBox
    Private WithEvents Label10 As System.Windows.Forms.Label
    Private WithEvents TxtSigningReason As System.Windows.Forms.TextBox
    Private WithEvents Label9 As System.Windows.Forms.Label
    Private WithEvents TxtSigningOrganization As System.Windows.Forms.TextBox
    Private WithEvents Label8 As System.Windows.Forms.Label
    Private WithEvents TxtSignaturePolicyURL As System.Windows.Forms.TextBox
    Private WithEvents Label12 As System.Windows.Forms.Label
    Private WithEvents TxtSignaturePolicyOID As System.Windows.Forms.TextBox
    Private WithEvents Label14 As System.Windows.Forms.Label
    Private WithEvents TxtSignaturePolicyHash As System.Windows.Forms.TextBox
    Private WithEvents Label13 As System.Windows.Forms.Label
    Private WithEvents ComboTimeStampHashMethod As System.Windows.Forms.ComboBox
    Private WithEvents Label18 As System.Windows.Forms.Label
    Private WithEvents ChkIsDocumentTimeStamp As System.Windows.Forms.CheckBox
    Private WithEvents TxtTSAPassword As System.Windows.Forms.TextBox
    Private WithEvents Label16 As System.Windows.Forms.Label
    Private WithEvents TxtTSAUser As System.Windows.Forms.TextBox
    Private WithEvents Label17 As System.Windows.Forms.Label
    Private WithEvents TxtTSAURL As System.Windows.Forms.TextBox
    Private WithEvents Label15 As System.Windows.Forms.Label
    Private WithEvents ChkTimeStampingEnabled As System.Windows.Forms.CheckBox
    Private WithEvents TxtDocumentNameDefault As System.Windows.Forms.TextBox
    Private WithEvents Label20 As System.Windows.Forms.Label
    Private WithEvents ComboDocumentName As System.Windows.Forms.ComboBox
    Private WithEvents Label19 As System.Windows.Forms.Label
    Private WithEvents TxtCopyingOrganizationName As System.Windows.Forms.TextBox
    Private WithEvents Label21 As System.Windows.Forms.Label
    Private WithEvents ComboClauseFormat As System.Windows.Forms.ComboBox
    Private WithEvents Label25 As System.Windows.Forms.Label
    Private WithEvents Label23 As System.Windows.Forms.Label
    Private WithEvents TxtCopyingRegulationVersion As System.Windows.Forms.TextBox
    Private WithEvents Label24 As System.Windows.Forms.Label
    Private WithEvents TxtCopyingRegulationURL As System.Windows.Forms.TextBox
    Private WithEvents TxtCopyingRegulationName As System.Windows.Forms.TextBox
    Private WithEvents Label22 As System.Windows.Forms.Label
    Private WithEvents BsSetup As System.Windows.Forms.BindingSource
    Private WithEvents BtnImportSettings As System.Windows.Forms.Button
    Private WithEvents BtnOK As System.Windows.Forms.Button
    Private WithEvents BtnCancel As System.Windows.Forms.Button
    Private WithEvents BtnExportSettings As System.Windows.Forms.Button
    Private WithEvents LblCryptoProvider As System.Windows.Forms.Label
    Private WithEvents ComboCryptoProvider As System.Windows.Forms.ComboBox
    Private WithEvents ChkDocNameFromFormType As System.Windows.Forms.CheckBox
    Private WithEvents GrpBarCode As System.Windows.Forms.GroupBox
    Private WithEvents GrpTimeStamp As System.Windows.Forms.GroupBox
    Private WithEvents GrpClause As System.Windows.Forms.GroupBox
    Private WithEvents NumericSignatureMarkerPosition As System.Windows.Forms.NumericUpDown
    Private WithEvents BtnRemoveDocNameIndexAssignment As System.Windows.Forms.Button
    Private WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Private WithEvents OpenFileDialog As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog As System.Windows.Forms.SaveFileDialog
    Private WithEvents BtnResetSettings As System.Windows.Forms.Button
    Private WithEvents TabCryptoProviders As System.Windows.Forms.TabControl
    Private WithEvents GrpSignatureMQFTP As System.Windows.Forms.GroupBox
    Private WithEvents TxtMQPassword As System.Windows.Forms.TextBox
    Private WithEvents Label26 As System.Windows.Forms.Label
    Private WithEvents TxtMQUserName As System.Windows.Forms.TextBox
    Private WithEvents Label27 As System.Windows.Forms.Label
    Private WithEvents TxtMQFolderOut As System.Windows.Forms.TextBox
    Private WithEvents Label28 As System.Windows.Forms.Label
    Private WithEvents TabPDFStreamer As System.Windows.Forms.TabPage
    Private WithEvents TabPDFSigner As System.Windows.Forms.TabPage
    Private WithEvents TabMQ As System.Windows.Forms.TabPage
    Private WithEvents GrpSignaturePDFStreamer As System.Windows.Forms.GroupBox
    Private WithEvents TxtPDFStreamerAuthorizationCode As System.Windows.Forms.TextBox
    Private WithEvents Label29 As System.Windows.Forms.Label
    Private WithEvents TxtPDFStreamerConfigurationFileName As System.Windows.Forms.TextBox
    Private WithEvents Label30 As System.Windows.Forms.Label
    Private WithEvents TxtPDFStreamerURL As System.Windows.Forms.TextBox
    Private WithEvents Label31 As System.Windows.Forms.Label
    Private WithEvents GrpFileNaming As System.Windows.Forms.GroupBox
    Private WithEvents TxtFileExtension As System.Windows.Forms.TextBox
    Private WithEvents LblFileExtension As System.Windows.Forms.Label
    Private WithEvents TxtFileAppend As System.Windows.Forms.TextBox
    Private WithEvents LblFileAppend As System.Windows.Forms.Label
    Private WithEvents RadioCreateNewFile As System.Windows.Forms.RadioButton
    Private WithEvents RadioOverwriteOriginal As System.Windows.Forms.RadioButton
    Private WithEvents TxtMQFolderIn As System.Windows.Forms.TextBox
    Private WithEvents Label32 As System.Windows.Forms.Label
    Private WithEvents Label33 As System.Windows.Forms.Label
    Private WithEvents TxtMQDomain As System.Windows.Forms.TextBox
    Private WithEvents Label35 As System.Windows.Forms.Label
    Private WithEvents BtnRemoveIndexBarCode As System.Windows.Forms.Button
    Private WithEvents BtnRemoveIndexSignatureExpiry As System.Windows.Forms.Button
    Private WithEvents BtnRemoveIndexSigningUser As System.Windows.Forms.Button
    Private WithEvents BtnRemoveIndexSignDateTime As System.Windows.Forms.Button
    Private WithEvents BtnRemoveIndexIsSigned As System.Windows.Forms.Button
    Private WithEvents BtnRemoveIndexDocumentUID As System.Windows.Forms.Button
    Private WithEvents ComboMQDocUID As System.Windows.Forms.ComboBox
    Private WithEvents Label34 As System.Windows.Forms.Label
    Private WithEvents ChkSinglePassPadesLTA As System.Windows.Forms.CheckBox
    Private WithEvents ChkProxyEnabled As System.Windows.Forms.CheckBox
    Private WithEvents GrpProxy As System.Windows.Forms.GroupBox
    Private WithEvents TxtProxyPassword As System.Windows.Forms.TextBox
    Private WithEvents Label36 As System.Windows.Forms.Label
    Private WithEvents TxtProxyUser As System.Windows.Forms.TextBox
    Private WithEvents Label37 As System.Windows.Forms.Label
    Private WithEvents TxtProxyServer As System.Windows.Forms.TextBox
    Private WithEvents Label38 As System.Windows.Forms.Label
    Private WithEvents NumPort As System.Windows.Forms.NumericUpDown
    Private WithEvents Label39 As System.Windows.Forms.Label
    Private WithEvents ComboProxyAuthMethod As System.Windows.Forms.ComboBox
    Private WithEvents Label40 As System.Windows.Forms.Label
    Private WithEvents ChkAllowSkip As System.Windows.Forms.CheckBox
    Private WithEvents Label41 As System.Windows.Forms.Label
    Private WithEvents ComboDocumentViewer As System.Windows.Forms.ComboBox
    Private WithEvents BtnRemoveIndexSkipNote As System.Windows.Forms.Button
    Private WithEvents ComboSkipNote As System.Windows.Forms.ComboBox
    Private WithEvents Label42 As System.Windows.Forms.Label
    Private WithEvents TabMNBSigner As TabPage
    Private WithEvents GrpMNBSignerWCFSettings As GroupBox
    Private WithEvents Label48 As Label
    Private WithEvents Label49 As Label
    Private WithEvents NumMNBSignerChunkSize As NumericUpDown
    Private WithEvents Label46 As Label
    Private WithEvents Label47 As Label
    Private WithEvents NumMNBSignerSignTimeout As NumericUpDown
    Private WithEvents Label45 As Label
    Private WithEvents Label44 As Label
    Private WithEvents NumMNBSignerWCFTimeout As NumericUpDown
    Private WithEvents TxtMNBSignerURL As TextBox
    Private WithEvents Label43 As Label
    Private WithEvents GrpMNBSignerAuthentication As GroupBox
    Private WithEvents RadioMNBSignerPasswordAuthentication As RadioButton
    Private WithEvents RadioMNBSignerWindowsAuthentication As RadioButton
    Private WithEvents Label50 As Label
    Private WithEvents TxtMNBSignerDomain As TextBox
    Private WithEvents TxtMNBSignerUserName As TextBox
    Private WithEvents TxtMNBSignerPassword As TextBox
    Private WithEvents Label51 As Label
    Private WithEvents Label52 As Label
    Private WithEvents PanelPDFSigner As Panel
    Private WithEvents ChkPDFSignerEnabled As CheckBox
    Private WithEvents PanelPDFStreamer As Panel
    Private WithEvents ChkPDFStreamerEnabled As CheckBox
    Private WithEvents PanelMQFTP As Panel
    Private WithEvents ChkMQEnabled As CheckBox
    Private WithEvents PanelMNBSigner As Panel
    Private WithEvents ChkMNBSignerEnabled As CheckBox
    Private WithEvents PanelMNBSignerAuthentication As Panel
    Friend WithEvents PanelMain As Panel
    Private WithEvents ChkAllowQualifiedCertificatesOnly As CheckBox
    Private WithEvents BsPDFSignerCryptoProvider As BindingSource
    Private WithEvents BsPDFStreamerCryptoProvider As BindingSource
    Private WithEvents BsMQFTPCryptoProvider As BindingSource
    Private WithEvents BsMNBSignerCryptoProvider As BindingSource
End Class
