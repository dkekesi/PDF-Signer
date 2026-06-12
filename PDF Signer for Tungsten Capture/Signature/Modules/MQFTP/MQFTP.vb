Imports System.Text
Imports System.Security
Imports System.Security.Principal
Imports System.IO
Imports PDFSignerCommon

Friend Class MQFTP
    Private ReadOnly _sbSignLog As New StringBuilder
    Private _settings As MQFTPCryptoProvider

    Friend Sub Initialize(ProviderSettings As MQFTPCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Function SignDocument(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult
        Dim responseArrived As Boolean
        Dim StagingFolderName As String = "Staging"
        Dim MQFolderOutStaging As String = Path.Combine(_settings.MQFolderOut, StagingFolderName)
        Dim timeOut As Integer = CalculateTimeOut(Request.FileToSign.Length)
        Dim signedDoc As Stream = Nothing
        Dim fileOp As New FileOperation

        _sbSignLog.AppendLine("Kapcsolódás az MQFTP cserekönyvtárakhoz")

        Using ImpersonateIdentity(_settings.MQDomain, _settings.MQUserName, _settings.MQPassword)
            If Not Directory.Exists(_settings.MQFolderOut) Then
                res.ErrorMessage = $"Az MQFTP kimeneti cserekönyvtár '{_settings.MQFolderOut}' nem létezik vagy nem olvasható!"
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            If Not Directory.Exists(MQFolderOutStaging) Then
                res.ErrorMessage = $"Az MQFTP kimeneti staging könyvtár '{MQFolderOutStaging}' nem létezik vagy nem olvasható!"
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            If Not Directory.Exists(_settings.MQFolderIn.ToString) Then
                res.ErrorMessage = $"Az MQFTP bemeneti cserekönyvtár '{_settings.MQFolderIn}' nem létezik vagy nem olvasható!"
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            _sbSignLog.AppendLine("MQFTP cserekönyvtárak elérhetősége ellenőrizve")

            Dim pdfFileNameWithExtension As String = Request.DocItem.KofaxDocumentGUID + ".pdf"
            Dim pdfOutStagingPath As String = Path.Combine(_settings.MQFolderOut, StagingFolderName, pdfFileNameWithExtension)
            Dim pdfOutPath As String = Path.Combine(_settings.MQFolderOut, pdfFileNameWithExtension)
            Dim pdfInPath As String = Path.Combine(_settings.MQFolderIn, pdfFileNameWithExtension)

            ' delete existing files that shall not be there
            fileOp.DeleteWhenAvailable(pdfOutPath)
            fileOp.DeleteWhenAvailable(pdfInPath)

            ' write file into output MQ exchange folder
            _sbSignLog.AppendLine("Aláírás nélküli PDF fájl küldése hitelesítésre")
            Using fs As New FileStream(pdfOutStagingPath, FileMode.Create, FileAccess.Write, FileShare.None)
                Request.FileToSign.Seek(0, SeekOrigin.Begin)
                Request.FileToSign.CopyTo(fs)
            End Using

            ' arm the watcher before the request leaves staging, so a fast response cannot be missed
            Using pdfWatcher As New FileSystemWatcher With {
                .Path = _settings.MQFolderIn,
                .Filter = pdfFileNameWithExtension,
                .EnableRaisingEvents = True,
                .IncludeSubdirectories = False
            }
                fileOp.MoveWhenAvailable(pdfOutStagingPath, pdfOutPath)

                _sbSignLog.AppendLine("Várakozás MTRACK válaszra")

                If File.Exists(pdfInPath) Then
                    responseArrived = True
                Else
                    responseArrived = Not pdfWatcher.WaitForChanged(WatcherChangeTypes.Changed Or WatcherChangeTypes.Created, timeOut).TimedOut

                    ' a response landing between the existence check and WaitForChanged subscribing
                    ' raises no event; re-check before declaring a timeout
                    If Not responseArrived Then responseArrived = File.Exists(pdfInPath)
                End If
            End Using

            If responseArrived Then
                _sbSignLog.AppendLine("MTRACK válasz beérkezett, válasz beolvasása")

                ' open with FileShare.Delete so the exchange file can be deleted while we still hold the stream
                signedDoc = fileOp.ReadFileStreamWhenAvailable(pdfInPath, FileShare.Read Or FileShare.Delete)

                If signedDoc IsNot Nothing Then
                    _sbSignLog.AppendLine("MTRACK válasz beolvasva")
                    fileOp.DeleteWhenAvailable(pdfInPath)
                Else
                    res.ErrorMessage = "Időtúllépés: az MTRACK felől beérkezett hitelesített dokumentumot nem sikerült beolvasni a fájlrendszerből a határidőn (10 mp) belül, mert egy másik folyamat lock-olta!"
                    res.SignatureLog = _sbSignLog.ToString
                    Return res
                End If
            End If
        End Using

        If Not responseArrived Then
            res.ErrorMessage = $"Időtúllépés: az MTRACK felől nem érkezett hitelesített dokumentum a határidőn ({timeOut / 1000} mp) belül!"
            res.SignatureLog = _sbSignLog.ToString
            Return res
        End If

        res.SignatureLog = _sbSignLog.ToString
        If String.IsNullOrEmpty(res.ErrorMessage) Then
            res.SignedFile = signedDoc
        End If

        Return res
    End Function

    Private Function ImpersonateIdentity(DomainName As String, UserName As String, Password As String) As WindowsImpersonationContext
        Dim userToken = IntPtr.Zero

        ' NEW_CREDENTIALS: local identity stays the same; the supplied credentials are used for
        ' outbound network access (SMB share) only. PROVIDER_WINNT50 is mandatory for this logon type.
        ' Note: the password is NOT validated at logon time - bad credentials surface later as
        ' access/existence failures on the exchange folders, not as a logon error here.
        Dim success = NativeMethods.LogonUser(UserName, DomainName, Password, CInt(NativeMethods.LogonType.LOGON32_LOGON_NEW_CREDENTIALS), CInt(NativeMethods.LogonProvider.LOGON32_PROVIDER_WINNT50), userToken)

        If Not success Then
            Throw New SecurityException($"nem sikerült bejelentkezni '{UserName}' felhasználóval az MQFTP cserekönyvtárak eléréséhez!")
        End If

        Try
            Return WindowsIdentity.Impersonate(userToken)
        Finally
            NativeMethods.CloseHandle(userToken)
        End Try
    End Function

    Private Function CalculateTimeOut(FileLength As Integer) As Integer
        Dim minimumTimeOut As Integer = 30000
        Dim addOneSecPerKB As Integer = 50 ' add 1 second per 50 KB
        Dim roundTo As Integer = 1000 ' rounds to the nearest thousand
        Dim res = Math.Round((minimumTimeOut + FileLength / addOneSecPerKB) / roundTo, 0) * 1000
        Return res
    End Function

End Class
