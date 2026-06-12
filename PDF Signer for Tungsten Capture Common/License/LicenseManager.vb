Imports System.IO
Imports PDFSignerCommon.My.Resources
Imports System.Xml
Imports System.Globalization

Public Class LicenseManager
    ''' <summary>
    ''' Get LicenseInfo object from XML license file
    ''' </summary>
    ''' <returns>Model object</returns>
    Public Function LoadFromFile() As LicenseInfo
        Const LicenseFileName As String = "PDFSigner.license"
        Dim lic = New LicenseInfo
        Dim enc = New Encrypt
        Dim dom As XDocument

        Try
            Dim LicPathAndFileName As String = String.Empty
            Dim decryptedLic As String

            ' Search for license file next to the executable
            Dim localLicPathAndFileName As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicenseFileName)
            If File.Exists(localLicPathAndFileName) Then
                LicPathAndFileName = localLicPathAndFileName
            End If

            ' If not found then look on the server share
            If String.IsNullOrEmpty(LicPathAndFileName) Then
                LicPathAndFileName = Path.Combine(KofaxRegistry.KofaxPDFSignerLicenseFolder, LicenseFileName)
            End If

            ' Load and validate contents if file found
            If Not String.IsNullOrEmpty(LicPathAndFileName) AndAlso File.Exists(LicPathAndFileName) Then

                ' decrypt the license file
                decryptedLic = enc.AES256Decrypt(File.ReadAllText(LicPathAndFileName))
                If String.IsNullOrEmpty(decryptedLic) Then
                    lic.LicenseError = CommonRes.LicenseDecryptError
                    Return lic
                End If

                dom = XDocument.Parse(decryptedLic)

                Dim vm = New ValidationMethods()
                Dim XmlSchemaError = vm.ValidateXMLWithXSD(dom, XmlSchemas.PDFSignerLicense)
                If Not String.IsNullOrEmpty(XmlSchemaError) Then
                    lic.LicenseError = String.Format(CommonRes.XMLSchemaError, LicPathAndFileName, XmlSchemaError)
                    Return lic
                End If
            Else ' Error if not found
                lic.LicenseError = String.Format(CommonRes.LicenseFileNotFound, LicenseFileName, LicPathAndFileName)
                Return lic
            End If

        Catch ex As UnauthorizedAccessException
            lic.LicenseError = String.Format(CommonRes.LicenseFileCannotBeRead + vbCrLf + vbCrLf + ex.ToString, LicenseFileName)
            Return lic

        Catch ex As XmlException
            lic.LicenseError = String.Format(CommonRes.LicenseFileNotWellFormed + vbCrLf + vbCrLf + ex.ToString, LicenseFileName)
            Return lic

        End Try

        Dim conf As XElement = dom.Root.Element("Licenses")

        lic.Owner = conf.Element("Owner").Value
        lic.MainVersion = CInt(conf.Element("MainVersion"))
        lic.KofaxSerial = conf.Element("KofaxSerial").Value
        lic.AsyncOperation = CBool(conf.Element("AsyncOperation"))
        lic.ValidUntil = Date.ParseExact(conf.Element("ValidUntil").Value, "yyyy-MM-dd", CultureInfo.InvariantCulture)

        Dim sigNode As String = dom.Root.Element("Signature").Value
        Dim licNode = dom.Root.Element("Licenses").ToString
        Dim isSigValid As Boolean = enc.RSAVerifySignedString(licNode, sigNode)

        If Not isSigValid Then
            lic.LicenseError = String.Format(CommonRes.LicenseSignatureError, LicenseFileName)
            Return lic
        End If

        Dim assemblyVer = Reflection.Assembly.GetEntryAssembly().GetName().Version
        If lic.MainVersion <> assemblyVer.Major Then
            lic.LicenseError = String.Format(CommonRes.LicenseVersionMismatch, LicenseFileName)
            Return lic
        End If

        Dim kfxSerial = GetKofaxSerial()
        If kfxSerial <> lic.KofaxSerial Then
            lic.LicenseError = String.Format(CommonRes.LicenseKofaxSerialMismatch, LicenseFileName)
            Return lic
        End If

        If lic.ValidUntil < Date.Now.Date Then
            lic.LicenseError = String.Format(CommonRes.LicenseExpired, LicenseFileName, lic.ValidUntil.ToShortDateString)
            Return lic
        End If

        Return lic
    End Function

    Private Function GetKofaxSerial() As String
        Dim ACLicense As New Kofax.ASBU.ACLicClnt.LicClientConnection
        Dim KofaxSerial As String = ACLicense.GetDisplayAscentSerialNumber
        Return KofaxSerial
    End Function

End Class
