Imports System.IO
Imports DevExpress.Utils
Imports NLog
Imports PDFSigner.My.Resources

Friend Class LayoutManager
    Const extension As String = ".xml"
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()
    Private ReadOnly LayoutPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PDF Signer", "Layouts")

    Friend Sub LoadWorkspace(WsManager As WorkspaceManager, FormName As String)
        Dim success As Boolean
        Dim layoutXmlPath = Path.Combine(LayoutPath, FormName + extension)

        Try
            If File.Exists(layoutXmlPath) Then
                success = WsManager.LoadWorkspace(FormName, layoutXmlPath)
            Else
                Return
            End If

            If success Then
                WsManager.ApplyWorkspace(FormName)
            End If

        Catch ex As Exception
            _logger.Warn(Messages.Workspace_Load_Error, FormName, ex.ToString)

            Try
                If File.Exists(layoutXmlPath) Then
                    File.Delete(layoutXmlPath)
                End If
            Catch
            End Try
        End Try
    End Sub

    Friend Sub SaveWorkspace(WsManager As WorkspaceManager, FormName As String)
        Dim layoutXmlPath = Path.Combine(LayoutPath, FormName + extension)

        Try
            WsManager.CaptureWorkspace(FormName)

            If Not Directory.Exists(LayoutPath) Then
                Directory.CreateDirectory(LayoutPath)
            End If

            WsManager.SaveWorkspace(FormName, layoutXmlPath, True)
        Catch ex As Exception
            _logger.Warn(Messages.Workspace_Save_Error, FormName, ex.ToString)
        End Try
    End Sub

    Friend Sub ResetAllWorkspaces()
        If Not Directory.Exists(LayoutPath) Then Return

        Dim files As String() = Directory.GetFiles(LayoutPath)

        For Each f As String In files
            File.Delete(f)
        Next
    End Sub
End Class
