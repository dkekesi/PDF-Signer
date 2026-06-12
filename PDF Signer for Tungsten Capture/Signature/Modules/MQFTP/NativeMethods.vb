Imports System.Runtime.InteropServices

NotInheritable Class NativeMethods
    Private Sub New()
    End Sub

    Friend Enum LogonType
        LOGON32_LOGON_INTERACTIVE = 2
        LOGON32_LOGON_NETWORK = 3
        LOGON32_LOGON_BATCH = 4
        LOGON32_LOGON_SERVICE = 5
        LOGON32_LOGON_UNLOCK = 7
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8
        LOGON32_LOGON_NEW_CREDENTIALS = 9
    End Enum

    Friend Enum LogonProvider
        LOGON32_PROVIDER_DEFAULT = 0
        LOGON32_PROVIDER_WINNT35 = 1
        LOGON32_PROVIDER_WINNT40 = 2
        LOGON32_PROVIDER_WINNT50 = 3
    End Enum

    Friend Enum ImpersonationLevel
        SecurityAnonymous = 0
        SecurityIdentification = 1
        SecurityImpersonation = 2
        SecurityDelegation = 3
    End Enum

    <DllImport("advapi32.dll", SetLastError:=True)> _
    Friend Shared Function LogonUser(lpszUsername As String, lpszDomain As String, lpszPassword As String, dwLogonType As Integer, dwLogonProvider As Integer, ByRef phToken As IntPtr) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)> _
    Friend Shared Function CloseHandle(hObject As IntPtr) As Boolean
    End Function
End Class