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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FrmSetup))
        BtnOK = New Button()
        BtnCancel = New Button()
        ChkSigningEnabled = New CheckBox()
        BsSetup = New BindingSource(components)
        GrpIndexes = New GroupBox()
        BtnRemoveIndexSkipNote = New Button()
        ComboSkipNote = New ComboBox()
        Label42 = New Label()
        BtnRemoveIndexSignatureExpiry = New Button()
        BtnRemoveIndexSigningUser = New Button()
        BtnRemoveIndexSignDateTime = New Button()
        BtnRemoveIndexIsSigned = New Button()
        ComboExpiry = New ComboBox()
        Label5 = New Label()
        ComboSignedBy = New ComboBox()
        Label4 = New Label()
        ComboSignedAt = New ComboBox()
        Label3 = New Label()
        ComboIsSigned = New ComboBox()
        Label2 = New Label()
        ComboBarCode = New ComboBox()
        Label1 = New Label()
        GrpBarCode = New GroupBox()
        ChkAllowSkip = New CheckBox()
        Label35 = New Label()
        BtnRemoveIndexBarCode = New Button()
        NumericSignatureMarkerPosition = New NumericUpDown()
        TxtSignatureMarker = New TextBox()
        Label7 = New Label()
        ChkAllDocumentsNeedSigning = New CheckBox()
        Label6 = New Label()
        ComboSignatureHashMethod = New ComboBox()
        BsPDFSignerCryptoProvider = New BindingSource(components)
        Label11 = New Label()
        ComboRevocationCheckProtocol = New ComboBox()
        LblPAdESLevel = New Label()
        TxtSigningReason = New TextBox()
        Label9 = New Label()
        TxtSigningOrganization = New TextBox()
        Label8 = New Label()
        TxtSignaturePolicyOID = New TextBox()
        Label14 = New Label()
        TxtSignaturePolicyHash = New TextBox()
        Label13 = New Label()
        TxtSignaturePolicyURL = New TextBox()
        Label12 = New Label()
        GrpTimeStamp = New GroupBox()
        ComboTimeStampHashMethod = New ComboBox()
        Label18 = New Label()
        TxtTSAPassword = New TextBox()
        Label16 = New Label()
        TxtTSAUser = New TextBox()
        Label17 = New Label()
        TxtTSAURL = New TextBox()
        Label15 = New Label()
        GrpClause = New GroupBox()
        BtnRemoveDocNameIndexAssignment = New Button()
        ChkDocNameFromFormType = New CheckBox()
        ComboClauseFormat = New ComboBox()
        Label25 = New Label()
        Label23 = New Label()
        TxtCopyingRegulationVersion = New TextBox()
        Label24 = New Label()
        TxtCopyingRegulationURL = New TextBox()
        TxtCopyingRegulationName = New TextBox()
        Label22 = New Label()
        TxtCopyingOrganizationName = New TextBox()
        Label21 = New Label()
        TxtDocumentNameDefault = New TextBox()
        Label20 = New Label()
        ComboDocumentName = New ComboBox()
        Label19 = New Label()
        BtnImportSettings = New Button()
        BtnExportSettings = New Button()
        LblCryptoProvider = New Label()
        ComboCryptoProvider = New ComboBox()
        ToolTip1 = New ToolTip(components)
        BtnRemoveIndexDocumentUID = New Button()
        BtnResetSettings = New Button()
        TabCryptoProviders = New TabControl()
        TabPDFSigner = New TabPage()
        ChkPDFSignerEnabled = New CheckBox()
        PanelPDFSigner = New Panel()
        ChkAllowQualifiedCertificatesOnly = New CheckBox()
        ChkProxyEnabled = New CheckBox()
        GrpProxy = New GroupBox()
        ComboProxyAuthMethod = New ComboBox()
        Label40 = New Label()
        NumPort = New NumericUpDown()
        Label39 = New Label()
        TxtProxyPassword = New TextBox()
        Label36 = New Label()
        TxtProxyUser = New TextBox()
        Label37 = New Label()
        TxtProxyServer = New TextBox()
        Label38 = New Label()
        ChkSignerNameFromLoggedOnUser = New CheckBox()
        ComboPAdESLevel = New ComboBox()
        GrpRevocation = New GroupBox()
        ChkEnableRevocationChecking = New CheckBox()
        LblRevocationProtocol = New Label()
        ChkEmbedRevocationInformation = New CheckBox()
        TabPDFStreamer = New TabPage()
        ChkPDFStreamerEnabled = New CheckBox()
        BsPDFStreamerCryptoProvider = New BindingSource(components)
        PanelPDFStreamer = New Panel()
        GrpSignaturePDFStreamer = New GroupBox()
        TxtPDFStreamerAuthorizationCode = New TextBox()
        Label29 = New Label()
        TxtPDFStreamerConfigurationFileName = New TextBox()
        Label30 = New Label()
        TxtPDFStreamerURL = New TextBox()
        Label31 = New Label()
        TabMQ = New TabPage()
        ChkMQEnabled = New CheckBox()
        BsMQFTPCryptoProvider = New BindingSource(components)
        PanelMQFTP = New Panel()
        GrpSignatureMQFTP = New GroupBox()
        ComboMQDocUID = New ComboBox()
        Label34 = New Label()
        Label33 = New Label()
        TxtMQDomain = New TextBox()
        TxtMQUserName = New TextBox()
        TxtMQFolderIn = New TextBox()
        Label32 = New Label()
        TxtMQPassword = New TextBox()
        Label26 = New Label()
        Label27 = New Label()
        TxtMQFolderOut = New TextBox()
        Label28 = New Label()
        TabMNBSigner = New TabPage()
        ChkMNBSignerEnabled = New CheckBox()
        BsMNBSignerCryptoProvider = New BindingSource(components)
        PanelMNBSigner = New Panel()
        GrpMNBSignerAuthentication = New GroupBox()
        RadioMNBSignerPasswordAuthentication = New RadioButton()
        RadioMNBSignerWindowsAuthentication = New RadioButton()
        PanelMNBSignerAuthentication = New Panel()
        Label50 = New Label()
        TxtMNBSignerDomain = New TextBox()
        TxtMNBSignerUserName = New TextBox()
        TxtMNBSignerPassword = New TextBox()
        Label51 = New Label()
        Label52 = New Label()
        GrpMNBSignerWCFSettings = New GroupBox()
        Label48 = New Label()
        Label49 = New Label()
        NumMNBSignerChunkSize = New NumericUpDown()
        Label46 = New Label()
        Label47 = New Label()
        NumMNBSignerSignTimeout = New NumericUpDown()
        Label45 = New Label()
        Label44 = New Label()
        NumMNBSignerWCFTimeout = New NumericUpDown()
        TxtMNBSignerURL = New TextBox()
        Label43 = New Label()
        GrpFileNaming = New GroupBox()
        TxtFileExtension = New TextBox()
        LblFileExtension = New Label()
        TxtFileAppend = New TextBox()
        LblFileAppend = New Label()
        RadioCreateNewFile = New RadioButton()
        RadioOverwriteOriginal = New RadioButton()
        Label41 = New Label()
        ComboDocumentViewer = New ComboBox()
        PanelMain = New Panel()
        OpenFileDialog = New OpenFileDialog()
        SaveFileDialog = New SaveFileDialog()
        CType(BsSetup, ComponentModel.ISupportInitialize).BeginInit()
        GrpIndexes.SuspendLayout()
        GrpBarCode.SuspendLayout()
        CType(NumericSignatureMarkerPosition, ComponentModel.ISupportInitialize).BeginInit()
        CType(BsPDFSignerCryptoProvider, ComponentModel.ISupportInitialize).BeginInit()
        GrpTimeStamp.SuspendLayout()
        GrpClause.SuspendLayout()
        TabCryptoProviders.SuspendLayout()
        TabPDFSigner.SuspendLayout()
        PanelPDFSigner.SuspendLayout()
        GrpProxy.SuspendLayout()
        GrpRevocation.SuspendLayout()
        CType(NumPort, ComponentModel.ISupportInitialize).BeginInit()
        TabPDFStreamer.SuspendLayout()
        CType(BsPDFStreamerCryptoProvider, ComponentModel.ISupportInitialize).BeginInit()
        PanelPDFStreamer.SuspendLayout()
        GrpSignaturePDFStreamer.SuspendLayout()
        TabMQ.SuspendLayout()
        CType(BsMQFTPCryptoProvider, ComponentModel.ISupportInitialize).BeginInit()
        PanelMQFTP.SuspendLayout()
        GrpSignatureMQFTP.SuspendLayout()
        TabMNBSigner.SuspendLayout()
        CType(BsMNBSignerCryptoProvider, ComponentModel.ISupportInitialize).BeginInit()
        PanelMNBSigner.SuspendLayout()
        GrpMNBSignerAuthentication.SuspendLayout()
        PanelMNBSignerAuthentication.SuspendLayout()
        GrpMNBSignerWCFSettings.SuspendLayout()
        CType(NumMNBSignerChunkSize, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumMNBSignerSignTimeout, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumMNBSignerWCFTimeout, ComponentModel.ISupportInitialize).BeginInit()
        GrpFileNaming.SuspendLayout()
        PanelMain.SuspendLayout()
        SuspendLayout()
        ' 
        ' BtnOK
        ' 
        resources.ApplyResources(BtnOK, "BtnOK")
        BtnOK.Name = "BtnOK"
        BtnOK.UseVisualStyleBackColor = True
        ' 
        ' BtnCancel
        ' 
        resources.ApplyResources(BtnCancel, "BtnCancel")
        BtnCancel.DialogResult = DialogResult.Cancel
        BtnCancel.Name = "BtnCancel"
        BtnCancel.UseVisualStyleBackColor = True
        ' 
        ' ChkSigningEnabled
        ' 
        resources.ApplyResources(ChkSigningEnabled, "ChkSigningEnabled")
        ChkSigningEnabled.DataBindings.Add(New Binding("Checked", BsSetup, "IsSigningEnabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkSigningEnabled.Name = "ChkSigningEnabled"
        ChkSigningEnabled.UseVisualStyleBackColor = True
        ' 
        ' BsSetup
        ' 
        BsSetup.DataSource = GetType(PDFSignerCommon.SetupModel)
        ' 
        ' GrpIndexes
        ' 
        resources.ApplyResources(GrpIndexes, "GrpIndexes")
        GrpIndexes.Controls.Add(BtnRemoveIndexSkipNote)
        GrpIndexes.Controls.Add(ComboSkipNote)
        GrpIndexes.Controls.Add(Label42)
        GrpIndexes.Controls.Add(BtnRemoveIndexSignatureExpiry)
        GrpIndexes.Controls.Add(BtnRemoveIndexSigningUser)
        GrpIndexes.Controls.Add(BtnRemoveIndexSignDateTime)
        GrpIndexes.Controls.Add(BtnRemoveIndexIsSigned)
        GrpIndexes.Controls.Add(ComboExpiry)
        GrpIndexes.Controls.Add(Label5)
        GrpIndexes.Controls.Add(ComboSignedBy)
        GrpIndexes.Controls.Add(Label4)
        GrpIndexes.Controls.Add(ComboSignedAt)
        GrpIndexes.Controls.Add(Label3)
        GrpIndexes.Controls.Add(ComboIsSigned)
        GrpIndexes.Controls.Add(Label2)
        GrpIndexes.Name = "GrpIndexes"
        GrpIndexes.TabStop = False
        ' 
        ' BtnRemoveIndexSkipNote
        ' 
        resources.ApplyResources(BtnRemoveIndexSkipNote, "BtnRemoveIndexSkipNote")
        BtnRemoveIndexSkipNote.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexSkipNote.Name = "BtnRemoveIndexSkipNote"
        BtnRemoveIndexSkipNote.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexSkipNote, resources.GetString("BtnRemoveIndexSkipNote.ToolTip"))
        BtnRemoveIndexSkipNote.UseVisualStyleBackColor = True
        ' 
        ' ComboSkipNote
        ' 
        resources.ApplyResources(ComboSkipNote, "ComboSkipNote")
        ComboSkipNote.DataBindings.Add(New Binding("Text", BsSetup, "IndexSkipNote", True))
        ComboSkipNote.DropDownStyle = ComboBoxStyle.DropDownList
        ComboSkipNote.FormattingEnabled = True
        ComboSkipNote.Name = "ComboSkipNote"
        ' 
        ' Label42
        ' 
        resources.ApplyResources(Label42, "Label42")
        Label42.Name = "Label42"
        ' 
        ' BtnRemoveIndexSignatureExpiry
        ' 
        resources.ApplyResources(BtnRemoveIndexSignatureExpiry, "BtnRemoveIndexSignatureExpiry")
        BtnRemoveIndexSignatureExpiry.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexSignatureExpiry.Name = "BtnRemoveIndexSignatureExpiry"
        BtnRemoveIndexSignatureExpiry.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexSignatureExpiry, resources.GetString("BtnRemoveIndexSignatureExpiry.ToolTip"))
        BtnRemoveIndexSignatureExpiry.UseVisualStyleBackColor = True
        ' 
        ' BtnRemoveIndexSigningUser
        ' 
        resources.ApplyResources(BtnRemoveIndexSigningUser, "BtnRemoveIndexSigningUser")
        BtnRemoveIndexSigningUser.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexSigningUser.Name = "BtnRemoveIndexSigningUser"
        BtnRemoveIndexSigningUser.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexSigningUser, resources.GetString("BtnRemoveIndexSigningUser.ToolTip"))
        BtnRemoveIndexSigningUser.UseVisualStyleBackColor = True
        ' 
        ' BtnRemoveIndexSignDateTime
        ' 
        resources.ApplyResources(BtnRemoveIndexSignDateTime, "BtnRemoveIndexSignDateTime")
        BtnRemoveIndexSignDateTime.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexSignDateTime.Name = "BtnRemoveIndexSignDateTime"
        BtnRemoveIndexSignDateTime.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexSignDateTime, resources.GetString("BtnRemoveIndexSignDateTime.ToolTip"))
        BtnRemoveIndexSignDateTime.UseVisualStyleBackColor = True
        ' 
        ' BtnRemoveIndexIsSigned
        ' 
        resources.ApplyResources(BtnRemoveIndexIsSigned, "BtnRemoveIndexIsSigned")
        BtnRemoveIndexIsSigned.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexIsSigned.Name = "BtnRemoveIndexIsSigned"
        BtnRemoveIndexIsSigned.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexIsSigned, resources.GetString("BtnRemoveIndexIsSigned.ToolTip"))
        BtnRemoveIndexIsSigned.UseVisualStyleBackColor = True
        ' 
        ' ComboExpiry
        ' 
        resources.ApplyResources(ComboExpiry, "ComboExpiry")
        ComboExpiry.DataBindings.Add(New Binding("Text", BsSetup, "IndexSignatureValidUntil", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboExpiry.DropDownStyle = ComboBoxStyle.DropDownList
        ComboExpiry.FormattingEnabled = True
        ComboExpiry.Name = "ComboExpiry"
        ' 
        ' Label5
        ' 
        resources.ApplyResources(Label5, "Label5")
        Label5.Name = "Label5"
        ' 
        ' ComboSignedBy
        ' 
        resources.ApplyResources(ComboSignedBy, "ComboSignedBy")
        ComboSignedBy.DataBindings.Add(New Binding("Text", BsSetup, "IndexSignedBy", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboSignedBy.DropDownStyle = ComboBoxStyle.DropDownList
        ComboSignedBy.FormattingEnabled = True
        ComboSignedBy.Name = "ComboSignedBy"
        ' 
        ' Label4
        ' 
        resources.ApplyResources(Label4, "Label4")
        Label4.Name = "Label4"
        ' 
        ' ComboSignedAt
        ' 
        resources.ApplyResources(ComboSignedAt, "ComboSignedAt")
        ComboSignedAt.DataBindings.Add(New Binding("Text", BsSetup, "IndexSignedDateTime", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboSignedAt.DropDownStyle = ComboBoxStyle.DropDownList
        ComboSignedAt.FormattingEnabled = True
        ComboSignedAt.Name = "ComboSignedAt"
        ' 
        ' Label3
        ' 
        resources.ApplyResources(Label3, "Label3")
        Label3.Name = "Label3"
        ' 
        ' ComboIsSigned
        ' 
        resources.ApplyResources(ComboIsSigned, "ComboIsSigned")
        ComboIsSigned.DataBindings.Add(New Binding("Text", BsSetup, "IndexIsSigned", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboIsSigned.DropDownStyle = ComboBoxStyle.DropDownList
        ComboIsSigned.FormattingEnabled = True
        ComboIsSigned.Name = "ComboIsSigned"
        ' 
        ' Label2
        ' 
        resources.ApplyResources(Label2, "Label2")
        Label2.Name = "Label2"
        ' 
        ' ComboBarCode
        ' 
        resources.ApplyResources(ComboBarCode, "ComboBarCode")
        ComboBarCode.DataBindings.Add(New Binding("Text", BsSetup, "IndexBarCode", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboBarCode.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBarCode.FormattingEnabled = True
        ComboBarCode.Name = "ComboBarCode"
        ' 
        ' Label1
        ' 
        resources.ApplyResources(Label1, "Label1")
        Label1.Name = "Label1"
        ' 
        ' GrpBarCode
        ' 
        resources.ApplyResources(GrpBarCode, "GrpBarCode")
        GrpBarCode.Controls.Add(ChkAllowSkip)
        GrpBarCode.Controls.Add(Label35)
        GrpBarCode.Controls.Add(BtnRemoveIndexBarCode)
        GrpBarCode.Controls.Add(NumericSignatureMarkerPosition)
        GrpBarCode.Controls.Add(TxtSignatureMarker)
        GrpBarCode.Controls.Add(Label7)
        GrpBarCode.Controls.Add(ChkAllDocumentsNeedSigning)
        GrpBarCode.Controls.Add(Label6)
        GrpBarCode.Controls.Add(Label1)
        GrpBarCode.Controls.Add(ComboBarCode)
        GrpBarCode.Name = "GrpBarCode"
        GrpBarCode.TabStop = False
        ' 
        ' ChkAllowSkip
        ' 
        resources.ApplyResources(ChkAllowSkip, "ChkAllowSkip")
        ChkAllowSkip.DataBindings.Add(New Binding("Checked", BsSetup, "AllowSkipRequiredDocument", True))
        ChkAllowSkip.Name = "ChkAllowSkip"
        ChkAllowSkip.UseVisualStyleBackColor = True
        ' 
        ' Label35
        ' 
        resources.ApplyResources(Label35, "Label35")
        Label35.Name = "Label35"
        ' 
        ' BtnRemoveIndexBarCode
        ' 
        resources.ApplyResources(BtnRemoveIndexBarCode, "BtnRemoveIndexBarCode")
        BtnRemoveIndexBarCode.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexBarCode.Name = "BtnRemoveIndexBarCode"
        BtnRemoveIndexBarCode.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexBarCode, resources.GetString("BtnRemoveIndexBarCode.ToolTip"))
        BtnRemoveIndexBarCode.UseVisualStyleBackColor = True
        ' 
        ' NumericSignatureMarkerPosition
        ' 
        NumericSignatureMarkerPosition.DataBindings.Add(New Binding("Value", BsSetup, "SignatureMarkerPosition", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(NumericSignatureMarkerPosition, "NumericSignatureMarkerPosition")
        NumericSignatureMarkerPosition.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        NumericSignatureMarkerPosition.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        NumericSignatureMarkerPosition.Name = "NumericSignatureMarkerPosition"
        NumericSignatureMarkerPosition.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' TxtSignatureMarker
        ' 
        TxtSignatureMarker.DataBindings.Add(New Binding("Text", BsSetup, "SignatureMarker", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(TxtSignatureMarker, "TxtSignatureMarker")
        TxtSignatureMarker.Name = "TxtSignatureMarker"
        ' 
        ' Label7
        ' 
        resources.ApplyResources(Label7, "Label7")
        Label7.Name = "Label7"
        ' 
        ' ChkAllDocumentsNeedSigning
        ' 
        resources.ApplyResources(ChkAllDocumentsNeedSigning, "ChkAllDocumentsNeedSigning")
        ChkAllDocumentsNeedSigning.DataBindings.Add(New Binding("Checked", BsSetup, "SignAllDocuments", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkAllDocumentsNeedSigning.Name = "ChkAllDocumentsNeedSigning"
        ChkAllDocumentsNeedSigning.UseVisualStyleBackColor = True
        ' 
        ' Label6
        ' 
        resources.ApplyResources(Label6, "Label6")
        Label6.Name = "Label6"
        ' 
        ' ComboSignatureHashMethod
        ' 
        resources.ApplyResources(ComboSignatureHashMethod, "ComboSignatureHashMethod")
        ComboSignatureHashMethod.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "SignatureHashMethod", True))
        ComboSignatureHashMethod.DropDownStyle = ComboBoxStyle.DropDownList
        ComboSignatureHashMethod.FormattingEnabled = True
        ComboSignatureHashMethod.Name = "ComboSignatureHashMethod"
        ' 
        ' BsPDFSignerCryptoProvider
        ' 
        BsPDFSignerCryptoProvider.DataSource = GetType(PDFSignerCommon.PDFSignerCryptoProvider)
        ' 
        ' Label11
        ' 
        resources.ApplyResources(Label11, "Label11")
        Label11.Name = "Label11"
        ' 
        ' ComboRevocationCheckProtocol
        ' 
        resources.ApplyResources(ComboRevocationCheckProtocol, "ComboRevocationCheckProtocol")
        ComboRevocationCheckProtocol.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "RevocationCheckProtocol", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboRevocationCheckProtocol.DropDownStyle = ComboBoxStyle.DropDownList
        ComboRevocationCheckProtocol.FormattingEnabled = True
        ComboRevocationCheckProtocol.Name = "ComboRevocationCheckProtocol"
        ' 
        ' LblPAdESLevel
        ' 
        resources.ApplyResources(LblPAdESLevel, "LblPAdESLevel")
        LblPAdESLevel.Name = "LblPAdESLevel"
        ' 
        ' ComboPAdESLevel
        ' 
        resources.ApplyResources(ComboPAdESLevel, "ComboPAdESLevel")
        ComboPAdESLevel.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "PAdESLevel", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboPAdESLevel.DropDownStyle = ComboBoxStyle.DropDownList
        ComboPAdESLevel.FormattingEnabled = True
        ComboPAdESLevel.Name = "ComboPAdESLevel"
        ' 
        ' ChkEnableRevocationChecking
        ' 
        resources.ApplyResources(ChkEnableRevocationChecking, "ChkEnableRevocationChecking")
        ChkEnableRevocationChecking.BackColor = Drawing.SystemColors.ControlLightLight
        ChkEnableRevocationChecking.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "EnableRevocationChecking", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkEnableRevocationChecking.Name = "ChkEnableRevocationChecking"
        ChkEnableRevocationChecking.UseVisualStyleBackColor = False
        ' 
        ' GrpRevocation
        ' 
        resources.ApplyResources(GrpRevocation, "GrpRevocation")
        GrpRevocation.Controls.Add(ChkEmbedRevocationInformation)
        GrpRevocation.Controls.Add(ComboRevocationCheckProtocol)
        GrpRevocation.Controls.Add(LblRevocationProtocol)
        GrpRevocation.Name = "GrpRevocation"
        GrpRevocation.TabStop = False
        ' 
        ' LblRevocationProtocol
        ' 
        resources.ApplyResources(LblRevocationProtocol, "LblRevocationProtocol")
        LblRevocationProtocol.Name = "LblRevocationProtocol"
        ' 
        ' ChkEmbedRevocationInformation
        ' 
        resources.ApplyResources(ChkEmbedRevocationInformation, "ChkEmbedRevocationInformation")
        ChkEmbedRevocationInformation.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "EmbedRevocationInformation", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkEmbedRevocationInformation.Name = "ChkEmbedRevocationInformation"
        ChkEmbedRevocationInformation.UseVisualStyleBackColor = True
        ' 
        ' TxtSigningReason
        ' 
        resources.ApplyResources(TxtSigningReason, "TxtSigningReason")
        TxtSigningReason.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "SigningReason", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtSigningReason.Name = "TxtSigningReason"
        ' 
        ' Label9
        ' 
        resources.ApplyResources(Label9, "Label9")
        Label9.Name = "Label9"
        ' 
        ' TxtSigningOrganization
        ' 
        resources.ApplyResources(TxtSigningOrganization, "TxtSigningOrganization")
        TxtSigningOrganization.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "SigningOrganization", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtSigningOrganization.Name = "TxtSigningOrganization"
        ' 
        ' Label8
        ' 
        resources.ApplyResources(Label8, "Label8")
        Label8.Name = "Label8"
        ' 
        ' TxtSignaturePolicyOID
        ' 
        resources.ApplyResources(TxtSignaturePolicyOID, "TxtSignaturePolicyOID")
        TxtSignaturePolicyOID.DataBindings.Add(New Binding("Text", BsSetup, "SignaturePolicyOID", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtSignaturePolicyOID.Name = "TxtSignaturePolicyOID"
        ' 
        ' Label14
        ' 
        resources.ApplyResources(Label14, "Label14")
        Label14.Name = "Label14"
        ' 
        ' TxtSignaturePolicyHash
        ' 
        resources.ApplyResources(TxtSignaturePolicyHash, "TxtSignaturePolicyHash")
        TxtSignaturePolicyHash.DataBindings.Add(New Binding("Text", BsSetup, "SignaturePolicyHash", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtSignaturePolicyHash.Name = "TxtSignaturePolicyHash"
        ' 
        ' Label13
        ' 
        resources.ApplyResources(Label13, "Label13")
        Label13.Name = "Label13"
        ' 
        ' TxtSignaturePolicyURL
        ' 
        resources.ApplyResources(TxtSignaturePolicyURL, "TxtSignaturePolicyURL")
        TxtSignaturePolicyURL.DataBindings.Add(New Binding("Text", BsSetup, "SignaturePolicyURL", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtSignaturePolicyURL.Name = "TxtSignaturePolicyURL"
        ' 
        ' Label12
        ' 
        resources.ApplyResources(Label12, "Label12")
        Label12.Name = "Label12"
        ' 
        ' GrpTimeStamp
        ' 
        resources.ApplyResources(GrpTimeStamp, "GrpTimeStamp")
        GrpTimeStamp.Controls.Add(ComboTimeStampHashMethod)
        GrpTimeStamp.Controls.Add(Label18)
        GrpTimeStamp.Controls.Add(TxtTSAPassword)
        GrpTimeStamp.Controls.Add(Label16)
        GrpTimeStamp.Controls.Add(TxtTSAUser)
        GrpTimeStamp.Controls.Add(Label17)
        GrpTimeStamp.Controls.Add(TxtTSAURL)
        GrpTimeStamp.Controls.Add(Label15)
        GrpTimeStamp.Name = "GrpTimeStamp"
        GrpTimeStamp.TabStop = False
        ' 
        ' ComboTimeStampHashMethod
        ' 
        resources.ApplyResources(ComboTimeStampHashMethod, "ComboTimeStampHashMethod")
        ComboTimeStampHashMethod.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "TimeStampHashMethod", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboTimeStampHashMethod.DropDownStyle = ComboBoxStyle.DropDownList
        ComboTimeStampHashMethod.FormattingEnabled = True
        ComboTimeStampHashMethod.Name = "ComboTimeStampHashMethod"
        ' 
        ' Label18
        ' 
        resources.ApplyResources(Label18, "Label18")
        Label18.Name = "Label18"
        ' 
        ' TxtTSAPassword
        ' 
        resources.ApplyResources(TxtTSAPassword, "TxtTSAPassword")
        TxtTSAPassword.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "TSAPassword", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtTSAPassword.Name = "TxtTSAPassword"
        ' 
        ' Label16
        ' 
        resources.ApplyResources(Label16, "Label16")
        Label16.Name = "Label16"
        ' 
        ' TxtTSAUser
        ' 
        resources.ApplyResources(TxtTSAUser, "TxtTSAUser")
        TxtTSAUser.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "TSAUserName", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtTSAUser.Name = "TxtTSAUser"
        ' 
        ' Label17
        ' 
        resources.ApplyResources(Label17, "Label17")
        Label17.Name = "Label17"
        ' 
        ' TxtTSAURL
        ' 
        resources.ApplyResources(TxtTSAURL, "TxtTSAURL")
        TxtTSAURL.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "TSAURL", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtTSAURL.Name = "TxtTSAURL"
        ' 
        ' Label15
        ' 
        resources.ApplyResources(Label15, "Label15")
        Label15.Name = "Label15"
        ' 
        ' GrpClause
        ' 
        resources.ApplyResources(GrpClause, "GrpClause")
        GrpClause.Controls.Add(TxtSignaturePolicyOID)
        GrpClause.Controls.Add(BtnRemoveDocNameIndexAssignment)
        GrpClause.Controls.Add(Label14)
        GrpClause.Controls.Add(ChkDocNameFromFormType)
        GrpClause.Controls.Add(TxtSignaturePolicyHash)
        GrpClause.Controls.Add(ComboClauseFormat)
        GrpClause.Controls.Add(Label13)
        GrpClause.Controls.Add(Label25)
        GrpClause.Controls.Add(TxtSignaturePolicyURL)
        GrpClause.Controls.Add(Label23)
        GrpClause.Controls.Add(Label12)
        GrpClause.Controls.Add(TxtCopyingRegulationVersion)
        GrpClause.Controls.Add(Label24)
        GrpClause.Controls.Add(TxtCopyingRegulationURL)
        GrpClause.Controls.Add(TxtCopyingRegulationName)
        GrpClause.Controls.Add(Label22)
        GrpClause.Controls.Add(TxtCopyingOrganizationName)
        GrpClause.Controls.Add(Label21)
        GrpClause.Controls.Add(TxtDocumentNameDefault)
        GrpClause.Controls.Add(Label20)
        GrpClause.Controls.Add(ComboDocumentName)
        GrpClause.Controls.Add(Label19)
        GrpClause.Name = "GrpClause"
        GrpClause.TabStop = False
        ' 
        ' BtnRemoveDocNameIndexAssignment
        ' 
        resources.ApplyResources(BtnRemoveDocNameIndexAssignment, "BtnRemoveDocNameIndexAssignment")
        BtnRemoveDocNameIndexAssignment.Image = My.Resources.Resources.Delete16
        BtnRemoveDocNameIndexAssignment.Name = "BtnRemoveDocNameIndexAssignment"
        BtnRemoveDocNameIndexAssignment.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveDocNameIndexAssignment, resources.GetString("BtnRemoveDocNameIndexAssignment.ToolTip"))
        BtnRemoveDocNameIndexAssignment.UseVisualStyleBackColor = True
        ' 
        ' ChkDocNameFromFormType
        ' 
        resources.ApplyResources(ChkDocNameFromFormType, "ChkDocNameFromFormType")
        ChkDocNameFromFormType.DataBindings.Add(New Binding("Checked", BsSetup, "DocumentNameFromFormType", True))
        ChkDocNameFromFormType.Name = "ChkDocNameFromFormType"
        ChkDocNameFromFormType.UseVisualStyleBackColor = True
        ' 
        ' ComboClauseFormat
        ' 
        resources.ApplyResources(ComboClauseFormat, "ComboClauseFormat")
        ComboClauseFormat.DataBindings.Add(New Binding("SelectedValue", BsSetup, "MetadataFormat", True))
        ComboClauseFormat.DropDownStyle = ComboBoxStyle.DropDownList
        ComboClauseFormat.FormattingEnabled = True
        ComboClauseFormat.Name = "ComboClauseFormat"
        ' 
        ' Label25
        ' 
        resources.ApplyResources(Label25, "Label25")
        Label25.Name = "Label25"
        ' 
        ' Label23
        ' 
        resources.ApplyResources(Label23, "Label23")
        Label23.Name = "Label23"
        ' 
        ' TxtCopyingRegulationVersion
        ' 
        resources.ApplyResources(TxtCopyingRegulationVersion, "TxtCopyingRegulationVersion")
        TxtCopyingRegulationVersion.DataBindings.Add(New Binding("Text", BsSetup, "ConvertingRegulationVersion", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtCopyingRegulationVersion.Name = "TxtCopyingRegulationVersion"
        ' 
        ' Label24
        ' 
        resources.ApplyResources(Label24, "Label24")
        Label24.Name = "Label24"
        ' 
        ' TxtCopyingRegulationURL
        ' 
        resources.ApplyResources(TxtCopyingRegulationURL, "TxtCopyingRegulationURL")
        TxtCopyingRegulationURL.DataBindings.Add(New Binding("Text", BsSetup, "ConvertingRegulationURL", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtCopyingRegulationURL.Name = "TxtCopyingRegulationURL"
        ' 
        ' TxtCopyingRegulationName
        ' 
        resources.ApplyResources(TxtCopyingRegulationName, "TxtCopyingRegulationName")
        TxtCopyingRegulationName.DataBindings.Add(New Binding("Text", BsSetup, "ConvertingRegulationName", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtCopyingRegulationName.Name = "TxtCopyingRegulationName"
        ' 
        ' Label22
        ' 
        resources.ApplyResources(Label22, "Label22")
        Label22.Name = "Label22"
        ' 
        ' TxtCopyingOrganizationName
        ' 
        resources.ApplyResources(TxtCopyingOrganizationName, "TxtCopyingOrganizationName")
        TxtCopyingOrganizationName.DataBindings.Add(New Binding("Text", BsSetup, "ConvertingOrganization", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtCopyingOrganizationName.Name = "TxtCopyingOrganizationName"
        ' 
        ' Label21
        ' 
        resources.ApplyResources(Label21, "Label21")
        Label21.Name = "Label21"
        ' 
        ' TxtDocumentNameDefault
        ' 
        resources.ApplyResources(TxtDocumentNameDefault, "TxtDocumentNameDefault")
        TxtDocumentNameDefault.DataBindings.Add(New Binding("Text", BsSetup, "DocumentNameDefault", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtDocumentNameDefault.Name = "TxtDocumentNameDefault"
        ' 
        ' Label20
        ' 
        resources.ApplyResources(Label20, "Label20")
        Label20.Name = "Label20"
        ' 
        ' ComboDocumentName
        ' 
        resources.ApplyResources(ComboDocumentName, "ComboDocumentName")
        ComboDocumentName.DataBindings.Add(New Binding("Text", BsSetup, "IndexDocumentName", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboDocumentName.DropDownStyle = ComboBoxStyle.DropDownList
        ComboDocumentName.FormattingEnabled = True
        ComboDocumentName.Name = "ComboDocumentName"
        ' 
        ' Label19
        ' 
        resources.ApplyResources(Label19, "Label19")
        Label19.Name = "Label19"
        ' 
        ' BtnImportSettings
        ' 
        resources.ApplyResources(BtnImportSettings, "BtnImportSettings")
        BtnImportSettings.Name = "BtnImportSettings"
        BtnImportSettings.UseVisualStyleBackColor = True
        ' 
        ' BtnExportSettings
        ' 
        resources.ApplyResources(BtnExportSettings, "BtnExportSettings")
        BtnExportSettings.Name = "BtnExportSettings"
        BtnExportSettings.UseVisualStyleBackColor = True
        ' 
        ' LblCryptoProvider
        ' 
        resources.ApplyResources(LblCryptoProvider, "LblCryptoProvider")
        LblCryptoProvider.Name = "LblCryptoProvider"
        ' 
        ' ComboCryptoProvider
        ' 
        resources.ApplyResources(ComboCryptoProvider, "ComboCryptoProvider")
        ComboCryptoProvider.DataBindings.Add(New Binding("SelectedValue", BsSetup, "DefaultCryptographicProvider", True))
        ComboCryptoProvider.DropDownStyle = ComboBoxStyle.DropDownList
        ComboCryptoProvider.FormattingEnabled = True
        ComboCryptoProvider.Items.AddRange(New Object() {resources.GetString("ComboCryptoProvider.Items"), resources.GetString("ComboCryptoProvider.Items1"), resources.GetString("ComboCryptoProvider.Items2"), resources.GetString("ComboCryptoProvider.Items3")})
        ComboCryptoProvider.Name = "ComboCryptoProvider"
        ' 
        ' BtnRemoveIndexDocumentUID
        ' 
        resources.ApplyResources(BtnRemoveIndexDocumentUID, "BtnRemoveIndexDocumentUID")
        BtnRemoveIndexDocumentUID.Image = My.Resources.Resources.Delete16
        BtnRemoveIndexDocumentUID.Name = "BtnRemoveIndexDocumentUID"
        BtnRemoveIndexDocumentUID.TabStop = False
        ToolTip1.SetToolTip(BtnRemoveIndexDocumentUID, resources.GetString("BtnRemoveIndexDocumentUID.ToolTip"))
        BtnRemoveIndexDocumentUID.UseVisualStyleBackColor = True
        ' 
        ' BtnResetSettings
        ' 
        resources.ApplyResources(BtnResetSettings, "BtnResetSettings")
        BtnResetSettings.Name = "BtnResetSettings"
        BtnResetSettings.UseVisualStyleBackColor = True
        ' 
        ' TabCryptoProviders
        ' 
        resources.ApplyResources(TabCryptoProviders, "TabCryptoProviders")
        TabCryptoProviders.Controls.Add(TabPDFSigner)
        TabCryptoProviders.Controls.Add(TabPDFStreamer)
        TabCryptoProviders.Controls.Add(TabMQ)
        TabCryptoProviders.Controls.Add(TabMNBSigner)
        TabCryptoProviders.Name = "TabCryptoProviders"
        TabCryptoProviders.SelectedIndex = 0
        ' 
        ' TabPDFSigner
        ' 
        TabPDFSigner.Controls.Add(ChkPDFSignerEnabled)
        TabPDFSigner.Controls.Add(PanelPDFSigner)
        resources.ApplyResources(TabPDFSigner, "TabPDFSigner")
        TabPDFSigner.Name = "TabPDFSigner"
        TabPDFSigner.UseVisualStyleBackColor = True
        ' 
        ' ChkPDFSignerEnabled
        ' 
        resources.ApplyResources(ChkPDFSignerEnabled, "ChkPDFSignerEnabled")
        ChkPDFSignerEnabled.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "Enabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkPDFSignerEnabled.Name = "ChkPDFSignerEnabled"
        ChkPDFSignerEnabled.UseVisualStyleBackColor = True
        ' 
        ' PanelPDFSigner
        ' 
        PanelPDFSigner.Controls.Add(ChkAllowQualifiedCertificatesOnly)
        PanelPDFSigner.Controls.Add(ComboSignatureHashMethod)
        PanelPDFSigner.Controls.Add(ChkEnableRevocationChecking)
        PanelPDFSigner.Controls.Add(ChkProxyEnabled)
        PanelPDFSigner.Controls.Add(Label11)
        PanelPDFSigner.Controls.Add(ComboPAdESLevel)
        PanelPDFSigner.Controls.Add(LblPAdESLevel)
        PanelPDFSigner.Controls.Add(TxtSigningReason)
        PanelPDFSigner.Controls.Add(GrpTimeStamp)
        PanelPDFSigner.Controls.Add(GrpRevocation)
        PanelPDFSigner.Controls.Add(GrpProxy)
        PanelPDFSigner.Controls.Add(Label9)
        PanelPDFSigner.Controls.Add(TxtSigningOrganization)
        PanelPDFSigner.Controls.Add(Label8)
        PanelPDFSigner.Controls.Add(ChkSignerNameFromLoggedOnUser)
        resources.ApplyResources(PanelPDFSigner, "PanelPDFSigner")
        PanelPDFSigner.Name = "PanelPDFSigner"
        ' 
        ' ChkAllowQualifiedCertificatesOnly
        ' 
        resources.ApplyResources(ChkAllowQualifiedCertificatesOnly, "ChkAllowQualifiedCertificatesOnly")
        ChkAllowQualifiedCertificatesOnly.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "AllowQualifiedCertificatesOnly", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkAllowQualifiedCertificatesOnly.Name = "ChkAllowQualifiedCertificatesOnly"
        ChkAllowQualifiedCertificatesOnly.UseVisualStyleBackColor = True
        ' 
        ' ChkProxyEnabled
        ' 
        resources.ApplyResources(ChkProxyEnabled, "ChkProxyEnabled")
        ChkProxyEnabled.BackColor = Drawing.SystemColors.ControlLightLight
        ChkProxyEnabled.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "IsProxyEnabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkProxyEnabled.Name = "ChkProxyEnabled"
        ChkProxyEnabled.UseVisualStyleBackColor = False
        ' 
        ' GrpProxy
        ' 
        resources.ApplyResources(GrpProxy, "GrpProxy")
        GrpProxy.Controls.Add(ComboProxyAuthMethod)
        GrpProxy.Controls.Add(Label40)
        GrpProxy.Controls.Add(NumPort)
        GrpProxy.Controls.Add(Label39)
        GrpProxy.Controls.Add(TxtProxyPassword)
        GrpProxy.Controls.Add(Label36)
        GrpProxy.Controls.Add(TxtProxyUser)
        GrpProxy.Controls.Add(Label37)
        GrpProxy.Controls.Add(TxtProxyServer)
        GrpProxy.Controls.Add(Label38)
        GrpProxy.Name = "GrpProxy"
        GrpProxy.TabStop = False
        ' 
        ' ComboProxyAuthMethod
        ' 
        ComboProxyAuthMethod.DataBindings.Add(New Binding("SelectedValue", BsPDFSignerCryptoProvider, "ProxyAuthMethod", True))
        ComboProxyAuthMethod.DropDownStyle = ComboBoxStyle.DropDownList
        resources.ApplyResources(ComboProxyAuthMethod, "ComboProxyAuthMethod")
        ComboProxyAuthMethod.FormattingEnabled = True
        ComboProxyAuthMethod.Name = "ComboProxyAuthMethod"
        ' 
        ' Label40
        ' 
        resources.ApplyResources(Label40, "Label40")
        Label40.Name = "Label40"
        ' 
        ' NumPort
        ' 
        resources.ApplyResources(NumPort, "NumPort")
        NumPort.DataBindings.Add(New Binding("Value", BsPDFSignerCryptoProvider, "ProxyPort", True, DataSourceUpdateMode.OnPropertyChanged))
        NumPort.Maximum = New Decimal(New Integer() {65535, 0, 0, 0})
        NumPort.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        NumPort.Name = "NumPort"
        NumPort.Value = New Decimal(New Integer() {8080, 0, 0, 0})
        ' 
        ' Label39
        ' 
        resources.ApplyResources(Label39, "Label39")
        Label39.Name = "Label39"
        ' 
        ' TxtProxyPassword
        ' 
        resources.ApplyResources(TxtProxyPassword, "TxtProxyPassword")
        TxtProxyPassword.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "ProxyPassword", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtProxyPassword.Name = "TxtProxyPassword"
        ' 
        ' Label36
        ' 
        resources.ApplyResources(Label36, "Label36")
        Label36.Name = "Label36"
        ' 
        ' TxtProxyUser
        ' 
        resources.ApplyResources(TxtProxyUser, "TxtProxyUser")
        TxtProxyUser.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "ProxyUserName", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtProxyUser.Name = "TxtProxyUser"
        ' 
        ' Label37
        ' 
        resources.ApplyResources(Label37, "Label37")
        Label37.Name = "Label37"
        ' 
        ' TxtProxyServer
        ' 
        resources.ApplyResources(TxtProxyServer, "TxtProxyServer")
        TxtProxyServer.DataBindings.Add(New Binding("Text", BsPDFSignerCryptoProvider, "ProxyServer", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtProxyServer.Name = "TxtProxyServer"
        ' 
        ' Label38
        ' 
        resources.ApplyResources(Label38, "Label38")
        Label38.Name = "Label38"
        ' 
        ' ChkSignerNameFromLoggedOnUser
        ' 
        resources.ApplyResources(ChkSignerNameFromLoggedOnUser, "ChkSignerNameFromLoggedOnUser")
        ChkSignerNameFromLoggedOnUser.DataBindings.Add(New Binding("Checked", BsPDFSignerCryptoProvider, "SignerNameFromLoggedOnUser", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkSignerNameFromLoggedOnUser.Name = "ChkSignerNameFromLoggedOnUser"
        ChkSignerNameFromLoggedOnUser.UseVisualStyleBackColor = True
        ' 
        ' TabPDFStreamer
        ' 
        TabPDFStreamer.Controls.Add(ChkPDFStreamerEnabled)
        TabPDFStreamer.Controls.Add(PanelPDFStreamer)
        resources.ApplyResources(TabPDFStreamer, "TabPDFStreamer")
        TabPDFStreamer.Name = "TabPDFStreamer"
        TabPDFStreamer.UseVisualStyleBackColor = True
        ' 
        ' ChkPDFStreamerEnabled
        ' 
        resources.ApplyResources(ChkPDFStreamerEnabled, "ChkPDFStreamerEnabled")
        ChkPDFStreamerEnabled.DataBindings.Add(New Binding("Checked", BsPDFStreamerCryptoProvider, "Enabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkPDFStreamerEnabled.Name = "ChkPDFStreamerEnabled"
        ChkPDFStreamerEnabled.UseVisualStyleBackColor = True
        ' 
        ' BsPDFStreamerCryptoProvider
        ' 
        BsPDFStreamerCryptoProvider.DataSource = GetType(PDFSignerCommon.PDFStreamerCryptoProvider)
        ' 
        ' PanelPDFStreamer
        ' 
        PanelPDFStreamer.Controls.Add(GrpSignaturePDFStreamer)
        resources.ApplyResources(PanelPDFStreamer, "PanelPDFStreamer")
        PanelPDFStreamer.Name = "PanelPDFStreamer"
        ' 
        ' GrpSignaturePDFStreamer
        ' 
        resources.ApplyResources(GrpSignaturePDFStreamer, "GrpSignaturePDFStreamer")
        GrpSignaturePDFStreamer.Controls.Add(TxtPDFStreamerAuthorizationCode)
        GrpSignaturePDFStreamer.Controls.Add(Label29)
        GrpSignaturePDFStreamer.Controls.Add(TxtPDFStreamerConfigurationFileName)
        GrpSignaturePDFStreamer.Controls.Add(Label30)
        GrpSignaturePDFStreamer.Controls.Add(TxtPDFStreamerURL)
        GrpSignaturePDFStreamer.Controls.Add(Label31)
        GrpSignaturePDFStreamer.Name = "GrpSignaturePDFStreamer"
        GrpSignaturePDFStreamer.TabStop = False
        ' 
        ' TxtPDFStreamerAuthorizationCode
        ' 
        resources.ApplyResources(TxtPDFStreamerAuthorizationCode, "TxtPDFStreamerAuthorizationCode")
        TxtPDFStreamerAuthorizationCode.DataBindings.Add(New Binding("Text", BsPDFStreamerCryptoProvider, "PDFStreamerAuthorizationCode", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtPDFStreamerAuthorizationCode.Name = "TxtPDFStreamerAuthorizationCode"
        ' 
        ' Label29
        ' 
        resources.ApplyResources(Label29, "Label29")
        Label29.Name = "Label29"
        ' 
        ' TxtPDFStreamerConfigurationFileName
        ' 
        resources.ApplyResources(TxtPDFStreamerConfigurationFileName, "TxtPDFStreamerConfigurationFileName")
        TxtPDFStreamerConfigurationFileName.DataBindings.Add(New Binding("Text", BsPDFStreamerCryptoProvider, "PDFStreamerConfigFile", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtPDFStreamerConfigurationFileName.Name = "TxtPDFStreamerConfigurationFileName"
        ' 
        ' Label30
        ' 
        resources.ApplyResources(Label30, "Label30")
        Label30.Name = "Label30"
        ' 
        ' TxtPDFStreamerURL
        ' 
        resources.ApplyResources(TxtPDFStreamerURL, "TxtPDFStreamerURL")
        TxtPDFStreamerURL.DataBindings.Add(New Binding("Text", BsPDFStreamerCryptoProvider, "PDFStreamerURL", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtPDFStreamerURL.Name = "TxtPDFStreamerURL"
        ' 
        ' Label31
        ' 
        resources.ApplyResources(Label31, "Label31")
        Label31.Name = "Label31"
        ' 
        ' TabMQ
        ' 
        TabMQ.Controls.Add(ChkMQEnabled)
        TabMQ.Controls.Add(PanelMQFTP)
        resources.ApplyResources(TabMQ, "TabMQ")
        TabMQ.Name = "TabMQ"
        TabMQ.UseVisualStyleBackColor = True
        ' 
        ' ChkMQEnabled
        ' 
        resources.ApplyResources(ChkMQEnabled, "ChkMQEnabled")
        ChkMQEnabled.DataBindings.Add(New Binding("Checked", BsMQFTPCryptoProvider, "Enabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkMQEnabled.Name = "ChkMQEnabled"
        ChkMQEnabled.UseVisualStyleBackColor = True
        ' 
        ' BsMQFTPCryptoProvider
        ' 
        BsMQFTPCryptoProvider.DataSource = GetType(PDFSignerCommon.MQFTPCryptoProvider)
        ' 
        ' PanelMQFTP
        ' 
        PanelMQFTP.Controls.Add(GrpSignatureMQFTP)
        resources.ApplyResources(PanelMQFTP, "PanelMQFTP")
        PanelMQFTP.Name = "PanelMQFTP"
        ' 
        ' GrpSignatureMQFTP
        ' 
        resources.ApplyResources(GrpSignatureMQFTP, "GrpSignatureMQFTP")
        GrpSignatureMQFTP.Controls.Add(BtnRemoveIndexDocumentUID)
        GrpSignatureMQFTP.Controls.Add(ComboMQDocUID)
        GrpSignatureMQFTP.Controls.Add(Label34)
        GrpSignatureMQFTP.Controls.Add(Label33)
        GrpSignatureMQFTP.Controls.Add(TxtMQDomain)
        GrpSignatureMQFTP.Controls.Add(TxtMQUserName)
        GrpSignatureMQFTP.Controls.Add(TxtMQFolderIn)
        GrpSignatureMQFTP.Controls.Add(Label32)
        GrpSignatureMQFTP.Controls.Add(TxtMQPassword)
        GrpSignatureMQFTP.Controls.Add(Label26)
        GrpSignatureMQFTP.Controls.Add(Label27)
        GrpSignatureMQFTP.Controls.Add(TxtMQFolderOut)
        GrpSignatureMQFTP.Controls.Add(Label28)
        GrpSignatureMQFTP.Name = "GrpSignatureMQFTP"
        GrpSignatureMQFTP.TabStop = False
        ' 
        ' ComboMQDocUID
        ' 
        resources.ApplyResources(ComboMQDocUID, "ComboMQDocUID")
        ComboMQDocUID.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "IndexMQDocUID", True, DataSourceUpdateMode.OnPropertyChanged))
        ComboMQDocUID.DropDownStyle = ComboBoxStyle.DropDownList
        ComboMQDocUID.FormattingEnabled = True
        ComboMQDocUID.Name = "ComboMQDocUID"
        ' 
        ' Label34
        ' 
        resources.ApplyResources(Label34, "Label34")
        Label34.Name = "Label34"
        ' 
        ' Label33
        ' 
        resources.ApplyResources(Label33, "Label33")
        Label33.Name = "Label33"
        ' 
        ' TxtMQDomain
        ' 
        TxtMQDomain.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "MQDomain", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(TxtMQDomain, "TxtMQDomain")
        TxtMQDomain.Name = "TxtMQDomain"
        ' 
        ' TxtMQUserName
        ' 
        resources.ApplyResources(TxtMQUserName, "TxtMQUserName")
        TxtMQUserName.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "MQUserName", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMQUserName.Name = "TxtMQUserName"
        ' 
        ' TxtMQFolderIn
        ' 
        resources.ApplyResources(TxtMQFolderIn, "TxtMQFolderIn")
        TxtMQFolderIn.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "MQFolderIn", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMQFolderIn.Name = "TxtMQFolderIn"
        ' 
        ' Label32
        ' 
        resources.ApplyResources(Label32, "Label32")
        Label32.Name = "Label32"
        ' 
        ' TxtMQPassword
        ' 
        resources.ApplyResources(TxtMQPassword, "TxtMQPassword")
        TxtMQPassword.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "MQPassword", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMQPassword.Name = "TxtMQPassword"
        ' 
        ' Label26
        ' 
        resources.ApplyResources(Label26, "Label26")
        Label26.Name = "Label26"
        ' 
        ' Label27
        ' 
        resources.ApplyResources(Label27, "Label27")
        Label27.Name = "Label27"
        ' 
        ' TxtMQFolderOut
        ' 
        resources.ApplyResources(TxtMQFolderOut, "TxtMQFolderOut")
        TxtMQFolderOut.DataBindings.Add(New Binding("Text", BsMQFTPCryptoProvider, "MQFolderOut", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMQFolderOut.Name = "TxtMQFolderOut"
        ' 
        ' Label28
        ' 
        resources.ApplyResources(Label28, "Label28")
        Label28.Name = "Label28"
        ' 
        ' TabMNBSigner
        ' 
        TabMNBSigner.Controls.Add(ChkMNBSignerEnabled)
        TabMNBSigner.Controls.Add(PanelMNBSigner)
        resources.ApplyResources(TabMNBSigner, "TabMNBSigner")
        TabMNBSigner.Name = "TabMNBSigner"
        TabMNBSigner.UseVisualStyleBackColor = True
        ' 
        ' ChkMNBSignerEnabled
        ' 
        resources.ApplyResources(ChkMNBSignerEnabled, "ChkMNBSignerEnabled")
        ChkMNBSignerEnabled.DataBindings.Add(New Binding("Checked", BsMNBSignerCryptoProvider, "Enabled", True, DataSourceUpdateMode.OnPropertyChanged))
        ChkMNBSignerEnabled.Name = "ChkMNBSignerEnabled"
        ChkMNBSignerEnabled.UseVisualStyleBackColor = True
        ' 
        ' BsMNBSignerCryptoProvider
        ' 
        BsMNBSignerCryptoProvider.DataSource = GetType(PDFSignerCommon.MNBSignerCryptoProvider)
        ' 
        ' PanelMNBSigner
        ' 
        PanelMNBSigner.Controls.Add(GrpMNBSignerAuthentication)
        PanelMNBSigner.Controls.Add(GrpMNBSignerWCFSettings)
        resources.ApplyResources(PanelMNBSigner, "PanelMNBSigner")
        PanelMNBSigner.Name = "PanelMNBSigner"
        ' 
        ' GrpMNBSignerAuthentication
        ' 
        resources.ApplyResources(GrpMNBSignerAuthentication, "GrpMNBSignerAuthentication")
        GrpMNBSignerAuthentication.Controls.Add(RadioMNBSignerPasswordAuthentication)
        GrpMNBSignerAuthentication.Controls.Add(RadioMNBSignerWindowsAuthentication)
        GrpMNBSignerAuthentication.Controls.Add(PanelMNBSignerAuthentication)
        GrpMNBSignerAuthentication.Name = "GrpMNBSignerAuthentication"
        GrpMNBSignerAuthentication.TabStop = False
        ' 
        ' RadioMNBSignerPasswordAuthentication
        ' 
        resources.ApplyResources(RadioMNBSignerPasswordAuthentication, "RadioMNBSignerPasswordAuthentication")
        RadioMNBSignerPasswordAuthentication.DataBindings.Add(New Binding("Checked", BsMNBSignerCryptoProvider, "IsMNBSignerUserPasswordAuthentication", True, DataSourceUpdateMode.OnPropertyChanged))
        RadioMNBSignerPasswordAuthentication.Name = "RadioMNBSignerPasswordAuthentication"
        RadioMNBSignerPasswordAuthentication.TabStop = True
        RadioMNBSignerPasswordAuthentication.UseVisualStyleBackColor = True
        ' 
        ' RadioMNBSignerWindowsAuthentication
        ' 
        resources.ApplyResources(RadioMNBSignerWindowsAuthentication, "RadioMNBSignerWindowsAuthentication")
        RadioMNBSignerWindowsAuthentication.DataBindings.Add(New Binding("Checked", BsMNBSignerCryptoProvider, "IsMNBSignerWindowsAuthentication", True, DataSourceUpdateMode.OnPropertyChanged))
        RadioMNBSignerWindowsAuthentication.Name = "RadioMNBSignerWindowsAuthentication"
        RadioMNBSignerWindowsAuthentication.TabStop = True
        RadioMNBSignerWindowsAuthentication.UseVisualStyleBackColor = True
        ' 
        ' PanelMNBSignerAuthentication
        ' 
        PanelMNBSignerAuthentication.Controls.Add(Label50)
        PanelMNBSignerAuthentication.Controls.Add(TxtMNBSignerDomain)
        PanelMNBSignerAuthentication.Controls.Add(TxtMNBSignerUserName)
        PanelMNBSignerAuthentication.Controls.Add(TxtMNBSignerPassword)
        PanelMNBSignerAuthentication.Controls.Add(Label51)
        PanelMNBSignerAuthentication.Controls.Add(Label52)
        resources.ApplyResources(PanelMNBSignerAuthentication, "PanelMNBSignerAuthentication")
        PanelMNBSignerAuthentication.Name = "PanelMNBSignerAuthentication"
        ' 
        ' Label50
        ' 
        resources.ApplyResources(Label50, "Label50")
        Label50.Name = "Label50"
        ' 
        ' TxtMNBSignerDomain
        ' 
        TxtMNBSignerDomain.DataBindings.Add(New Binding("Text", BsMNBSignerCryptoProvider, "MNBSignerDomain", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(TxtMNBSignerDomain, "TxtMNBSignerDomain")
        TxtMNBSignerDomain.Name = "TxtMNBSignerDomain"
        ' 
        ' TxtMNBSignerUserName
        ' 
        resources.ApplyResources(TxtMNBSignerUserName, "TxtMNBSignerUserName")
        TxtMNBSignerUserName.DataBindings.Add(New Binding("Text", BsMNBSignerCryptoProvider, "MNBSignerUserName", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMNBSignerUserName.Name = "TxtMNBSignerUserName"
        ' 
        ' TxtMNBSignerPassword
        ' 
        resources.ApplyResources(TxtMNBSignerPassword, "TxtMNBSignerPassword")
        TxtMNBSignerPassword.DataBindings.Add(New Binding("Text", BsMNBSignerCryptoProvider, "MNBSignerPassword", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMNBSignerPassword.Name = "TxtMNBSignerPassword"
        ' 
        ' Label51
        ' 
        resources.ApplyResources(Label51, "Label51")
        Label51.Name = "Label51"
        ' 
        ' Label52
        ' 
        resources.ApplyResources(Label52, "Label52")
        Label52.Name = "Label52"
        ' 
        ' GrpMNBSignerWCFSettings
        ' 
        resources.ApplyResources(GrpMNBSignerWCFSettings, "GrpMNBSignerWCFSettings")
        GrpMNBSignerWCFSettings.Controls.Add(Label48)
        GrpMNBSignerWCFSettings.Controls.Add(Label49)
        GrpMNBSignerWCFSettings.Controls.Add(NumMNBSignerChunkSize)
        GrpMNBSignerWCFSettings.Controls.Add(Label46)
        GrpMNBSignerWCFSettings.Controls.Add(Label47)
        GrpMNBSignerWCFSettings.Controls.Add(NumMNBSignerSignTimeout)
        GrpMNBSignerWCFSettings.Controls.Add(Label45)
        GrpMNBSignerWCFSettings.Controls.Add(Label44)
        GrpMNBSignerWCFSettings.Controls.Add(NumMNBSignerWCFTimeout)
        GrpMNBSignerWCFSettings.Controls.Add(TxtMNBSignerURL)
        GrpMNBSignerWCFSettings.Controls.Add(Label43)
        GrpMNBSignerWCFSettings.Name = "GrpMNBSignerWCFSettings"
        GrpMNBSignerWCFSettings.TabStop = False
        ' 
        ' Label48
        ' 
        resources.ApplyResources(Label48, "Label48")
        Label48.Name = "Label48"
        ' 
        ' Label49
        ' 
        resources.ApplyResources(Label49, "Label49")
        Label49.Name = "Label49"
        ' 
        ' NumMNBSignerChunkSize
        ' 
        NumMNBSignerChunkSize.DataBindings.Add(New Binding("Value", BsMNBSignerCryptoProvider, "MNBSignerChunkSize", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(NumMNBSignerChunkSize, "NumMNBSignerChunkSize")
        NumMNBSignerChunkSize.Maximum = New Decimal(New Integer() {Integer.MaxValue, 0, 0, 0})
        NumMNBSignerChunkSize.Name = "NumMNBSignerChunkSize"
        NumMNBSignerChunkSize.Value = New Decimal(New Integer() {25000000, 0, 0, 0})
        ' 
        ' Label46
        ' 
        resources.ApplyResources(Label46, "Label46")
        Label46.Name = "Label46"
        ' 
        ' Label47
        ' 
        resources.ApplyResources(Label47, "Label47")
        Label47.Name = "Label47"
        ' 
        ' NumMNBSignerSignTimeout
        ' 
        NumMNBSignerSignTimeout.DataBindings.Add(New Binding("Value", BsMNBSignerCryptoProvider, "MNBSignerSigningTimeout", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(NumMNBSignerSignTimeout, "NumMNBSignerSignTimeout")
        NumMNBSignerSignTimeout.Maximum = New Decimal(New Integer() {1200, 0, 0, 0})
        NumMNBSignerSignTimeout.Name = "NumMNBSignerSignTimeout"
        NumMNBSignerSignTimeout.Value = New Decimal(New Integer() {60, 0, 0, 0})
        ' 
        ' Label45
        ' 
        resources.ApplyResources(Label45, "Label45")
        Label45.Name = "Label45"
        ' 
        ' Label44
        ' 
        resources.ApplyResources(Label44, "Label44")
        Label44.Name = "Label44"
        ' 
        ' NumMNBSignerWCFTimeout
        ' 
        NumMNBSignerWCFTimeout.DataBindings.Add(New Binding("Value", BsMNBSignerCryptoProvider, "MNBSignerWSTimeout", True, DataSourceUpdateMode.OnPropertyChanged))
        resources.ApplyResources(NumMNBSignerWCFTimeout, "NumMNBSignerWCFTimeout")
        NumMNBSignerWCFTimeout.Maximum = New Decimal(New Integer() {1200, 0, 0, 0})
        NumMNBSignerWCFTimeout.Name = "NumMNBSignerWCFTimeout"
        NumMNBSignerWCFTimeout.Value = New Decimal(New Integer() {60, 0, 0, 0})
        ' 
        ' TxtMNBSignerURL
        ' 
        resources.ApplyResources(TxtMNBSignerURL, "TxtMNBSignerURL")
        TxtMNBSignerURL.DataBindings.Add(New Binding("Text", BsMNBSignerCryptoProvider, "MNBSignerURL", True, DataSourceUpdateMode.OnPropertyChanged))
        TxtMNBSignerURL.Name = "TxtMNBSignerURL"
        ' 
        ' Label43
        ' 
        resources.ApplyResources(Label43, "Label43")
        Label43.Name = "Label43"
        ' 
        ' GrpFileNaming
        ' 
        resources.ApplyResources(GrpFileNaming, "GrpFileNaming")
        GrpFileNaming.Controls.Add(TxtFileExtension)
        GrpFileNaming.Controls.Add(LblFileExtension)
        GrpFileNaming.Controls.Add(TxtFileAppend)
        GrpFileNaming.Controls.Add(LblFileAppend)
        GrpFileNaming.Controls.Add(RadioCreateNewFile)
        GrpFileNaming.Controls.Add(RadioOverwriteOriginal)
        GrpFileNaming.Name = "GrpFileNaming"
        GrpFileNaming.TabStop = False
        ' 
        ' TxtFileExtension
        ' 
        resources.ApplyResources(TxtFileExtension, "TxtFileExtension")
        TxtFileExtension.DataBindings.Add(New Binding("Text", BsSetup, "FileExtensionReplace", True))
        TxtFileExtension.Name = "TxtFileExtension"
        ' 
        ' LblFileExtension
        ' 
        resources.ApplyResources(LblFileExtension, "LblFileExtension")
        LblFileExtension.Name = "LblFileExtension"
        ' 
        ' TxtFileAppend
        ' 
        resources.ApplyResources(TxtFileAppend, "TxtFileAppend")
        TxtFileAppend.DataBindings.Add(New Binding("Text", BsSetup, "FileNameAppend", True))
        TxtFileAppend.Name = "TxtFileAppend"
        ' 
        ' LblFileAppend
        ' 
        resources.ApplyResources(LblFileAppend, "LblFileAppend")
        LblFileAppend.Name = "LblFileAppend"
        ' 
        ' RadioCreateNewFile
        ' 
        resources.ApplyResources(RadioCreateNewFile, "RadioCreateNewFile")
        RadioCreateNewFile.DataBindings.Add(New Binding("Checked", BsSetup, "FileCreateNew", True, DataSourceUpdateMode.OnPropertyChanged))
        RadioCreateNewFile.Name = "RadioCreateNewFile"
        RadioCreateNewFile.UseVisualStyleBackColor = True
        ' 
        ' RadioOverwriteOriginal
        ' 
        resources.ApplyResources(RadioOverwriteOriginal, "RadioOverwriteOriginal")
        RadioOverwriteOriginal.Checked = True
        RadioOverwriteOriginal.DataBindings.Add(New Binding("Checked", BsSetup, "FileOverwriteOriginal", True, DataSourceUpdateMode.OnPropertyChanged))
        RadioOverwriteOriginal.Name = "RadioOverwriteOriginal"
        RadioOverwriteOriginal.TabStop = True
        RadioOverwriteOriginal.UseVisualStyleBackColor = True
        ' 
        ' Label41
        ' 
        resources.ApplyResources(Label41, "Label41")
        Label41.Name = "Label41"
        ' 
        ' ComboDocumentViewer
        ' 
        resources.ApplyResources(ComboDocumentViewer, "ComboDocumentViewer")
        ComboDocumentViewer.DataBindings.Add(New Binding("SelectedValue", BsSetup, "DocumentViewer", True))
        ComboDocumentViewer.DropDownStyle = ComboBoxStyle.DropDownList
        ComboDocumentViewer.FormattingEnabled = True
        ComboDocumentViewer.Items.AddRange(New Object() {resources.GetString("ComboDocumentViewer.Items"), resources.GetString("ComboDocumentViewer.Items1")})
        ComboDocumentViewer.Name = "ComboDocumentViewer"
        ' 
        ' PanelMain
        ' 
        resources.ApplyResources(PanelMain, "PanelMain")
        PanelMain.Controls.Add(Label41)
        PanelMain.Controls.Add(GrpIndexes)
        PanelMain.Controls.Add(ComboDocumentViewer)
        PanelMain.Controls.Add(GrpBarCode)
        PanelMain.Controls.Add(GrpFileNaming)
        PanelMain.Controls.Add(GrpClause)
        PanelMain.Controls.Add(TabCryptoProviders)
        PanelMain.Controls.Add(LblCryptoProvider)
        PanelMain.Controls.Add(ComboCryptoProvider)
        PanelMain.DataBindings.Add(New Binding("Enabled", BsSetup, "IsSigningEnabled", True, DataSourceUpdateMode.OnPropertyChanged))
        PanelMain.Name = "PanelMain"
        ' 
        ' OpenFileDialog
        ' 
        OpenFileDialog.DefaultExt = "xml"
        OpenFileDialog.FileName = "PDFSignerConfig"
        resources.ApplyResources(OpenFileDialog, "OpenFileDialog")
        ' 
        ' SaveFileDialog
        ' 
        SaveFileDialog.DefaultExt = "xml"
        SaveFileDialog.FileName = "PDFSignerConfig"
        resources.ApplyResources(SaveFileDialog, "SaveFileDialog")
        ' 
        ' FrmSetup
        ' 
        AcceptButton = BtnOK
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = BtnCancel
        Controls.Add(BtnResetSettings)
        Controls.Add(BtnExportSettings)
        Controls.Add(BtnImportSettings)
        Controls.Add(ChkSigningEnabled)
        Controls.Add(BtnCancel)
        Controls.Add(BtnOK)
        Controls.Add(PanelMain)
        MaximizeBox = False
        Name = "FrmSetup"
        ShowInTaskbar = False
        CType(BsSetup, ComponentModel.ISupportInitialize).EndInit()
        GrpIndexes.ResumeLayout(False)
        GrpIndexes.PerformLayout()
        GrpBarCode.ResumeLayout(False)
        GrpBarCode.PerformLayout()
        CType(NumericSignatureMarkerPosition, ComponentModel.ISupportInitialize).EndInit()
        CType(BsPDFSignerCryptoProvider, ComponentModel.ISupportInitialize).EndInit()
        GrpTimeStamp.ResumeLayout(False)
        GrpTimeStamp.PerformLayout()
        GrpClause.ResumeLayout(False)
        GrpClause.PerformLayout()
        TabCryptoProviders.ResumeLayout(False)
        TabPDFSigner.ResumeLayout(False)
        TabPDFSigner.PerformLayout()
        PanelPDFSigner.ResumeLayout(False)
        PanelPDFSigner.PerformLayout()
        GrpProxy.ResumeLayout(False)
        GrpProxy.PerformLayout()
        GrpRevocation.ResumeLayout(False)
        GrpRevocation.PerformLayout()
        CType(NumPort, ComponentModel.ISupportInitialize).EndInit()
        TabPDFStreamer.ResumeLayout(False)
        TabPDFStreamer.PerformLayout()
        CType(BsPDFStreamerCryptoProvider, ComponentModel.ISupportInitialize).EndInit()
        PanelPDFStreamer.ResumeLayout(False)
        GrpSignaturePDFStreamer.ResumeLayout(False)
        GrpSignaturePDFStreamer.PerformLayout()
        TabMQ.ResumeLayout(False)
        TabMQ.PerformLayout()
        CType(BsMQFTPCryptoProvider, ComponentModel.ISupportInitialize).EndInit()
        PanelMQFTP.ResumeLayout(False)
        GrpSignatureMQFTP.ResumeLayout(False)
        GrpSignatureMQFTP.PerformLayout()
        TabMNBSigner.ResumeLayout(False)
        TabMNBSigner.PerformLayout()
        CType(BsMNBSignerCryptoProvider, ComponentModel.ISupportInitialize).EndInit()
        PanelMNBSigner.ResumeLayout(False)
        GrpMNBSignerAuthentication.ResumeLayout(False)
        GrpMNBSignerAuthentication.PerformLayout()
        PanelMNBSignerAuthentication.ResumeLayout(False)
        PanelMNBSignerAuthentication.PerformLayout()
        GrpMNBSignerWCFSettings.ResumeLayout(False)
        GrpMNBSignerWCFSettings.PerformLayout()
        CType(NumMNBSignerChunkSize, ComponentModel.ISupportInitialize).EndInit()
        CType(NumMNBSignerSignTimeout, ComponentModel.ISupportInitialize).EndInit()
        CType(NumMNBSignerWCFTimeout, ComponentModel.ISupportInitialize).EndInit()
        GrpFileNaming.ResumeLayout(False)
        GrpFileNaming.PerformLayout()
        PanelMain.ResumeLayout(False)
        PanelMain.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

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
    Private WithEvents ComboRevocationCheckProtocol As System.Windows.Forms.ComboBox
    Private WithEvents LblPAdESLevel As System.Windows.Forms.Label
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
    Private WithEvents TxtTSAPassword As System.Windows.Forms.TextBox
    Private WithEvents Label16 As System.Windows.Forms.Label
    Private WithEvents TxtTSAUser As System.Windows.Forms.TextBox
    Private WithEvents Label17 As System.Windows.Forms.Label
    Private WithEvents TxtTSAURL As System.Windows.Forms.TextBox
    Private WithEvents Label15 As System.Windows.Forms.Label
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
    Private WithEvents ChkSignerNameFromLoggedOnUser As CheckBox
    Private WithEvents ComboPAdESLevel As ComboBox
    Private WithEvents GrpRevocation As GroupBox
    Private WithEvents ChkEnableRevocationChecking As CheckBox
    Private WithEvents LblRevocationProtocol As Label
    Private WithEvents ChkEmbedRevocationInformation As CheckBox
    Private WithEvents BsPDFSignerCryptoProvider As BindingSource
    Private WithEvents BsPDFStreamerCryptoProvider As BindingSource
    Private WithEvents BsMQFTPCryptoProvider As BindingSource
    Private WithEvents BsMNBSignerCryptoProvider As BindingSource
End Class
