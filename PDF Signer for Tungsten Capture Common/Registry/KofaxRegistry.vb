Imports Microsoft.Win32

Public Module KofaxRegistry
    ' Always get the 32 bit view of the registry, even if application is running in 64-bit mode
    ReadOnly LocalMachineRegKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\Kofax Image Products\Ascent Capture\3.0", False)

    Public ReadOnly Property KofaxVersion As String
        Get
            Try
                Dim res As String = LocalMachineRegKey.GetValue("AppTitle").ToString()
                Return res
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property KofaxBinPath As String
        Get
            Try
                Dim res As String = LocalMachineRegKey.GetValue("ExePath").ToString()
                Return res
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property KofaxLogFolder As String
        Get
            Try
                Dim res As String = LocalMachineRegKey.GetValue("ServerPath").ToString() + "\Logs"
                Return res
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property

    Public ReadOnly Property KofaxPDFSignerLicenseFolder As String
        Get
            Try
                Dim res As String = LocalMachineRegKey.GetValue("ServerPath").ToString() + "\License"
                Return res
            Catch ex As Exception
                Return String.Empty
            End Try
        End Get
    End Property
End Module
