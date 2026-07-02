Imports Kofax.Capture.SDK.Data

Public Module SetupParser
    Dim _SetupDict As Dictionary(Of String, SetupModel)

    Public Sub ResetSetupData()
        _SetupDict = New Dictionary(Of String, SetupModel)
    End Sub

    Public Function GetSetupDataAtRuntime(SetupDocumentClassElement As IACDataElement, ExtractMode As SetupExtractMode) As SetupModel
        Dim name As String = SetupDocumentClassElement("Name")

        If _SetupDict.ContainsKey(name) Then
            Return _SetupDict(name)
        Else
            Dim s As SetupModel = Parse(SetupDocumentClassElement, ExtractMode)
            _SetupDict.Add(name, s)
            Return s
        End If
    End Function

    Private Function GetSetupCSSValue(SetupDocumentClassElement As IACDataElement, SetupCssName As String) As String
        Dim SetupCSSsElement As IACDataElement = SetupDocumentClassElement.FindChildElementByName("DocumentClassCustomStorageStrings")
        Dim SetupCSS As IACDataElement = SetupCSSsElement.FindChildElementByAttribute("DocumentClassCustomStorageString", "Name", SetupCssName)

        If SetupCSS IsNot Nothing Then
            Return SetupCSS("Value")
        Else
            Return String.Empty
        End If
    End Function

    Private Function Parse(SetupDocumentClassElement As IACDataElement, ExtractMode As SetupExtractMode) As SetupModel
        Dim res As New SetupModel
        Dim enc As New Encrypt

        With res
            .IsSigningEnabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsEnabled))

            .SignAllDocuments = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignAllDocuments))
            .IndexBarCode = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexBarCode)
            .SignatureMarker = GetSetupCSSValue(SetupDocumentClassElement, CSS.SignatureMarker)
            .SignatureMarkerPosition = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignatureMarkerPosition))

            If ExtractMode = SetupExtractMode.FullExtract Then
                .IndexIsSigned = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexIsSigned)
                .IndexSignedDateTime = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexSignedDateTime)
                .IndexSignedBy = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexSignedBy)
                .IndexSignatureValidUntil = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexSignatureValidUntil)
                .IndexSkipNote = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexSkipNote)

                .AllowSkipRequiredDocument = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.AllowSkipRequiredDocument))

                .IndexDocumentName = GetSetupCSSValue(SetupDocumentClassElement, CSS.IndexDocumentName)
                .DocumentNameFromFormType = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.DocumentNameFromFormType))
                .DocumentNameDefault = GetSetupCSSValue(SetupDocumentClassElement, CSS.DocumentNameDefault)
                .ConvertingOrganization = GetSetupCSSValue(SetupDocumentClassElement, CSS.ConvertingOrganization)
                .ConvertingRegulationName = GetSetupCSSValue(SetupDocumentClassElement, CSS.ConvertingRegulationName)
                .ConvertingRegulationURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.ConvertingRegulationURL))
                .ConvertingRegulationVersion = GetSetupCSSValue(SetupDocumentClassElement, CSS.ConvertingRegulationVersion)
                .MetadataFormat = GetSetupCSSValue(SetupDocumentClassElement, CSS.MetaDataFormat)
                .SignaturePolicyURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignaturePolicyURL))
                .SignaturePolicyHash = GetSetupCSSValue(SetupDocumentClassElement, CSS.SignaturePolicyHash)
                .SignaturePolicyOID = GetSetupCSSValue(SetupDocumentClassElement, CSS.SignaturePolicyOID)

                .FileOverwriteOriginal = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.FileOverwriteOriginal))
                .FileCreateNew = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.FileCreateNew))
                .FileNameAppend = GetSetupCSSValue(SetupDocumentClassElement, CSS.FileNameAppend)
                .FileExtensionReplace = GetSetupCSSValue(SetupDocumentClassElement, CSS.FileExtensionReplace)

                .DocumentViewer = GetSetupCSSValue(SetupDocumentClassElement, CSS.DocumentViewer)
                .DefaultCryptographicProvider = GetSetupCSSValue(SetupDocumentClassElement, CSS.DefaultCryptographicProvider)

                ' PDF Signer values
                .PDFSignerProvider.Enabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsPDFSignerEnabled))
                .PDFSignerProvider.SigningOrganization = GetSetupCSSValue(SetupDocumentClassElement, CSS.SigningOrganization)
                .PDFSignerProvider.SigningReason = GetSetupCSSValue(SetupDocumentClassElement, CSS.SigningReason)
                .PDFSignerProvider.RevocationCheck = GetSetupCSSValue(SetupDocumentClassElement, CSS.RevocationCheck)
                .PDFSignerProvider.SignatureHashMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.SignatureHashMethod)
                .PDFSignerProvider.AllowQualifiedCertificatesOnly = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.AllowQualifiedCertificatesOnly))
                .PDFSignerProvider.SignerNameFromLoggedOnUser = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.SignerNameFromLoggedOnUser))
                .PDFSignerProvider.IsTimeStampingEnabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsTimeStampingEnabled))
                .PDFSignerProvider.TSAURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAURL))
                .PDFSignerProvider.TSAUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAUserName)
                .PDFSignerProvider.TSAPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.TSAPassword))
                .PDFSignerProvider.TimeStampHashMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.TimeStampHashMethod)
                .PDFSignerProvider.IsDocumentTimeStamp = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsDocumentTimeStamp))
                .PDFSignerProvider.IsSinglePassPadesBLTA = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsSinglePassPadesBLTA))
                .PDFSignerProvider.IsProxyEnabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsProxyEnabled))
                .PDFSignerProvider.ProxyServer = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyServer)
                .PDFSignerProvider.ProxyPort = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyPort))
                .PDFSignerProvider.ProxyAuthMethod = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyAuthMethod)
                .PDFSignerProvider.ProxyUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyUserName)
                .PDFSignerProvider.ProxyPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.ProxyPassword))

                ' PDF Streamer values
                .PDFStreamerProvider.Enabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsPDFStreamerEnabled))
                .PDFStreamerProvider.PDFStreamerURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.PDFStreamerURL))
                .PDFStreamerProvider.PDFStreamerConfigFile = GetSetupCSSValue(SetupDocumentClassElement, CSS.PDFStreamerConfigFile)
                .PDFStreamerProvider.PDFStreamerAuthorizationCode = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.PDFStreamerAuthorizationCode))

                ' MQFTP values
                .MQFTPProvider.Enabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsMQEnabled))
                .MQFTPProvider.MQFolderOut = GetSetupCSSValue(SetupDocumentClassElement, CSS.MQFolderOut)
                .MQFTPProvider.MQFolderIn = GetSetupCSSValue(SetupDocumentClassElement, CSS.MQFolderIn)
                .MQFTPProvider.MQDomain = GetSetupCSSValue(SetupDocumentClassElement, CSS.MQDomain)
                .MQFTPProvider.MQUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.MQUserName)
                .MQFTPProvider.MQPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.MQPassword))
                .MQFTPProvider.IndexMQDocUID = GetSetupCSSValue(SetupDocumentClassElement, CSS.MQDocUID)

                ' MNB Signer values
                .MNBSignerProvider.Enabled = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsMNBSignerEnabled))
                .MNBSignerProvider.MNBSignerURL = Converter.StringToUri(GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerURL))
                .MNBSignerProvider.MNBSignerWSTimeout = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerWSTimeout))
                .MNBSignerProvider.MNBSignerSigningTimeout = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerSigningTimeout))
                .MNBSignerProvider.MNBSignerChunkSize = Converter.StringToInteger(GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerChunkSize))
                .MNBSignerProvider.IsMNBSignerWindowsAuthentication = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsMNBSignerWindowsAuthentication))
                .MNBSignerProvider.IsMNBSignerUserPasswordAuthentication = Converter.StringToBoolean(GetSetupCSSValue(SetupDocumentClassElement, CSS.IsMNBSignerUserPasswordAuthentication))
                .MNBSignerProvider.MNBSignerDomain = GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerDomain)
                .MNBSignerProvider.MNBSignerUserName = GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerUserName)
                .MNBSignerProvider.MNBSignerPassword = enc.AES256Decrypt(GetSetupCSSValue(SetupDocumentClassElement, CSS.MNBSignerPassword))
            End If
        End With

        Return res
    End Function
End Module
