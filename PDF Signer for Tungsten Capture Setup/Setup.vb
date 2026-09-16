Imports System.Xml.Linq
Imports PDFSignerSetup.My.Resources
Imports System.Linq
Imports PDFSignerCommon
Imports Kofax.Capture.AdminModule.InteropServices

Public Class Setup
    Inherits SetupModel

    Private ReadOnly _docClass As DocumentClass

    Public Sub New(DocClass As DocumentClass)
        _docClass = DocClass
        SignatureMarkerPosition = 1
        FileOverwriteOriginal = True
        FileCreateNew = False
        DocumentViewer = DocumentViewerType.AdvancedViewer
        DefaultCryptographicProvider = CryptoProviderType.PDFSigner
    End Sub

    Friend Function Validate() As String
        If Not IsSigningEnabled Then Return String.Empty

        If Not String.IsNullOrEmpty(SignatureMarker) Then SignatureMarker = SignatureMarker.Trim
        If Not String.IsNullOrEmpty(DocumentNameDefault) Then DocumentNameDefault = DocumentNameDefault.Trim
        If Not String.IsNullOrEmpty(ConvertingOrganization) Then ConvertingOrganization = ConvertingOrganization.Trim
        If Not String.IsNullOrEmpty(ConvertingRegulationName) Then ConvertingRegulationName = ConvertingRegulationName.Trim
        If Not String.IsNullOrEmpty(ConvertingRegulationVersion) Then ConvertingRegulationVersion = ConvertingRegulationVersion.Trim
        If Not String.IsNullOrEmpty(DocumentNameDefault) Then DocumentNameDefault = DocumentNameDefault.Trim
        If Not String.IsNullOrEmpty(FileNameAppend) Then FileNameAppend = FileNameAppend.Trim
        If Not String.IsNullOrEmpty(FileExtensionReplace) Then FileExtensionReplace = FileExtensionReplace.Trim

        If Not SignAllDocuments Then
            If String.IsNullOrEmpty(IndexBarCode) Then
                Return Messages.Index_Bar_Code_Missing
            End If
            If String.IsNullOrEmpty(SignatureMarker) Then
                Return Messages.Signature_Marker_Missing
            End If
        End If

        If AllowSkipRequiredDocument AndAlso String.IsNullOrEmpty(IndexSkipNote) Then
            Return Messages.Index_Skip_Note_Missing
        End If

        If String.IsNullOrEmpty(ConvertingOrganization) Then
            Return Messages.Copying_Organization_Missing
        End If
        If String.IsNullOrEmpty(ConvertingRegulationName) Then
            Return Messages.Copying_Regulation_Name_Missing
        End If
        If String.IsNullOrEmpty(ConvertingRegulationURL?.ToString) Then
            Return Messages.Copying_Regulation_URL_Missing
        End If
        If String.IsNullOrEmpty(ConvertingRegulationVersion) Then
            Return Messages.Copying_Regulation_Version_Missing
        End If
        If String.IsNullOrEmpty(MetadataFormat) Then
            Return Messages.Clause_Format_Missing
        End If
        If Not String.IsNullOrEmpty(SignaturePolicyHash) Then
            Dim v As New Validator
            If Not v.IsBase64String(SignaturePolicyHash) Then
                Return Messages.Signature_Policy_Hash_Error
            End If
        End If

        If Not FileOverwriteOriginal AndAlso Not FileCreateNew Then
            Return Messages.File_Naming_Not_Selected
        End If
        If FileCreateNew Then
            Dim v As New Validator

            If String.IsNullOrEmpty(FileNameAppend) AndAlso String.IsNullOrEmpty(FileExtensionReplace) Then
                Return Messages.File_New_Parameters_Not_Set
            End If
            If Not v.IsValidFileString(FileNameAppend) Then
                Return Messages.File_Append_Text_Invalid
            End If
            If Not v.IsValidFileString(FileExtensionReplace) Then
                Return Messages.File_Replacement_Extension_Invalid
            End If
        End If

        If String.IsNullOrEmpty(DocumentViewer) Then
            Return Messages.Document_Viewer_Not_Selected
        End If

        If String.IsNullOrEmpty(DefaultCryptographicProvider) Then
            Return Messages.Crypto_Provider_Not_Selected
        End If

        If Not CryptographicProviders.Any(Function(x) x.Enabled) Then
            Return Messages.No_Crypto_Provider_Enabled
        End If

        If Not CryptographicProviders.Any(Function(x) x.ProviderType = DefaultCryptographicProvider AndAlso x.Enabled) Then
            Return Messages.Default_Crypto_Provider_Not_Enabled
        End If

        For Each provider As CryptoProviderBase In CryptographicProviders
            Dim errMsg As String = provider.Validate()

            If Not String.IsNullOrEmpty(errMsg) Then
                Return errMsg
            End If
        Next

        Return String.Empty
    End Function

    Friend Sub LoadValues()
        Dim parser As New SetupCSSParser(_docClass)

        With parser
            IsSigningEnabled = Converter.StringToBoolean(.ReadSetupCSS(CSS.IsEnabled))

            IndexIsSigned = .ReadSetupCSS(CSS.IndexIsSigned)
            IndexSignedDateTime = .ReadSetupCSS(CSS.IndexSignedDateTime)
            IndexSignedBy = .ReadSetupCSS(CSS.IndexSignedBy)
            IndexSignatureValidUntil = .ReadSetupCSS(CSS.IndexSignatureValidUntil)
            IndexSkipNote = .ReadSetupCSS(CSS.IndexSkipNote)

            SignAllDocuments = Converter.StringToBoolean(.ReadSetupCSS(CSS.SignAllDocuments))
            IndexBarCode = .ReadSetupCSS(CSS.IndexBarCode)
            SignatureMarker = .ReadSetupCSS(CSS.SignatureMarker)
            SignatureMarkerPosition = Converter.StringToInteger(.ReadSetupCSS(CSS.SignatureMarkerPosition))
            AllowSkipRequiredDocument = Converter.StringToBoolean(.ReadSetupCSS(CSS.AllowSkipRequiredDocument))

            IndexDocumentName = .ReadSetupCSS(CSS.IndexDocumentName)
            DocumentNameFromFormType = Converter.StringToBoolean(.ReadSetupCSS(CSS.DocumentNameFromFormType))
            DocumentNameDefault = .ReadSetupCSS(CSS.DocumentNameDefault)
            ConvertingOrganization = .ReadSetupCSS(CSS.ConvertingOrganization)
            ConvertingRegulationName = .ReadSetupCSS(CSS.ConvertingRegulationName)
            ConvertingRegulationURL = Converter.StringToUri(.ReadSetupCSS(CSS.ConvertingRegulationURL))
            ConvertingRegulationVersion = .ReadSetupCSS(CSS.ConvertingRegulationVersion)
            MetadataFormat = .ReadSetupCSSInteger(CSS.MetaDataFormat, 0)
            SignaturePolicyURL = Converter.StringToUri(.ReadSetupCSS(CSS.SignaturePolicyURL))
            SignaturePolicyHash = .ReadSetupCSS(CSS.SignaturePolicyHash)
            SignaturePolicyOID = .ReadSetupCSS(CSS.SignaturePolicyOID)

            FileOverwriteOriginal = Converter.StringToBoolean(.ReadSetupCSS(CSS.FileOverwriteOriginal))
            FileCreateNew = Converter.StringToBoolean(.ReadSetupCSS(CSS.FileCreateNew))
            FileNameAppend = .ReadSetupCSS(CSS.FileNameAppend)
            FileExtensionReplace = .ReadSetupCSS(CSS.FileExtensionReplace)

            DocumentViewer = .ReadSetupCSSInteger(CSS.DocumentViewer, 0)
            DefaultCryptographicProvider = .ReadSetupCSSInteger(CSS.DefaultCryptographicProvider, 0)

            ' Load values into cryptographic providers
            For Each p As CryptoProviderBase In CryptographicProviders
                p.LoadSetupDataInAdmin(parser)
            Next
        End With
    End Sub

    Friend Sub SaveValues()
        Dim parser As New SetupCSSParser(_docClass)
        Dim enc As New Encrypt

        With parser
            .WriteSetupCSS(CSS.IsEnabled, Converter.BooleanToNumericString(IsSigningEnabled))

            .WriteSetupCSS(CSS.IndexIsSigned, IndexIsSigned)
            .WriteSetupCSS(CSS.IndexSignedDateTime, IndexSignedDateTime)
            .WriteSetupCSS(CSS.IndexSignedBy, IndexSignedBy)
            .WriteSetupCSS(CSS.IndexSignatureValidUntil, IndexSignatureValidUntil)
            .WriteSetupCSS(CSS.IndexSkipNote, IndexSkipNote)

            .WriteSetupCSS(CSS.SignAllDocuments, Converter.BooleanToNumericString(SignAllDocuments))
            .WriteSetupCSS(CSS.IndexBarCode, IndexBarCode)
            .WriteSetupCSS(CSS.SignatureMarker, SignatureMarker)
            .WriteSetupCSS(CSS.SignatureMarkerPosition, SignatureMarkerPosition.ToString)
            .WriteSetupCSS(CSS.AllowSkipRequiredDocument, Converter.BooleanToNumericString(AllowSkipRequiredDocument))

            .WriteSetupCSS(CSS.IndexDocumentName, IndexDocumentName)
            .WriteSetupCSS(CSS.DocumentNameFromFormType, Converter.BooleanToNumericString(DocumentNameFromFormType))
            .WriteSetupCSS(CSS.DocumentNameDefault, DocumentNameDefault)
            .WriteSetupCSS(CSS.ConvertingOrganization, ConvertingOrganization)
            .WriteSetupCSS(CSS.ConvertingRegulationName, ConvertingRegulationName)
            If ConvertingRegulationURL Is Nothing Then
                .WriteSetupCSS(CSS.ConvertingRegulationURL, String.Empty)
            Else
                .WriteSetupCSS(CSS.ConvertingRegulationURL, ConvertingRegulationURL.ToString)
            End If
            .WriteSetupCSS(CSS.ConvertingRegulationVersion, ConvertingRegulationVersion)
            .WriteSetupCSS(CSS.MetaDataFormat, MetadataFormat)
            If SignaturePolicyURL Is Nothing Then
                .WriteSetupCSS(CSS.SignaturePolicyURL, String.Empty)
            Else
                .WriteSetupCSS(CSS.SignaturePolicyURL, SignaturePolicyURL.ToString)
            End If
            .WriteSetupCSS(CSS.SignaturePolicyHash, SignaturePolicyHash)
            .WriteSetupCSS(CSS.SignaturePolicyOID, SignaturePolicyOID)

            .WriteSetupCSS(CSS.FileOverwriteOriginal, Converter.BooleanToNumericString(FileOverwriteOriginal))
            .WriteSetupCSS(CSS.FileCreateNew, Converter.BooleanToNumericString(FileCreateNew))
            .WriteSetupCSS(CSS.FileNameAppend, FileNameAppend)
            .WriteSetupCSS(CSS.FileExtensionReplace, FileExtensionReplace)

            .WriteSetupCSS(CSS.DocumentViewer, DocumentViewer)
            .WriteSetupCSS(CSS.DefaultCryptographicProvider, DefaultCryptographicProvider)

            ' Save values using cryptographic providers
            For Each p As CryptoProviderBase In CryptographicProviders
                p.SaveSetupDataInAdmin(parser)
            Next
        End With
    End Sub

    Friend Function ToXml() As String
        Return BuildXml(EncryptSecrets:=True).ToString
    End Function

    ''' <summary>
    ''' Returns all setting values as normalized XML for detecting unsaved edits.
    ''' Secrets are in plain text, so the result must never be persisted or exported.
    ''' </summary>
    Friend Function GetValueSnapshot() As String
        Dim dom = BuildXml(EncryptSecrets:=False)

        ' Nothing and empty text both mean "not set": cleared controls write either one.
        For Each element In dom.Descendants.Where(Function(x) x.IsEmpty).ToList
            element.Value = String.Empty
        Next

        Return dom.ToString
    End Function

    Private Function BuildXml(EncryptSecrets As Boolean) As XElement
        Dim dom As New XElement("PDFSignerConfig")
        dom.Add(New XElement("IsSigningEnabled", Converter.BooleanToNumericString(IsSigningEnabled)))
        dom.Add(New XElement("IndexIsSigned", IndexIsSigned))
        dom.Add(New XElement("IndexSignedDateTime", IndexSignedDateTime))
        dom.Add(New XElement("IndexSignedBy", IndexSignedBy))
        dom.Add(New XElement("IndexSignatureValidUntil", IndexSignatureValidUntil))
        dom.Add(New XElement("IndexSkipNote", IndexSkipNote))
        dom.Add(New XElement("SignAllDocuments", Converter.BooleanToNumericString(SignAllDocuments)))
        dom.Add(New XElement("IndexBarCode", IndexBarCode))
        dom.Add(New XElement("SignatureMarker", SignatureMarker))
        dom.Add(New XElement("SignatureMarkerPosition", SignatureMarkerPosition.ToString))
        dom.Add(New XElement("AllowSkipRequiredDocument", Converter.BooleanToNumericString(AllowSkipRequiredDocument)))
        dom.Add(New XElement("IndexDocumentName", IndexDocumentName))
        dom.Add(New XElement("DocumentNameFromFormType", Converter.BooleanToNumericString(DocumentNameFromFormType)))
        dom.Add(New XElement("DocumentNameDefault", DocumentNameDefault))
        dom.Add(New XElement("ConvertingOrganization", ConvertingOrganization))
        dom.Add(New XElement("ConvertingRegulationName", ConvertingRegulationName))
        dom.Add(New XElement("ConvertingRegulationURL", ConvertingRegulationURL))
        dom.Add(New XElement("ConvertingRegulationVersion", ConvertingRegulationVersion))
        dom.Add(New XElement("MetaDataFormat", MetadataFormat))
        dom.Add(New XElement("SignaturePolicyURL", SignaturePolicyURL))
        dom.Add(New XElement("SignaturePolicyHash", SignaturePolicyHash))
        dom.Add(New XElement("SignaturePolicyOID", SignaturePolicyOID))
        dom.Add(New XElement("FileOverwriteOriginal", Converter.BooleanToNumericString(FileOverwriteOriginal)))
        dom.Add(New XElement("FileCreateNew", Converter.BooleanToNumericString(FileCreateNew)))
        dom.Add(New XElement("FileNameAppend", FileNameAppend))
        dom.Add(New XElement("FileExtensionReplace", FileExtensionReplace))
        dom.Add(New XElement("DocumentViewer", DocumentViewer))
        dom.Add(New XElement("DefaultCryptographicProvider", DefaultCryptographicProvider))

        ' Create nodes using cryptographic providers
        For Each p As CryptoProviderBase In CryptographicProviders
            dom.Add(p.SetupDataToXml(EncryptSecrets))
        Next

        Return dom
    End Function

    Friend Sub FromXml(XmlText As String)
        Dim enc As New Encrypt

        Try
            Dim XmlDoc = XDocument.Parse(XmlText)
            Dim ConfigElement = XmlDoc.Element("PDFSignerConfig")

            IsSigningEnabled = Converter.StringToBoolean(ConfigElement.Element("IsSigningEnabled"))

            IndexIsSigned = ConfigElement.Element("IndexIsSigned")
            IndexSignedDateTime = ConfigElement.Element("IndexSignedDateTime")
            IndexSignedBy = ConfigElement.Element("IndexSignedBy")
            IndexSignatureValidUntil = ConfigElement.Element("IndexSignatureValidUntil")
            IndexSkipNote = ConfigElement.Element("IndexSkipNote")

            SignAllDocuments = Converter.StringToBoolean(ConfigElement.Element("SignAllDocuments"))
            IndexBarCode = ConfigElement.Element("IndexBarCode")
            SignatureMarker = ConfigElement.Element("SignatureMarker")
            SignatureMarkerPosition = ConfigElement.Element("SignatureMarkerPosition")
            AllowSkipRequiredDocument = Converter.StringToBoolean(ConfigElement.Element("AllowSkipRequiredDocument"))

            IndexDocumentName = ConfigElement.Element("IndexDocumentName")
            DocumentNameFromFormType = Converter.StringToBoolean(ConfigElement.Element("DocumentNameFromFormType"))
            DocumentNameDefault = ConfigElement.Element("DocumentNameDefault")
            ConvertingOrganization = ConfigElement.Element("ConvertingOrganization")
            ConvertingRegulationName = ConfigElement.Element("ConvertingRegulationName")
            ConvertingRegulationURL = Converter.StringToUri(ConfigElement.Element("ConvertingRegulationURL"))
            ConvertingRegulationVersion = ConfigElement.Element("ConvertingRegulationVersion")
            MetadataFormat = CInt(ConfigElement.Element("MetaDataFormat"))
            SignaturePolicyURL = Converter.StringToUri(ConfigElement.Element("SignaturePolicyURL"))
            SignaturePolicyHash = ConfigElement.Element("SignaturePolicyHash")
            SignaturePolicyOID = ConfigElement.Element("SignaturePolicyOID")

            FileOverwriteOriginal = Converter.StringToBoolean(ConfigElement.Element("FileOverwriteOriginal"))
            FileCreateNew = Converter.StringToBoolean(ConfigElement.Element("FileCreateNew"))
            FileNameAppend = ConfigElement.Element("FileNameAppend")
            FileExtensionReplace = ConfigElement.Element("FileExtensionReplace")

            DocumentViewer = ConfigElement.Element("DocumentViewer")
            DefaultCryptographicProvider = ConfigElement.Element("DefaultCryptographicProvider")

            ' Load XML using cryptographic providers
            For Each p As CryptoProviderBase In CryptographicProviders
                p.SetupDataFromXml(ConfigElement.Element(p.ConfigXmlElementName))
            Next

        Catch ex As Exception
            Throw New ArgumentException(Messages.XML_Schema_Error)
        End Try
    End Sub

End Class
