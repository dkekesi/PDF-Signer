Imports Microsoft.Win32
Imports NLog
Imports System.ComponentModel

Friend Module PDFSignerCurrentUserRegistry
    Const PDFSignerRegKey As String = "SOFTWARE\Kofax Image Products\Ascent Capture\DocSoft PDF Signer"
    Const BatchFiltersRegKey As String = "Batch Filters"
    Friend Const CurrentBatchFilterEntryName As String = "CurrentFilter"
    Private ReadOnly CurrentUserKey As RegistryKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32).CreateSubKey(PDFSignerRegKey) ' Always get the 32 bit view of the registry, even if application is running in 64-bit mode
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Friend Function LoadBatchFilterList() As BindingList(Of BatchFilter)
        Dim res As New BindingList(Of BatchFilter)
        Dim errMsg As String = "Hiba a mentett batch filter lista registryből történő betöltésekor:"

        If CurrentUserKey Is Nothing Then
            _logger.Warn("{0} a HKCU hive 32 bites nézetében nem érhető el a {1} kulcs", errMsg, PDFSignerRegKey)
            Return Nothing
        End If

        Try
            Dim key As RegistryKey = CurrentUserKey.CreateSubKey(BatchFiltersRegKey)

            For Each value As String In key.GetValueNames
                If value <> CurrentBatchFilterEntryName Then
                    Dim bf As New BatchFilter With {
                        .Name = value
                    }
                    res.Add(bf)
                End If
            Next

            Return res

        Catch ex As Exception
            _logger.Warn("{0} {1}", errMsg, ex.ToString)
            Return Nothing
        End Try
    End Function

    Friend Function LoadBatchFilterXml(Name As String) As String
        Dim errMsg As String = "Hiba a batch filter XML registryből történő betöltésekor:"

        If CurrentUserKey Is Nothing Then
            _logger.Warn("{0} a HKCU hive 32 bites nézetében nem érhető el a {1} kulcs", errMsg, PDFSignerRegKey)
            Return String.Empty
        End If

        Try
            Dim key As RegistryKey = CurrentUserKey.CreateSubKey(BatchFiltersRegKey)
            Dim res As Object = key.GetValue(Name)

            If res Is Nothing Then
                Return String.Empty
            Else
                Return res.ToString
            End If

        Catch ex As Exception
            _logger.Warn("{0} {1}", errMsg, ex.ToString)
            Return String.Empty
        End Try
    End Function

    Friend Sub SaveBatchFilterXml(BatchFilterXml As String, Name As String)
        Dim errMsg As String = "Hiba a batch filter XML registrybe történő mentésekor:"

        If CurrentUserKey Is Nothing Then
            _logger.Warn("{0} a HKCU hive 32 bites nézetében nem érhető el a {1} kulcs", errMsg, PDFSignerRegKey)
            Return
        End If

        Try
            Dim regkey As RegistryKey = CurrentUserKey.CreateSubKey(BatchFiltersRegKey)
            regkey.SetValue(Name, BatchFilterXml)
        Catch ex As Exception
            _logger.Warn("{0} {1}", errMsg, ex.ToString)
        End Try
    End Sub

    Friend Sub DeleteBatchFilter(Name As String)
        Dim errMsg As String = "Hiba a batch filter registryből történő törlésekor:"

        If CurrentUserKey Is Nothing Then
            _logger.Warn("{0} a HKCU hive 32 bites nézetében nem érhető el a {1} kulcs", errMsg, PDFSignerRegKey)
            Return
        End If

        Try
            Dim key As RegistryKey = CurrentUserKey.CreateSubKey(BatchFiltersRegKey)
            key.DeleteValue(Name)
        Catch ex As Exception
            _logger.Warn("{0} {1}", errMsg, ex.ToString)
        End Try
    End Sub

    Friend Sub DeleteAllBatchFilters()
        Dim errMsg As String = "Hiba a batch filter registryből történő törlésekor:"

        If CurrentUserKey Is Nothing Then
            _logger.Warn("{0} a HKCU hive 32 bites nézetében nem érhető el a {1} kulcs", errMsg, PDFSignerRegKey)
            Return
        End If

        Try
            Dim key As RegistryKey = CurrentUserKey.CreateSubKey(BatchFiltersRegKey)

            For Each value As String In key.GetValueNames
                If value <> CurrentBatchFilterEntryName Then
                    key.DeleteValue(value)
                End If
            Next

        Catch ex As Exception
            _logger.Warn("{0} {1}", errMsg, ex.ToString)
        End Try
    End Sub

End Module