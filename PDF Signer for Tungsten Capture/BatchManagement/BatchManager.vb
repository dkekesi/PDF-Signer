Imports System.Runtime.InteropServices
Imports PDFSigner.My.Resources
Imports NLog
Imports System.IO
Imports Kofax.Capture.DBLite
Imports Kofax.Capture.SDK.CustomModule

Friend Class BatchManager

    Private Const THIS_FILE As String = "PDFSigner.BatchManager"
    Private _KfxLogin As Login ' Access to database
    Private _KfxRuntimeSession As IRuntimeSession ' Access to batches
    Private _KfxActiveBatch As IBatch ' Selected batch object
    Private _processID As Integer ' Process ID for this queue
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Friend Property UniqueID As String
    Friend ReadOnly Property ActiveBatch() As Batch
        Get
            Return _KfxActiveBatch
        End Get
    End Property

    Friend Sub LoginToKofax()
        Try
            ' Initialize the login object
            If _KfxLogin Is Nothing Then
                _KfxLogin = New Login With {
                    .EnableSecurityBoost = True
                }
                _KfxLogin.Login(String.Empty, String.Empty)
                _KfxLogin.ApplicationName = Path.GetFileNameWithoutExtension(Application.ExecutablePath)
                _KfxLogin.Version = FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetExecutingAssembly.Location).FileMajorPart & "." & FileVersionInfo.GetVersionInfo(Reflection.Assembly.GetExecutingAssembly.Location).FileMinorPart
            End If

            _KfxLogin.ValidateUser(UniqueID)
            _KfxRuntimeSession = _KfxLogin.RuntimeSession
            _processID = _KfxLogin.ProcessID

        Catch ex As COMException
            _logger.Error(ex.ToString)

            If _KfxLogin IsNot Nothing Then
                If TypeOf ex Is COMException Then
                    If ex.ErrorCode = -2147217495 Then
                        _KfxLogin.LogError(ex.ErrorCode, 0, 0, THIS_FILE & ".LoginToRuntimeSession", Erl(), String.Format(Messages.Module_Not_Registered, CustomModuleID), True)
                    Else
                        _KfxLogin.LogError(ex.ErrorCode, 0, 0, THIS_FILE & ".LoginToRuntimeSession", Erl(), ex.ToString(), True)
                    End If
                Else
                    _KfxLogin.LogError(ex.ErrorCode, 0, 0, THIS_FILE & ".LoginToRuntimeSession", Erl(), ex.ToString(), True)
                End If
            End If

            MsgBox(ex.Message)
        End Try
    End Sub

    Friend Sub LogoutFromKofax()
        _KfxLogin?.Logout()
    End Sub

    ''' <summary>
    ''' Opens a batch by ID or by selecting it from the Open Batch window
    ''' </summary>
    ''' <param name="ParentForm">The form instance to which the OpenBatch form is centered</param>
    ''' <param name="SingleBatchOpenID">ID of the batch to open</param>
    ''' <returns>True if batch is successfully opened, False if any error encountered.</returns>
    ''' <remarks></remarks>
    Friend Function BatchSelect(ParentForm As Form, SingleBatchOpenID As Integer) As Boolean
        Try
            Debug.Assert(_KfxActiveBatch Is Nothing, String.Empty)
            Debug.Assert(_KfxRuntimeSession IsNot Nothing, String.Empty)
            Debug.Assert(_processID > 0, String.Empty)

            If SingleBatchOpenID = 0 Then
                Dim FrmBatchMan As New FrmOpenBatch With {
                    .KfxRunTimeSession = _KfxRuntimeSession,
                    .KfxProcessID = _processID
                }
                FrmBatchMan.ShowDialog(ParentForm)

                If FrmBatchMan.Batch IsNot Nothing Then
                    SingleBatchOpenID = FrmBatchMan.Batch.BatchId
                End If
            End If

            If SingleBatchOpenID > 0 Then ' If a batch was selected, lock it.
                Try
                    _KfxActiveBatch = _KfxRuntimeSession.BatchOpen(SingleBatchOpenID, _processID)
                    Return True

                Catch ex As Exception
                    MsgBox(Messages.Batch_Not_Available, vbExclamation, MessageBoxTitle)
                    Return False
                End Try
            End If

            Return False

        Catch ex As Exception
            _logger.Error(ex.ToString)
            _KfxLogin.LogError(Err.Number, 0, 0, THIS_FILE & ".BatchSelect", Erl(), Err.Description, True)
            Return False
        End Try

    End Function

    Friend Sub BatchClose(eNewState As KfxDbState)
        Dim msg As String
        Dim eQueue As KfxDbQueue

        Try
            ' Determine what queue to move to based on state.
            Select Case eNewState
                Case KfxDbState.KfxDbBatchError
                    msg = String.Format(Messages.Batch_Closed_Has_Rejected, _KfxActiveBatch.Name)
                    eQueue = KfxDbQueue.KfxDbQueueException
                Case KfxDbState.KfxDbBatchReady
                    msg = String.Format(Messages.Batch_Closed, _KfxActiveBatch.Name)
                    eQueue = KfxDbQueue.KfxDbQueueNext
                Case KfxDbState.KfxDbBatchCompleted
                    msg = String.Format(Messages.Batch_Closed_Completed, _KfxActiveBatch.Name)
                    eQueue = KfxDbQueue.KfxDbQueueSame
                Case Else
                    msg = String.Format(Messages.Batch_Suspended, _KfxActiveBatch.Name)
                    eQueue = KfxDbQueue.KfxDbQueueSame
            End Select

            ' Close the batch
            _KfxActiveBatch.BatchClose(eNewState, eQueue, 0, String.Empty)
            _logger.Info(msg)

        Catch ex As Exception
            _logger.Error(ex.ToString)
            _KfxLogin.LogError(Err.Number, 0, 0, Err.Source, Err.Erl, ex.Message, True)
        Finally
            _KfxActiveBatch = Nothing
        End Try
    End Sub

End Class