Imports Microsoft.Win32
Imports System.IO
Imports PDFSignerCommon.My.Resources

Public Module HelpProvider
    Public Sub ShowHelp(CSHIDString As String)
        Try
            Dim browserPath = GetBrowserPath()

            If String.IsNullOrEmpty(browserPath) Then
                Throw New ArgumentException(CommonRes.Browser_Path_Cannot_Be_Determined)
            End If

            Dim lang As String = Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower()
            Dim AppPath As String = Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly.Location)

            ' check if help exists for language... if not, set language to default "hu"
            If Not Directory.Exists(AppPath + "\" + lang + "\help\") Then
                lang = "hu"
            End If

            ' Launch the browser with help file as parameter
            Dim helpPath As String = (Convert.ToString("""" + "file:///" + AppPath + "\" + lang + "\help\index.htm#cshid=") & CSHIDString) + """"
            Process.Start(browserPath, helpPath)
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ShowURI(URIToShow As Uri)
        Dim browserPath = GetBrowserPath()

        If String.IsNullOrEmpty(browserPath) Then
            Throw New ArgumentException(CommonRes.Browser_Path_Cannot_Be_Determined)
        End If

        ' Launch the browser with URI as parameter
        Process.Start(browserPath, URIToShow.ToString)
    End Sub

    Private Function GetRegisteredApplication(extension As String) As String
        Dim extensionId As String = GetClassesRootKeyDefaultValue(extension)
        If extensionId Is Nothing Then
            Return String.Empty
        End If

        Dim openCommand As String = GetClassesRootKeyDefaultValue(Path.Combine(extensionId, "shell\open\command"))

        If openCommand Is Nothing Then
            Return String.Empty
        End If

        Return openCommand
    End Function

    Private Function GetClassesRootKeyDefaultValue(keyPath As String) As String
        Using key = Registry.ClassesRoot.OpenSubKey(keyPath)
            If key Is Nothing Then
                Return Nothing
            End If

            Dim defaultValue = key.GetValue(Nothing)
            If defaultValue Is Nothing Then
                Return Nothing
            End If

            Return defaultValue.ToString()
        End Using
    End Function

    Private Function GetBrowserPath() As String
        ' Get default browser from the Registry
        ' System.Diagnostics.Process.Start(URL) nem jó, mert a #-ot tartalmazó URL-eket levágja
        Const userChoice As String = "Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"
        Const defaultHttpCommand As String = "Software\Classes\http\shell\open\command"
        Const exeSuffix As String = ".exe"
        Dim progId As String
        Dim browserPath As String = String.Empty
        Dim path As String = String.Empty

        Using userChoiceKey As RegistryKey = Registry.CurrentUser.OpenSubKey(userChoice)
            If userChoiceKey IsNot Nothing Then
                Dim progIdValue As Object = userChoiceKey.GetValue("Progid")

                If progIdValue IsNot Nothing Then
                    progId = progIdValue.ToString()
                    ' Get the executable full path of the browser
                    Dim RegistryPath As String = progId & Convert.ToString("\shell\open\command")

                    Using pathKey As RegistryKey = Registry.ClassesRoot.OpenSubKey(RegistryPath)
                        If pathKey IsNot Nothing Then
                            ' Trim parameters
                            path = pathKey.GetValue(Nothing).ToString().ToLower().Replace("""", String.Empty)
                        End If
                    End Using
                End If
            End If
        End Using

        If String.IsNullOrEmpty(path) Then
            Using defaultHttpCommandKey As RegistryKey = Registry.CurrentUser.OpenSubKey(defaultHttpCommand)
                If defaultHttpCommandKey IsNot Nothing Then
                    path = defaultHttpCommandKey.GetValue(Nothing).ToString().ToLower().Replace("""", String.Empty)
                End If
            End Using
        End If

        If String.IsNullOrEmpty(path) Then
            path = GetRegisteredApplication(".html")
        End If

        If Not String.IsNullOrEmpty(path) Then
            If Not path.EndsWith(exeSuffix) Then
                path = path.Substring(0, path.LastIndexOf(exeSuffix, StringComparison.Ordinal) + exeSuffix.Length)
                browserPath = path.Replace("""", String.Empty)
            End If
        End If

        Return browserPath

    End Function
End Module
