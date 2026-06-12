Imports System.Runtime.InteropServices

Friend NotInheritable Class FileUtil
    Private Sub New()
    End Sub

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure RM_UNIQUE_PROCESS
        Public dwProcessId As Integer
        Public ProcessStartTime As System.Runtime.InteropServices.ComTypes.FILETIME
    End Structure

    Const RmRebootReasonNone As Integer = 0
    Const CCH_RM_MAX_APP_NAME As Integer = 255
    Const CCH_RM_MAX_SVC_NAME As Integer = 63

    Private Enum RM_APP_TYPE
        RmUnknownApp = 0
        RmMainWindow = 1
        RmOtherWindow = 2
        RmService = 3
        RmExplorer = 4
        RmConsole = 5
        RmCritical = 1000
    End Enum

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Private Structure RM_PROCESS_INFO
        Public Process As RM_UNIQUE_PROCESS

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=CCH_RM_MAX_APP_NAME + 1)> _
        Public strAppName As String

        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=CCH_RM_MAX_SVC_NAME + 1)> _
        Public strServiceShortName As String

        Public ApplicationType As RM_APP_TYPE
        Public AppStatus As UInteger
        Public TSSessionId As UInteger
        <MarshalAs(UnmanagedType.Bool)> _
        Public bRestartable As Boolean
    End Structure

    <DllImport("rstrtmgr.dll", CharSet:=CharSet.Unicode)> _
    Private Shared Function RmRegisterResources(pSessionHandle As UInteger, nFiles As UInt32, rgsFilenames As String(), nApplications As UInt32, <[In]> rgApplications As RM_UNIQUE_PROCESS(), nServices As UInt32, _
        rgsServiceNames As String()) As Integer
    End Function

    <DllImport("rstrtmgr.dll", CharSet:=CharSet.Auto)> _
    Private Shared Function RmStartSession(ByRef pSessionHandle As UInteger, dwSessionFlags As Integer, strSessionKey As String) As Integer
    End Function

    <DllImport("rstrtmgr.dll")> _
    Private Shared Function RmEndSession(pSessionHandle As UInteger) As Integer
    End Function

    <DllImport("rstrtmgr.dll")> _
    Private Shared Function RmGetList(dwSessionHandle As UInteger, ByRef pnProcInfoNeeded As UInteger, ByRef pnProcInfo As UInteger, <[In], Out> rgAffectedApps As RM_PROCESS_INFO(), ByRef lpdwRebootReasons As UInteger) As Integer
    End Function

    ''' <summary>
    ''' Find out what process(es) have a lock on the specified file.
    ''' </summary>
    ''' <param name="path">Path of the file.</param>
    ''' <returns>Processes locking the file</returns>
    ''' <remarks>See also:
    ''' http://msdn.microsoft.com/en-us/library/windows/desktop/aa373661(v=vs.85).aspx
    ''' http://wyupdate.googlecode.com/svn-history/r401/trunk/frmFilesInUse.cs (no copyright in code at time of viewing)
    ''' 
    ''' </remarks>
    Friend Shared Function WhoIsLocking(path As String) As List(Of Process)
        Dim handle As UInteger
        Dim key As String = Guid.NewGuid().ToString()
        Dim processes As New List(Of Process)()

        Dim res As Integer = RmStartSession(handle, 0, key)
        If res <> 0 Then
            Throw New Exception("Could not begin restart session.  Unable to determine file locker.")
        End If

        Try
            Const ERROR_MORE_DATA As Integer = 234
            Dim pnProcInfoNeeded As UInteger = 0, pnProcInfo As UInteger = 0, lpdwRebootReasons As UInteger = RmRebootReasonNone

            Dim resources As String() = New String() {path}
            ' Just checking on one resource.
            res = RmRegisterResources(handle, CUInt(resources.Length), resources, 0, Nothing, 0, _
                Nothing)

            If res <> 0 Then
                Throw New Exception("Could not register resource.")
            End If

            'Note: there's a race condition here -- the first call to RmGetList() returns
            '      the total number of process. However, when we call RmGetList() again to get
            '      the actual processes this number may have increased.
            res = RmGetList(handle, pnProcInfoNeeded, pnProcInfo, Nothing, lpdwRebootReasons)

            If res = ERROR_MORE_DATA Then
                ' Create an array to store the process results
                Dim processInfo As RM_PROCESS_INFO() = New RM_PROCESS_INFO(pnProcInfoNeeded - 1) {}
                pnProcInfo = pnProcInfoNeeded

                ' Get the list
                res = RmGetList(handle, pnProcInfoNeeded, pnProcInfo, processInfo, lpdwRebootReasons)
                If res = 0 Then
                    processes = New List(Of Process)(CInt(pnProcInfo))

                    ' Enumerate all of the results and add them to the 
                    ' list to be returned
                    For i As Integer = 0 To pnProcInfo - 1
                        Try
                            processes.Add(Process.GetProcessById(processInfo(i).Process.dwProcessId))
                            ' catch the error -- in case the process is no longer running
                        Catch generatedExceptionName As ArgumentException
                        End Try
                    Next
                Else
                    Throw New Exception("Could not list processes locking resource.")
                End If
            ElseIf res <> 0 Then
                Throw New Exception("Could not list processes locking resource. Failed to get size of result.")
            End If
        Finally
            RmEndSession(handle)
        End Try

        Return processes
    End Function
End Class
