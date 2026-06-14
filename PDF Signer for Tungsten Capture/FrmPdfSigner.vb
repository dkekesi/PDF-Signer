#Region "Imports"
Imports DevExpress.XtraBars
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraEditors.ViewInfo
Imports DevExpress.XtraTreeList
Imports DevExpress.XtraTreeList.Nodes
Imports DevExpress.Utils
Imports NLog
Imports PDFSigner.My.Resources
Imports PDFSignerCommon
Imports PDFSignerCommon.My.Resources
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports Kofax.Capture.SDK.CustomModule
#End Region

Friend Class FrmPdfSigner
    Private ReadOnly _BatMan As New BatchManager
    Private _Batch As IBatch
    Private _FullUserName As String
    Private _FormCSHIDString As String
    Private ReadOnly _BinPath As String
    Private _LicenseInfo As New LicenseInfo
    Private _ActiveTasks As List(Of Task)
    Private _CtrlIsDown As Boolean
    Private _SaveLayout As Boolean = True
    Private _RefreshingProviders As Boolean
    Private _DocumentActionRunning As Boolean
    Private ReadOnly _SingleBatchOpenID As Integer

    Private WithEvents ViewerBaseOriginalControl As New ViewerBase
    Private WithEvents ViewerBaseSignedControl As New ViewerBase
    Private WithEvents DevExpressPDFViewerOriginalControl As New DevExpressPDFViewer
    Private WithEvents DevExpressPDFViewerSignedControl As New DevExpressPDFViewer
    Private WithEvents TallPDFViewerOriginalControl As New TallPDFViewer
    Private WithEvents TallPDFViewerSignedControl As New TallPDFViewer

    Dim WithEvents CurrentDomain As AppDomain = AppDomain.CurrentDomain

    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            HelpProvider.ShowHelp(_FormCSHIDString)
            ' Indicate that you handled this keystroke
            Return True
        End If

        ' Call the base class
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub PdfViewer_CurrentPageChanged(sender As Object, e As EventArgs) Handles PdfViewerOriginal.CurrentPageChangedEvent, PdfViewerSigned.CurrentPageChangedEvent

        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)
        If currentItem.GetType Is GetType(DocumentItem) Then
            If PdfViewerOriginal.CurrentPageNumber > 0 Then
                If BatchTree.FocusedNode.Nodes.Count - 1 >= PdfViewerOriginal.CurrentPageNumber Then
                    ' Get the actual node
                    If TypeOf PdfViewerOriginal Is DevExpressPDFViewer Then
                        BatchTree.FocusedNode = BatchTree.FocusedNode.Nodes(PdfViewerOriginal.CurrentPageNumber - 1)
                    Else
                        BatchTree.FocusedNode = BatchTree.FocusedNode.Nodes(PdfViewerOriginal.CurrentPageNumber)
                    End If
                Else
                    ' Get the last node, if for some reason there are fewer pages than in the PDF
                    BatchTree.FocusedNode = BatchTree.FocusedNode.Nodes(BatchTree.FocusedNode.Nodes.Count - 1)
                End If
            End If
        End If

        If currentItem.GetType Is GetType(PageItem) Then
            If BatchTree.FocusedNode.ParentNode.Nodes.Count - 1 >= PdfViewerOriginal.CurrentPageNumber Then
                ' Get the actual node
                If TypeOf PdfViewerOriginal Is DevExpressPDFViewer Then
                    BatchTree.FocusedNode = BatchTree.FocusedNode.ParentNode.Nodes(PdfViewerOriginal.CurrentPageNumber - 1)
                Else
                    BatchTree.FocusedNode = BatchTree.FocusedNode.ParentNode.Nodes(PdfViewerOriginal.CurrentPageNumber)
                End If
            Else
                ' Get the last node, if for some reason there are fewer pages than in the PDF
                BatchTree.FocusedNode = BatchTree.FocusedNode.ParentNode.Nodes(BatchTree.FocusedNode.ParentNode.Nodes.Count - 1)
            End If
        End If
    End Sub

    Private Sub GUI_MouseEnter(sender As Object, e As EventArgs) Handles BatchTree.MouseEnter
        sender.Focus()
    End Sub

    Private Sub BatchTreeProgressBarInProgress_CustomDisplayText(sender As Object, e As CustomDisplayTextEventArgs) Handles BatchTreeProgressBar.CustomDisplayText
        If e.Value = 0 Then
            e.DisplayText = String.Empty
        Else
            e.DisplayText += "%"
        End If
    End Sub

    Public Sub New(args As String())
        Thread.CurrentThread.CurrentCulture = New CultureInfo("hu")
        Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture
        CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture
        CultureInfo.DefaultThreadCurrentUICulture = Thread.CurrentThread.CurrentCulture

        ' This call is required by the designer.
        InitializeComponent()

        If args IsNot Nothing AndAlso args.Count > 0 AndAlso args(0).StartsWith("-B") Then
            Dim batchId As Integer
            If Integer.TryParse(args(0).Substring(2), batchId) Then ' we cut off "-B" from the beginning
                _SingleBatchOpenID = batchId
            Else
                _logger.Warn("Érvénytelen -B kötegazonosító a parancssorban: {0}", args(0))
            End If
        End If
    End Sub

    Private Sub FrmPdfSigner_Load(sender As Object, e As EventArgs) Handles Me.Load
        Hide()

        Dim lm As New LayoutManager
        lm.LoadWorkspace(WsManager, Name)

        Show()

        Text = MessageBoxTitle
        _FormCSHIDString = "OPERATORTASKS"
        _FullUserName = DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName
        BarLabelUser.Caption = String.Format(Messages.Logged_In_User, _FullUserName, Environment.UserDomainName, Environment.UserName)

        _logger.Info(Messages.Module_Version_Started, MessageBoxTitle, ProductVersion)

        If String.IsNullOrEmpty(KofaxRegistry.KofaxBinPath) Then
            MsgBox(Messages.Registry_Kofax_Bin_Path_Not_Found, MsgBoxStyle.Critical, MessageBoxTitle)
            Close()
            Return
        End If

        Dim licMan As New PDFSignerCommon.LicenseManager
        _LicenseInfo = licMan.LoadFromFile

        If Not String.IsNullOrEmpty(_LicenseInfo.LicenseError) Then
            MsgBox(_LicenseInfo.LicenseError, MsgBoxStyle.Critical, MessageBoxTitle)
            Close()
            Return
        End If

        _BatMan.UniqueID = CustomModuleID
        _BatMan.LoginToKofax()

        Dim sigOp As New SignatureOperation
        sigOp.ActivateSBBLicense()

        RefreshCertificates(False)
        ActivateBatch()
    End Sub

    Private Sub FrmPdfSiger_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _Batch IsNot Nothing Then
            If SigningTasksRunning(True) Then
                e.Cancel = True
                Return
            End If

            Dim frm As New FrmCloseSuspend
            frm.ShowDialog()

            If frm.Result = CloseSuspendResult.Close Then
                e.Cancel = CloseBatch(False, False, False)
            End If

            If frm.Result = CloseSuspendResult.Suspend Then
                e.Cancel = SuspendBatch(False, False)
            End If

            If frm.Result = CloseSuspendResult.Cancel Then
                e.Cancel = True
            End If
        End If

        If e.Cancel Then Return

        _BatMan?.LogoutFromKofax()

        If _SaveLayout Then
            Dim lm As New LayoutManager
            lm.SaveWorkspace(WsManager, Name)
        End If

        _logger.Info(Messages.Module_Version_Stopped, MessageBoxTitle, ProductVersion.ToString)
    End Sub

    Private Sub ActivateBatch()
        '_SingleBatchOpenID = 14 ' set this to a valid batch ID to debug launch from Batch Manager

        _BatMan.BatchSelect(Me, _SingleBatchOpenID)
        If _BatMan.ActiveBatch Is Nothing Then Return

        _Batch = _BatMan.ActiveBatch
        _FormCSHIDString = "MAINWINDOW"

        Text = String.Format(Messages.Main_Window_Title, MessageBoxTitle, _Batch.Name)
        TabViewer.SelectedTabPage = TabOriginal

        ' initialize a new task set
        _ActiveTasks = New List(Of Task)

        ' load batch contents
        Dim p As New BatchParser
        Dim batchCont As BatchContents = Nothing

        Try
            batchCont = p.GetBatchContents(_Batch) ' this is where we actually load the data
        Catch ex As Exception
            Dim errMsg As String = String.Format(Messages.Error_Opening_Batch, _Batch.Name, ex.Message)
            _logger.Error(errMsg)
            Dim keepApplicationOpen As Boolean = CloseBatch(False, False, False)
            MsgBox(errMsg, MsgBoxStyle.Critical, MessageBoxTitle)

            ' if program was opened in single batch mode, then we close the application
            If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()

            Return
        End Try

        With BatchTree
            .DataSource = batchCont
            .Nodes(0).Expanded = True
        End With

        ' batch is empty, we close it without question
        If Not batchCont.Content.OfType(Of DocumentItem).Any Then
            MsgBox(Messages.No_Unreleased_Documents, MsgBoxStyle.Information, MessageBoxTitle)
            Dim keepApplicationOpen As Boolean = CloseBatch(True, False, False)

            ' if program was opened in single batch mode, then we close the application
            If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()

            Return
        End If

        ' go to first signable document
        Dim hasSignableDocument As Boolean = SelectNextSignableDocument(True)

        _logger.Info(Messages.Batch_Opened, _Batch.Name)
    End Sub

    Private Sub DeactivateBatch(BatchState As KfxDbState, OpenNextBatch As Boolean)
        ' we never open the next batch automatically if program was opened in single batch mode, no matter what the caller wanted
        If _SingleBatchOpenID > 0 Then OpenNextBatch = False

        If _Batch IsNot Nothing Then     ' van nyitva köteg
            Dim batchCont As BindingList(Of TreeItem) = CType(BatchTree.DataSource, BatchContents).Content

            If batchCont.Any(Function(e) e.IsRejected) Then
                ' ha Ready-vel Error-ral vagy Completed-del akartuk lezárni, akkor Error lesz
                If BatchState = KfxDbState.KfxDbBatchReady Or BatchState = KfxDbState.KfxDbBatchCompleted Or BatchState = KfxDbState.KfxDbBatchError Then
                    _BatMan.BatchClose(KfxDbState.KfxDbBatchError)
                Else
                    ' egyébként pedig Suspend-elni akartunk és az marad
                    _BatMan.BatchClose(BatchState)
                End If
            Else
                ' nincs rejected doksi, marad a megadott státusz
                _BatMan.BatchClose(BatchState)
            End If

            _ActiveTasks = Nothing
            _Batch = Nothing

            BatchTree.DataSource = Nothing
            Text = MessageBoxTitle

            _FormCSHIDString = "OPERATORTASKS"

            ' restart the process if needed
            If OpenNextBatch Then ActivateBatch()
        End If
    End Sub

    Friend Function CloseBatch(AutoOpenNextBatch As Boolean, AskConfirmation As Boolean, IsAutoClose As Boolean) As Boolean
        ' returns false when application needs to close afterwards
        ' returns true when application must remain open

        ' ráállunk a batch node-ra, hogy minden deaktivált legyen
        If BatchTree.Nodes IsNot Nothing AndAlso BatchTree.Nodes.Count > 0 Then
            BatchTree.SetFocusedNode(BatchTree.Nodes(0))
        End If

        If AllDocumentsProcessed() Then
            Dim msg As String = Messages.Ask_Close_Batch
            If IsAutoClose Then msg = Messages.Ask_All_Documents_Processed

            If AskConfirmation Then
                If MsgBox(msg, vbYesNo + vbQuestion, MessageBoxTitle) = vbYes Then
                    DeactivateBatch(KfxDbState.KfxDbBatchReady, AutoOpenNextBatch)
                    Return False
                Else
                    Return True
                End If
            Else
                DeactivateBatch(KfxDbState.KfxDbBatchReady, AutoOpenNextBatch)
                Return False
            End If
        Else
            If AskConfirmation Then
                If MsgBox(Messages.Ask_Suspend_Batch_With_Signable_Docs, vbYesNo + vbExclamation, MessageBoxTitle) = vbYes Then
                    DeactivateBatch(KfxDbState.KfxDbBatchSuspended, AutoOpenNextBatch)
                    Return False
                Else
                    Return True
                End If
            Else
                DeactivateBatch(KfxDbState.KfxDbBatchSuspended, AutoOpenNextBatch)
                Return False
            End If
        End If
    End Function

    Friend Function SuspendBatch(AutoOpenNextBatch As Boolean, AskConfirmation As Boolean) As Boolean
        ' returns false when application needs to close afterwards
        ' returns true when application must remain open

        ' ráállunk a batch node-ra, hogy minden deaktivált legyen
        BatchTree.SetFocusedNode(BatchTree.Nodes(0))

        If AskConfirmation Then
            If MsgBox(Messages.Ask_Suspend_Batch, vbYesNo + vbQuestion, MessageBoxTitle) = vbYes Then
                DeactivateBatch(KfxDbState.KfxDbBatchSuspended, AutoOpenNextBatch)
                Return False
            Else
                Return True
            End If
        Else
            DeactivateBatch(KfxDbState.KfxDbBatchSuspended, AutoOpenNextBatch)
            Return False
        End If
    End Function

    Private Function SigningTasksRunning(ShowMessage As Boolean) As Boolean
        If _ActiveTasks IsNot Nothing AndAlso _ActiveTasks.Any(Function(f) Not f.IsCompleted) Then
            If ShowMessage Then MsgBox(Messages.Signature_Tasks_Running, MsgBoxStyle.Exclamation, Resources.MessageBoxTitle)
            Return True
        End If

        Return False
    End Function

    ''' <summary>
    ''' Common epilogue of sign/skip/reject: jump to the next signable document, or —
    ''' when none is left — wait for the running signing tasks, re-check (a task may
    ''' have failed and made its document signable again), then offer to close the batch.
    ''' </summary>
    Private Async Function FinishDocumentActionAsync(ClosingLogMessage As String) As Task
        If SelectNextSignableDocument(False) Then Return

        If _ActiveTasks IsNot Nothing Then Await Task.WhenAll(_ActiveTasks)

        ' yield once so data-binding callbacks posted by the worker threads are processed
        Await Task.Yield()

        ' we have to check it again, as a previously in-progress task might have come back with an error
        If SelectNextSignableDocument(False) Then Return

        If Not AllDocumentsProcessed() Then Return
        If _Batch Is Nothing Then Return

        _logger.Debug(ClosingLogMessage, _Batch.Name)

        Dim keepApplicationOpen As Boolean = CloseBatch(True, True, True)

        ' if program was opened in single batch mode, then we close the application
        If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()
    End Function

#Region "Ribbon items"

#Region "Home ribbon"
    Private Sub BtnBatchOpen_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnBatchOpen.ItemClick
        If _Batch Is Nothing Then ActivateBatch()
    End Sub

    Private Sub BtnBatchSuspend_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnBatchSuspend.ItemClick
        If _Batch IsNot Nothing Then
            If SigningTasksRunning(True) Then Return

            Dim keepApplicationOpen As Boolean = SuspendBatch(True, True)

            ' if program was opened in single batch mode, then we close the application
            If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()
        End If
    End Sub

    Private Sub BtnBatchClose_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnBatchClose.ItemClick
        If _Batch IsNot Nothing Then
            If SigningTasksRunning(True) Then Return

            Dim keepApplicationOpen As Boolean = CloseBatch(True, True, False)

            ' if program was opened in single batch mode, then we close the application
            If _SingleBatchOpenID > 0 AndAlso Not keepApplicationOpen Then Close()
        End If
    End Sub

    Private Async Sub BtnSign_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnSign.ItemClick
        If _DocumentActionRunning Then Return
        _DocumentActionRunning = True
        Try
            If _LicenseInfo.ValidUntil < Date.Now.Date Then
                MsgBox(String.Format(CommonRes.LicenseExpired, _LicenseInfo.ValidUntil.ToShortDateString), MsgBoxStyle.Critical, MessageBoxTitle)
                Return
            End If

            Dim docItem As DocumentItem = GetDocumentItemFromBatchContents()
            If docItem Is Nothing Then Return

            If TabViewer.SelectedTabPage IsNot TabOriginal Then
                MsgBox(Messages.Original_Document_Must_Be_Viewed, MsgBoxStyle.Exclamation, MessageBoxTitle)
                TabViewer.SelectedTabPage = TabOriginal
                Return
            End If

            If docItem.IsRejected Or docItem.Pages.Any(Function(f) f.IsRejected) Then
                MsgBox(Messages.Rejected_Document_Cannot_Be_Signed, MsgBoxStyle.Exclamation, MessageBoxTitle)
                Return
            End If

            If String.IsNullOrWhiteSpace(docItem.FormTypeName) Then
                MsgBox(Messages.Document_Not_Identified, MsgBoxStyle.Exclamation, MessageBoxTitle)
                Return
            End If

            If Not docItem.IsSigningPossible Then
                MsgBox(Messages.Document_Not_Setup_For_Signing, MsgBoxStyle.Exclamation, MessageBoxTitle)
                Return
            End If

            Dim provider As CryptoProviderBase = CType(BarComboProvider.EditValue, CryptoProviderBase)
            If provider.ProviderType = CryptoProviderType.PDFSigner Then
                If BarComboCert.EditValue Is Nothing Then
                    MsgBox(Messages.No_Signing_Certificate_Selected, MsgBoxStyle.Exclamation, MessageBoxTitle)
                    Return
                End If
            End If

            Dim cert As SigningCertificate = CType(BarComboCert.EditValue, SigningCertificate)
            Dim docWidth As Integer = PdfViewerOriginal.PageWidth
            Dim docHeight As Integer = PdfViewerOriginal.PageHeight
            Dim docDimensions As String = String.Format("{0} * {1} mm", docWidth.ToString(), docHeight.ToString())

            If PdfViewerSigned.HasDocument Then
                PdfViewerSigned.CloseDocument()
            End If

            Dim pdfFileStream As Stream

            If docItem.FileSize <= Constant.FileStreamSizeLimit Then
                Dim mt As New MemoryTributary()
                PdfViewerOriginal.GetPDFStream(mt)
                pdfFileStream = mt
            Else
                Dim fo As New FileOperation
                Dim tmpFile As String = fo.GetTempFile("tmp-")
                Dim fs As New FileStream(tmpFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
                PdfViewerOriginal.GetPDFStream(fs)
                pdfFileStream = fs
            End If

            ' MsgBox(cert.Certificate.Thumbprint, MsgBoxStyle.OkOnly, "Aláíró tanúsítvány SHA-1 lenyomata")

            ' we need to set ProgressPercent before the task is started so the document is in progress
            ' when SelectNextSignableDocument() is hit (which happens immediately after the task is stared)
            docItem.ProgressPercent = 1

            Dim t As New Task(Sub()
                                  Dim sigOp As New SignatureOperation()
                                  Dim res As New SignatureResult
                                  Dim startTime As Date = Date.Now
                                  Dim totalTime As TimeSpan

                                  'Thread.Sleep(30000) ' for debugging async operations

                                  res = sigOp.SignDocument(docItem, pdfFileStream, provider, cert, docDimensions, docItem.PartialCopyData, _FullUserName)

                                  docItem.SignatureLogContent = res.SignatureLog
                                  docItem.ErrorMessage = res.ErrorMessage

                                  If docItem.IsSigned AndAlso String.IsNullOrEmpty(docItem.ErrorMessage) Then
                                      totalTime = Date.Now - startTime
                                      _logger.Info(Messages.Document_Signed, docItem.FormTypeName, docItem.KofaxDocumentGUID, totalTime.ToString)
                                  Else
                                      _logger.Error(Messages.Document_Sign_Error, docItem.FormTypeName, docItem.KofaxDocumentGUID, docItem.ErrorMessage)

                                      If Not String.IsNullOrWhiteSpace(res.ExceptionText) Then
                                          _logger.Error(Messages.Document_Sign_Exception, res.ExceptionText)
                                      End If

                                      _logger.Error(res.SignatureLog) ' we log the whole crypto provider log in case of an error
                                  End If

                                  ' we set progress bar to the end, so the document becomes available even when exceptions happen
                                  docItem.ProgressPercent = 100
                              End Sub)
            _ActiveTasks.Add(t)

            If _LicenseInfo.AsyncOperation Then
                t.Start()
            Else
                t.RunSynchronously()
            End If

            Await FinishDocumentActionAsync(Messages.Batch_Closing)
        Finally
            _DocumentActionRunning = False
        End Try
    End Sub

    Private Sub BtnRemoveSignatures_Click(sender As Object, e As ItemClickEventArgs) Handles BtnRemoveSignatures.ItemClick
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        If MsgBox(Messages.Ask_Remove_Signatures, MsgBoxStyle.Question + MsgBoxStyle.YesNo, MessageBoxTitle) = MsgBoxResult.No Then Return

        PdfViewerSigned.CloseDocument()

        Dim docOp As New DocumentOperation
        docOp.DeleteSignature(doc, True)

        _logger.Info(Messages.Document_Signatures_Removed, doc.FormTypeName, doc.KofaxDocumentGUID)

        TabViewer.SelectedTabPage = TabOriginal
    End Sub

    Private Async Sub BtnSkipSign_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnSkipSign.ItemClick
        If _DocumentActionRunning Then Return
        _DocumentActionRunning = True
        Try
            If BatchTree.FocusedNode Is Nothing Then Return

            Dim doc As DocumentItem = Nothing
            Dim docOp As New DocumentOperation
            Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

            If currentItem.GetType Is GetType(BatchItem) Then Return

            If currentItem.GetType Is GetType(DocumentItem) Then
                doc = currentItem
            End If

            If currentItem.GetType Is GetType(PageItem) Then
                doc = CType(currentItem, PageItem).Document
            End If

            If doc.IsRejected Or doc.Pages.Any(Function(f) f.IsRejected) Then
                MsgBox(Messages.Rejected_Document_Cannot_Be_Skipped, MsgBoxStyle.Exclamation, MessageBoxTitle)
                Return
            End If

            Dim msg As String = doc.ErrorMessage
            If Not String.IsNullOrEmpty(doc.SkipNote) Then msg = doc.SkipNote

            docOp.SkipSigning(doc, msg)

            If doc.IsSkipped Then ' we only do something, if skipping really happened (user can cancel the procedure in the Notes window)
                _logger.Info(Messages.Document_Skipped, doc.FormTypeName, doc.KofaxDocumentGUID)

                PdfViewerSigned.CloseDocument()

                Await FinishDocumentActionAsync(Messages.Batch_Closing_With_Rejected_PDF)
            End If
        Finally
            _DocumentActionRunning = False
        End Try
    End Sub

    Private Sub BarComboProvider_EditValueChanged(sender As Object, e As EventArgs) Handles BarComboProvider.EditValueChanged
        Dim selectedProvider As CryptoProviderBase = TryCast(BarComboProvider.EditValue, CryptoProviderBase)

        If selectedProvider Is Nothing Then
            BarComboCert.Enabled = False
            Return
        End If

        If selectedProvider.SupportsLocalCertificates AndAlso BtnRefreshCertificates.Enabled Then
            BarComboCert.Enabled = True
            ' RefreshCryptoProviders triggers the refresh itself; avoid enumerating the cert store twice
            If Not _RefreshingProviders Then BtnRefreshCertificates_ItemClick(Nothing, Nothing)
        Else
            BarComboCert.Enabled = False
        End If
    End Sub

    Private Sub BtnRefreshCertificates_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnRefreshCertificates.ItemClick
        Dim qualifiedCertificatesOnly As Boolean
        Dim docItem As DocumentItem = GetDocumentItemFromBatchContents()

        If docItem Is Nothing Then
            qualifiedCertificatesOnly = False
        Else
            If docItem.SetupData.PDFSignerProvider.Enabled AndAlso
               TypeOf BarComboProvider.EditValue Is PDFSignerCryptoProvider AndAlso
               docItem.SetupData.PDFSignerProvider.AllowQualifiedCertificatesOnly Then
                qualifiedCertificatesOnly = True
            End If
        End If

        RefreshCertificates(qualifiedCertificatesOnly)
    End Sub

    Private Sub BtnViewOriginal_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnViewOriginal.ItemClick
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        Try
            Process.Start(doc.UnsignedFilePath)
        Catch
        End Try
    End Sub

    Private Sub BtnViewSigned_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnViewSigned.ItemClick
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        If doc.IsSigned Then
            Try
                Process.Start(doc.SignedFilePath)
            Catch
            End Try
        End If
    End Sub

    Private Async Sub BtnReject_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnReject.ItemClick
        If _DocumentActionRunning Then Return
        _DocumentActionRunning = True
        Try
            If BatchTree.FocusedNode Is Nothing Then Return

            Dim doc As DocumentItem = Nothing
            Dim docOp As New DocumentOperation
            Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

            If currentItem.GetType Is GetType(BatchItem) Then Return

            If currentItem.GetType Is GetType(DocumentItem) Then
                doc = currentItem
                docOp.Reject(CType(currentItem, DocumentItem))
            End If

            If currentItem.GetType Is GetType(PageItem) Then
                doc = CType(currentItem, PageItem).Document
                docOp.Reject(CType(currentItem, PageItem))
            End If

            If currentItem.IsRejected Then ' we only do something, if rejection really happened (user can cancel the procedure in the Reject window)
                _logger.Info(Messages.Document_Rejected, doc.FormTypeName, doc.KofaxDocumentGUID)

                PdfViewerSigned.CloseDocument()

                Await FinishDocumentActionAsync(Messages.Batch_Closing_With_Rejected_PDF)
            End If
        Finally
            _DocumentActionRunning = False
        End Try
    End Sub

    Private Sub BtnUnreject_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnUnreject.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return

        Dim doc As DocumentItem = Nothing
        Dim docOp As New DocumentOperation
        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType Is GetType(BatchItem) Then Return

        If currentItem.GetType Is GetType(DocumentItem) Then
            doc = currentItem
            docOp.Unreject(CType(currentItem, DocumentItem))
        End If
        If currentItem.GetType Is GetType(PageItem) Then
            doc = CType(currentItem, PageItem).Document
            docOp.Unreject(CType(currentItem, PageItem))
        End If

        _logger.Info(Messages.Document_Unrejected, doc.FormTypeName, doc.KofaxDocumentGUID)

        BatchTree.RefreshNode(BatchTree.FocusedNode)
    End Sub

    Private Sub BtnRotateLeft_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnRotateLeft.ItemClick
        If Not PdfViewerOriginal.SupportsRotate Then Return
        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType IsNot GetType(PageItem) Then Return

        Dim doc As DocumentItem = CType(currentItem, PageItem).Document

        TabViewer.SelectedTabPage = TabOriginal

        RotatePage(RotateDirection.Left)

        _logger.Info(Messages.Page_Rotated_Left, PdfViewerOriginal.CurrentPageNumber, doc.FormTypeName, doc.KofaxDocumentGUID)
    End Sub

    Private Sub BtnRotateRight_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnRotateRight.ItemClick
        If Not PdfViewerOriginal.SupportsRotate Then Return
        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType IsNot GetType(PageItem) Then Return

        Dim doc As DocumentItem = CType(currentItem, PageItem).Document

        TabViewer.SelectedTabPage = TabOriginal

        RotatePage(RotateDirection.Right)

        _logger.Info(Messages.Page_Rotated_Right, PdfViewerOriginal.CurrentPageNumber, doc.FormTypeName, doc.KofaxDocumentGUID)
    End Sub

    Private Sub BtnDeletePage_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnDeletePage.ItemClick
        If Not PdfViewerOriginal.SupportsDelete Then Return

        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)
        If currentItem.GetType IsNot GetType(PageItem) Then Return

        Dim currentPage As PageItem = CType(currentItem, PageItem)
        If currentPage.Document.Pages.Count = 1 Then
            MsgBox(Messages.Last_Page_Cannot_Be_Deleted, MsgBoxStyle.Exclamation, MessageBoxTitle)
            Return
        End If

        TabViewer.SelectedTabPage = TabOriginal

        If currentPage.Document.IsSigned Then
            If MsgBox(Messages.Ask_Delete_And_Remove_Signatures, MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, MessageBoxTitle) = MsgBoxResult.No Then Return

            Dim docOp As New DocumentOperation
            docOp.DeleteSignature(currentPage.Document, True)
        Else
            If MsgBox(Messages.Ask_Delete_Page, MsgBoxStyle.Question + MsgBoxStyle.YesNo, MessageBoxTitle) = MsgBoxResult.No Then Return
        End If

        Dim CurrentDocumentNode As TreeListNode = BatchTree.FocusedNode.ParentNode
        Dim CurrentDocumentItem As DocumentItem = BatchTree.GetDataRecordByNode(CurrentDocumentNode)
        Dim CurrentPageNumber As Integer = PdfViewerOriginal.CurrentPageNumber

        ' perform delete from control and save changes into original file
        PdfViewerOriginal.DeleteCurrentPage(CurrentDocumentItem.UnsignedFilePath)

        ' if next item is to be a document node then we backtrack to stay on the same document
        ' we have to perform this check after deleting from the PDF but before deleting from the Kofax DB
        If BatchTree.FocusedNode.NextVisibleNode IsNot Nothing Then
            Dim nextItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode.NextVisibleNode)
            If nextItem.GetType Is GetType(DocumentItem) Then
                BatchTree.MovePrevVisible()
            End If
        Else
            BatchTree.MovePrevVisible()
        End If

        CurrentDocumentItem.UnsignedFileLastTimeStamp = File.GetLastWriteTime(CurrentDocumentItem.UnsignedFilePath)
        SetCSSValue(CurrentDocumentItem.KofaxElement, CSS.UnsignedFileDateTime, CurrentDocumentItem.UnsignedFileLastTimeStamp)

        ' perform delete from batch tree and Kofax database
        currentPage.Delete()

        ' refresh the tree and renumber page numbers
        BatchTree.RefreshDataSource()

        _logger.Info(Messages.Page_Deleted, CurrentPageNumber, currentPage.Document.FormTypeName, currentPage.Document.KofaxDocumentGUID)

        For i As Integer = 0 To CurrentDocumentNode.Nodes.Count - 2 ' we need -2 instead of -1 because we deleted a page
            Dim item As PageItem = BatchTree.GetDataRecordByNode(CurrentDocumentNode.Nodes(i))
            item.OrderNumber = i + 1
            item.DisplayName = String.Format(GUIText.Page_Display_Name, item.OrderNumber, item.KofaxElement("ImageID"))
        Next

        ' we need to refresh the tree once more for renumbering to show up
        BatchTree.RefreshDataSource()
    End Sub

    Private Sub BtnAbout_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnAbout.ItemClick
        Dim frmActive As New FrmAbout With {
            .LicenseInfo = _LicenseInfo
        }
        frmActive.ShowDialog()
    End Sub

#End Region

#Region "Navigation ribbon"
    Private Sub BtnDocPrevious_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnDocPrevious.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType Is GetType(PageItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.ParentNode.PrevNode
        ElseIf currentItem.GetType Is GetType(DocumentItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.PrevNode
        Else
            Return
        End If
    End Sub

    Private Sub BtnDocNext_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnDocNext.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType Is GetType(PageItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.ParentNode.NextNode
        ElseIf currentItem.GetType Is GetType(DocumentItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.NextNode
        Else
            Return
        End If
    End Sub

    Private Sub BtnDocFirst_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnDocFirst.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return
        BatchTree.FocusedNode = BatchTree.Nodes(0).FirstNode
    End Sub

    Private Sub BtnDocLast_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnDocLast.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return
        BatchTree.FocusedNode = BatchTree.Nodes(0).LastNode
    End Sub

    Private Sub BtnPagePrevious_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnPagePrevious.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType Is GetType(PageItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.PrevNode
        Else
            Return
        End If
    End Sub

    Private Sub BtnPageNext_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles BtnPageNext.ItemClick
        If BatchTree.FocusedNode Is Nothing Then Return

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)

        If currentItem.GetType Is GetType(PageItem) Then
            BatchTree.FocusedNode = BatchTree.FocusedNode.NextNode
        ElseIf currentItem.GetType Is GetType(DocumentItem) Then ' if document node is selected then we go to the 2nd page if exists
            If BatchTree.FocusedNode.Nodes.Count > 1 Then
                BatchTree.FocusedNode = BatchTree.FocusedNode.Nodes(1)
            End If
        Else
            Return
        End If
    End Sub

#End Region

#Region "View ribbon"
    Private Sub BtnZoomWhole_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnZoomWhole.ItemClick
        PdfViewerOriginal.ZoomWholePage()
        PdfViewerSigned.ZoomWholePage()
    End Sub

    Private Sub BtnZoomWidth_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnZoomWidth.ItemClick
        PdfViewerOriginal.ZoomWidth()
        PdfViewerSigned.ZoomWidth()
    End Sub

    Private Sub BtnZoom1on1_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnZoom1on1.ItemClick
        PdfViewerOriginal.Zoom1on1()
        PdfViewerSigned.Zoom1on1()
    End Sub

    Private Sub BtnZoomIn_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnZoomIn.ItemClick
        PdfViewerOriginal.ZoomIn()
        PdfViewerSigned.ZoomIn()
    End Sub

    Private Sub BtnZoomOut_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnZoomOut.ItemClick
        PdfViewerOriginal.ZoomOut()
        PdfViewerSigned.ZoomOut()
    End Sub

    'Private Sub BtnLayoutSingle_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnLayoutSingle.ItemClick
    '    PdfViewerOriginal.SinglePageLayout()
    '    PdfViewerSigned.SinglePageLayout()
    'End Sub

    'Private Sub BtnLayoutDouble_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnLayoutDouble.ItemClick
    '    PdfViewerOriginal.DoublePageLayout()
    '    PdfViewerSigned.DoublePageLayout()
    'End Sub

    Private Sub BtnWorkspaceReset_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnWorkspaceReset.ItemClick
        Dim lm As New LayoutManager

        Try
            lm.ResetAllWorkspaces()
            MsgBox(Messages.Layout_Reset_Restart_Needed, MsgBoxStyle.Information, MessageBoxTitle)

        Catch ex As Exception
            _logger.Warn(Messages.Layout_Reset_Error, ex.ToString)
            MsgBox(String.Format(Messages.Layout_Reset_Error, ex.Message), MsgBoxStyle.Exclamation, MessageBoxTitle)
        End Try

        _SaveLayout = False ' So we do not save the layout on exit
    End Sub

#End Region

#End Region

    Private Sub ActivateDocument(DocItem As DocumentItem)
        If Not String.IsNullOrEmpty(DocItem.KofaxPDFFilePath) Then
            If DocItem.SetupData.FileOverwriteOriginal Then
                If Not File.Exists(DocItem.UnsignedFilePath) Then
                    If DocItem.IsSignatureNeeded Then
                        File.Move(DocItem.KofaxPDFFilePath, DocItem.UnsignedFilePath)
                    Else
                        File.Copy(DocItem.KofaxPDFFilePath, DocItem.UnsignedFilePath, True)
                    End If
                End If
            End If

            ' set viewers according to settings
            If DocItem.SetupData.DocumentViewer = DocumentViewerType.SimpleViewer Then
                If TypeOf PdfViewerOriginal IsNot DevExpressPDFViewer Then
                    TabOriginal.Controls.Remove(PdfViewerOriginal)
                    TabOriginal.Controls.Add(DevExpressPDFViewerOriginalControl)
                    PdfViewerOriginal = DevExpressPDFViewerOriginalControl
                    PdfViewerOriginal.Dock = DockStyle.Fill

                    TabSigned.Controls.Remove(PdfViewerSigned)
                    TabSigned.Controls.Add(DevExpressPDFViewerSignedControl)
                    PdfViewerSigned = DevExpressPDFViewerSignedControl
                    PdfViewerSigned.Dock = DockStyle.Fill
                End If
            End If
            If DocItem.SetupData.DocumentViewer = DocumentViewerType.AdvancedViewer Then
                If TypeOf PdfViewerOriginal IsNot TallPDFViewer Then
                    TabOriginal.Controls.Remove(PdfViewerOriginal)
                    TabOriginal.Controls.Add(TallPDFViewerOriginalControl)
                    PdfViewerOriginal = TallPDFViewerOriginalControl
                    PdfViewerOriginal.Dock = DockStyle.Fill

                    TabSigned.Controls.Remove(PdfViewerSigned)
                    TabSigned.Controls.Add(TallPDFViewerSignedControl)
                    PdfViewerSigned = TallPDFViewerSignedControl
                    PdfViewerSigned.Dock = DockStyle.Fill
                End If
            End If

            ' set cryptographic prviders and certificates
            RefreshCryptoProviders(DocItem)

            ' setting outher GUI elements
            BarComboCert.Enabled = True
            RibbonPageGroupEdit.Visible = PdfViewerOriginal.SupportsDocumentEditing

            PdfViewerOriginal.OpenDocument(DocItem.UnsignedFilePath)

            SetCSSValue(DocItem.KofaxElement, CSS.UnsignedFileDateTime, File.GetLastWriteTime(DocItem.UnsignedFilePath))

            DocItem.FileSize = New FileInfo(DocItem.UnsignedFilePath).Length

            If DocItem.IsSigned AndAlso File.Exists(DocItem.SignedFilePath) AndAlso TabViewer.SelectedTabPage Is TabSigned Then
                PdfViewerSigned.OpenDocument(DocItem.SignedFilePath)
            End If

            GrpPartialCopy.Enabled = True
        Else
            _logger.Warn(Messages.PDF_File_Not_Found, DocItem.FormTypeName)
            MsgBox(Messages.Kofax_Generated_PDF_File_Not_Found, MsgBoxStyle.Critical, MessageBoxTitle)

            Dim docOp As New DocumentOperation
            docOp.Reject(DocItem, String.Format(Messages.PDF_File_Not_Found, DocItem.FormTypeName))
            SelectNextSignableDocument(False)
        End If

        DocumentItemBindingSource.DataSource = DocItem
    End Sub

    Private Sub ActivatePage(PageNumber As Integer)
        PdfViewerOriginal.ActivatePage(PageNumber)

        If PdfViewerSigned.HasDocument Then
            PdfViewerSigned.ActivatePage(PageNumber)
        End If
    End Sub

    Private Sub DeactivateDocument(DeactivateViewer As Boolean)
        PdfViewerOriginal.CloseDocument()
        PdfViewerSigned.CloseDocument()
        GrpPartialCopy.Enabled = False

        If DeactivateViewer Then
            TabOriginal.Controls.Remove(PdfViewerOriginal)
            TabOriginal.Controls.Add(ViewerBaseOriginalControl)
            PdfViewerOriginal = ViewerBaseOriginalControl
            PdfViewerOriginal.Dock = DockStyle.Fill

            TabSigned.Controls.Remove(PdfViewerSigned)
            TabSigned.Controls.Add(ViewerBaseSignedControl)
            PdfViewerSigned = ViewerBaseSignedControl
            PdfViewerSigned.Dock = DockStyle.Fill
        End If

        BarComboCert.Enabled = False
        DocumentItemBindingSource.DataSource = GetType(DocumentItem)
    End Sub

    Private Sub BatchTree_FocusedNodeChanged(sender As Object, e As FocusedNodeChangedEventArgs) Handles BatchTree.FocusedNodeChanged
        If e.Node Is Nothing Then Return

        e.Node.Expanded = True

        Dim NewBatchItem As TreeItem = BatchTree.GetDataRecordByNode(e.Node)
        Dim OldBatchItem As TreeItem = BatchTree.GetDataRecordByNode(e.OldNode)

        If OldBatchItem Is Nothing Then Return

        If OldBatchItem.GetType Is GetType(DocumentItem) AndAlso NewBatchItem.GetType Is GetType(DocumentItem) Then
            ' docX->docY-ra ugrott
            DeactivateDocument(False)
            ActivateDocument(NewBatchItem)

        ElseIf OldBatchItem.GetType Is GetType(DocumentItem) AndAlso NewBatchItem.GetType Is GetType(BatchItem) Then
            ' doc->batch-re ugrott
            DeactivateDocument(True)

        ElseIf OldBatchItem.GetType Is GetType(BatchItem) AndAlso NewBatchItem.GetType Is GetType(DocumentItem) Then
            ' batch->doc-ra ugrott
            ActivateDocument(NewBatchItem)

        ElseIf OldBatchItem.GetType Is GetType(BatchItem) AndAlso NewBatchItem.GetType Is GetType(PageItem) Then
            ' batch->oldalra ugrott
            ActivateDocument(CType(NewBatchItem, PageItem).Document)
            ActivatePage(NewBatchItem.OrderNumber)

        ElseIf OldBatchItem.GetType Is GetType(PageItem) AndAlso NewBatchItem.GetType Is GetType(BatchItem) Then
            ' page->batch-re ugrott
            DeactivateDocument(True)

        ElseIf OldBatchItem.GetType Is GetType(PageItem) AndAlso NewBatchItem.GetType Is GetType(PageItem) Then
            ' oldal->oldalra ugrott
            If e.Node.ParentNode Is e.OldNode.ParentNode Then
                ' ugyanebben a doksiban
                ActivatePage(NewBatchItem.OrderNumber)

            Else
                ' másik doksiban
                DeactivateDocument(False)
                ActivateDocument(CType(NewBatchItem, PageItem).Document)
                ActivatePage(NewBatchItem.OrderNumber)
            End If

        ElseIf OldBatchItem.GetType Is GetType(PageItem) AndAlso NewBatchItem.GetType Is GetType(DocumentItem) Then
            ' oldal->doc-ra ugrott
            If e.OldNode.ParentNode Is e.Node Then
                ' ugyanebben a doksiban
                ActivatePage(1)

            Else
                ' másik doksiban
                DeactivateDocument(False)
                ActivateDocument(NewBatchItem)
                ActivatePage(1)
            End If

        ElseIf OldBatchItem.GetType Is GetType(DocumentItem) AndAlso NewBatchItem.GetType Is GetType(PageItem) Then
            ' doc->oldalra ugrott
            If e.Node.ParentNode Is e.OldNode Then
                ' ugyanebben a doksiban
                ActivatePage(NewBatchItem.OrderNumber)

            Else
                ' másik doksiban
                DeactivateDocument(False)
                ActivateDocument(CType(NewBatchItem, PageItem).Document)
                ActivatePage(NewBatchItem.OrderNumber)
            End If
        End If
    End Sub

    Private Sub BatchTree_CustomDrawNodeCell(sender As Object, e As CustomDrawNodeCellEventArgs) Handles BatchTree.CustomDrawNodeCell
        Dim viewInfo = TryCast(e.EditViewInfo, ProgressBarViewInfo)
        If viewInfo Is Nothing Then Return

        Dim doc = TryCast(e.Node.TreeList.GetDataRecordByNode(e.Node), DocumentItem)
        If doc Is Nothing Then Return

        If viewInfo.Position = 100 Then
            If doc.HasError Then
                viewInfo.ProgressInfo.StartColor = Color.OrangeRed
                viewInfo.ProgressInfo.EndColor = Color.OrangeRed
            Else
                viewInfo.ProgressInfo.StartColor = Color.ForestGreen
                viewInfo.ProgressInfo.EndColor = Color.ForestGreen
            End If
        Else
            viewInfo.ProgressInfo.StartColor = SystemColors.Highlight
            viewInfo.ProgressInfo.EndColor = SystemColors.Highlight
        End If
    End Sub

    ''' <summary>
    ''' Selects the first unsigned document in the batch that needs signature
    ''' </summary>
    ''' <returns>true, if a signable document was found and a different node was selected in the batch content's tree</returns>
    ''' <returns>false, if no signable document was found</returns>
    Private Function SelectNextSignableDocument(AlwaysShowDocument As Boolean) As Boolean
        Dim batchCont As BindingList(Of TreeItem) = CType(BatchTree.DataSource, BatchContents).Content
        Dim currentTreeItem As TreeItem = CType(BatchTree.GetDataRecordByNode(BatchTree.FocusedNode), TreeItem)
        Dim currentDocItem As DocumentItem = Nothing
        Dim nextDocItem As DocumentItem

        If TypeOf currentTreeItem Is DocumentItem Then
            currentDocItem = currentTreeItem
        ElseIf TypeOf currentTreeItem Is PageItem Then
            currentDocItem = CType(currentTreeItem, PageItem).Document
        End If

        ' search after the current document
        nextDocItem = batchCont.OfType(Of DocumentItem).FirstOrDefault(Function(f) f.IsSignatureNeeded AndAlso Not f.IsSigned AndAlso Not f.IsSkipped AndAlso Not f.IsRejected AndAlso Not f.Pages.Any(Function(g) g.IsRejected) AndAlso f.Available AndAlso f.ID > currentTreeItem.ID)
        If nextDocItem IsNot Nothing Then
            Dim firstNode = BatchTree.FindNodeByKeyID(nextDocItem.ID)
            BatchTree.FocusedNode = firstNode
            Return True
        End If

        ' if no signable document after current, then start from the beginning of batch
        nextDocItem = batchCont.OfType(Of DocumentItem).FirstOrDefault(Function(f) f.IsSignatureNeeded AndAlso Not f.IsSigned AndAlso Not f.IsSkipped AndAlso Not f.IsRejected AndAlso Not f.Pages.Any(Function(g) g.IsRejected) AndAlso f.Available AndAlso f IsNot currentDocItem)
        If nextDocItem IsNot Nothing Then
            Dim firstNode = BatchTree.FindNodeByKeyID(nextDocItem.ID)
            BatchTree.FocusedNode = firstNode
            Return True
        End If

        ' if no signable document at all, then open first document, if requested
        If AlwaysShowDocument Then
            nextDocItem = batchCont.OfType(Of DocumentItem).FirstOrDefault
            If nextDocItem IsNot Nothing Then
                Dim firstNode = BatchTree.FindNodeByKeyID(nextDocItem.ID)
                BatchTree.FocusedNode = firstNode
                Return False ' we didn't find any document, just displayed the first one, hence False is returned
            End If
        End If

        Return False
    End Function

    Private Function AllDocumentsProcessed() As Boolean
        Dim batchCont As BindingList(Of TreeItem) = CType(BatchTree.DataSource, BatchContents).Content

        ' checking for unprocessed documents
        ' if a document is not available then it's not processed (but currently being processed)
        Dim res As Boolean = batchCont.OfType(Of DocumentItem).Where(Function(f) (f.IsSignatureNeeded AndAlso Not f.IsSigned AndAlso Not f.IsRejected AndAlso Not f.Pages.Any(Function(g) g.IsRejected) AndAlso Not f.IsSkipped) _
                                                    OrElse Not f.Available).Any

        ' return true if no unprocessed documents found
        Return Not res
    End Function

    Private Sub RotatePage(Direction As RotateDirection)
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        If doc.IsSigned Then
            If MsgBox(Messages.Ask_Rotate_And_Remove_Signatures, MsgBoxStyle.Exclamation + MsgBoxStyle.YesNo, MessageBoxTitle) = MsgBoxResult.No Then Return

            Dim docOp As New DocumentOperation
            docOp.DeleteSignature(doc, True)

            PdfViewerSigned.CloseDocument()

            BatchTree.RefreshDataSource()
        End If

        PdfViewerOriginal.RotatePage(Direction, doc.UnsignedFilePath)

        doc.UnsignedFileLastTimeStamp = File.GetLastWriteTime(doc.UnsignedFilePath)
        SetCSSValue(doc.KofaxElement, CSS.UnsignedFileDateTime, doc.UnsignedFileLastTimeStamp)
    End Sub

    Private Sub RefreshCryptoProviders(docitem As DocumentItem)
        Dim activeProviders As List(Of CryptoProviderBase) = docitem.SetupData.CryptographicProviders.Where(Function(x) x.Enabled).ToList

        _RefreshingProviders = True
        Try
            ComboProvider.Items.Clear()

            For Each provider As CryptoProviderBase In activeProviders
                ComboProvider.Items.Add(provider)

                If provider.ProviderType = docitem.SetupData.DefaultCryptographicProvider Then
                    BarComboProvider.EditValue = provider
                End If
            Next

            ' reset stale selection when the previous document's provider is not offered here
            Dim currentSelection As CryptoProviderBase = TryCast(BarComboProvider.EditValue, CryptoProviderBase)
            If currentSelection Is Nothing OrElse Not activeProviders.Contains(currentSelection) Then
                BarComboProvider.EditValue = activeProviders.FirstOrDefault
            End If
        Finally
            _RefreshingProviders = False
        End Try

        ' enumerate the certificate store only when the selected provider can actually use local certificates
        Dim selectedProvider As CryptoProviderBase = TryCast(BarComboProvider.EditValue, CryptoProviderBase)
        If selectedProvider IsNot Nothing AndAlso selectedProvider.SupportsLocalCertificates Then
            BtnRefreshCertificates_ItemClick(Nothing, Nothing)
        End If
    End Sub

    Private Sub RefreshCertificates(QualifiedCertificatesOnly As Boolean)
        Dim sigOp As New SignatureOperation
        Dim certs As List(Of SigningCertificate) = sigOp.GetCertificatesFromMyWinCertStore(QualifiedCertificatesOnly)

        ComboCert.Items.Clear()
        For Each c As SigningCertificate In certs
            ComboCert.Items.Add(c)
        Next
    End Sub

    Private Function CurrentDomain_AssemblyResolve(sender As Object, args As ResolveEventArgs) As Assembly Handles CurrentDomain.AssemblyResolve
        Dim newPath As String = Path.Combine(KofaxRegistry.KofaxBinPath, New AssemblyName(args.Name).Name + ".dll")
        If Not File.Exists(newPath) Then Return Nothing

        Dim newAssembly As Assembly = Assembly.LoadFrom(newPath)
        Return newAssembly
    End Function

    Private Sub ComboPartialCopy_Validated(sender As Object, e As EventArgs) Handles ComboPartialCopy.Validated
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        doc.PartialCopyData = ComboPartialCopy.Text
        SetCSSValue(doc.KofaxElement, CSS.PartialCopyData, doc.PartialCopyData)
    End Sub

    Private Sub BtnRegulation_ItemClick(sender As Object, e As ItemClickEventArgs) Handles BtnRegulation.ItemClick
        Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
        If doc Is Nothing Then Return

        If doc.SetupData.ConvertingRegulationURL IsNot Nothing AndAlso Not String.IsNullOrEmpty(doc.SetupData.ConvertingRegulationURL.ToString) Then
            HelpProvider.ShowURI(doc.SetupData.ConvertingRegulationURL)
        End If
    End Sub

    Private Function GetDocumentItemFromBatchContents() As DocumentItem
        If BatchTree.FocusedNode Is Nothing Then Return Nothing

        Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(BatchTree.FocusedNode)
        Dim doc As DocumentItem

        If currentItem.GetType Is GetType(PageItem) Then
            doc = CType(currentItem, PageItem).Document
        ElseIf currentItem.GetType Is GetType(DocumentItem) Then
            doc = currentItem
        Else
            Return Nothing
        End If

        Return doc
    End Function

    Private Sub TabViewer_SelectedPageChanged(sender As Object, e As DevExpress.XtraTab.TabPageChangedEventArgs) Handles TabViewer.SelectedPageChanged
        If TabViewer.SelectedTabPage Is TabSigned Then
            If PdfViewerSigned.HasDocument Then Return

            Dim doc As DocumentItem = GetDocumentItemFromBatchContents()
            If doc Is Nothing Then Return

            If doc.IsSigned AndAlso File.Exists(doc.SignedFilePath) Then
                PdfViewerSigned.OpenDocument(doc.SignedFilePath)
            End If
        End If
    End Sub

    Private Sub BatchTreeToolTip_GetActiveObjectInfo(sender As Object, e As DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs) Handles BatchTreeToolTip.GetActiveObjectInfo
        If e.SelectedControl IsNot BatchTree Then Return

        Dim hitInfo = BatchTree.CalcHitInfo(e.ControlMousePosition)
        If hitInfo.HitInfoType = HitInfoType.Cell AndAlso hitInfo.Column.ColumnEditName Is BatchTreeProgressBar.Name Then
            Dim currentItem As TreeItem = BatchTree.GetDataRecordByNode(hitInfo.Node)

            If currentItem.GetType Is GetType(DocumentItem) Then
                Dim doc As DocumentItem = currentItem

                If doc.ProgressPercent = 100 AndAlso doc.HasError Then
                    e.Info = New ToolTipControlInfo(hitInfo.Column.FieldName + BatchTree.GetNodeIndex(hitInfo.Node).ToString(), doc.ErrorMessage)
                End If
            End If
        End If
    End Sub

    Private Sub FrmPdfSigner_KeyDown_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown, MyBase.KeyUp
        _CtrlIsDown = e.Control
    End Sub

    Private Sub FrmPdfSigner_MouseWheel(sender As Object, e As MouseEventArgs) Handles MyBase.MouseWheel
        If Not _CtrlIsDown Then Return

        Select Case Math.Sign(e.Delta)
            Case Is < 0
                PdfViewerOriginal.ZoomOut()
                PdfViewerSigned.ZoomOut()
            Case Is > 0
                PdfViewerOriginal.ZoomIn()
                PdfViewerSigned.ZoomIn()
        End Select
    End Sub

End Class