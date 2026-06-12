Imports System.IO
Imports Kofax.Capture.DBLite
Imports Kofax.Capture.SDK.Data
Imports PDFSignerCommon
Imports PDFSigner.My.Resources
Imports NLog

Friend Class BatchParser
    Private KfxImageFilePath As String
    Private KfxBatchID As String
    Private BatchPDFPath As String
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Friend Function GetBatchContents(Batch As Batch) As BatchContents
        Dim Identity As Integer = 1

        ' clear data caches
        SetupDocClassParser.ResetSetupDocClassData()
        SetupParser.ResetSetupData()

        'Access runtime information
        Dim RootElement As IACDataElement = Batch.ExtractRuntimeACDataElement(0)
        Dim BatchElement As IACDataElement = RootElement.FindChildElementByName("Batch")

        'Access setup information
        Dim SetupRootElement As IACDataElement = Batch.ExtractSetupACDataElement(0)

        'Find the ImageDirectory
        KfxImageFilePath = BatchElement("ImageDirectory")
        KfxBatchID = Hex(BatchElement("ExternalBatchID")).PadLeft(8, "0"c)
        BatchPDFPath = Path.Combine(KfxImageFilePath, KfxBatchID & ".PDF")

        Dim res As New BatchContents



        ' ********** add batch node **********
        Dim batchItem As New BatchItem With
            {
                .ID = Identity,
                .ParentID = 0,
                .DisplayName = BatchElement("Name"),
                .KofaxElement = BatchElement,
                .Contents = res.Content,
                .SetupRootElement = SetupRootElement
            }

        batchItem.IconType = GetBatchTreeItemIcon(batchItem)
        res.AddBatch(batchItem)

        Identity += 1



        ' ********** add document nodes **********
        Dim DocumentsElement As IACDataElement = BatchElement.FindChildElementByName("Documents")
        Dim DocumentsCollection As IACDataElementCollection = DocumentsElement.FindChildElementsByName("Document")
        Dim DocCounter As Integer = 1

        For Each doc As IACDataElement In DocumentsCollection
            If doc("Released") Then Continue For

            Dim documentItem As New DocumentItem

            With documentItem
                .ID = Identity
                .ParentID = 1
                .OrderNumber = DocCounter
                .KofaxElement = doc
                .FormTypeName = doc("FormTypeName")
                .DisplayName = $"{ .OrderNumber}: { .FormTypeName}"

                If String.IsNullOrWhiteSpace(.FormTypeName) Then
                    .IsSigningPossible = False
                    Dim docOp As New DocumentOperation
                    docOp.Reject(documentItem, "Típus nélküli dokumentum nem hitelesíthető!")
                Else
                    Dim sd As IACDataElement = SetupDocClassParser.GetSetupDocClassByFormTypeName(SetupRootElement, .FormTypeName)
                    If sd Is Nothing Then
                        _logger.Error("SetupData értéke null, FormTypeName='{0}', dokumentum sorszáma = {1}", .FormTypeName, .OrderNumber)
                        _logger.Error("SetupRootElement dok.osztályok: {0}", GetSetupDocClassList(SetupRootElement))
                    End If
                    .SetupData = SetupParser.GetSetupDataAtRuntime(sd, SetupExtractMode.FullExtract)

                    .IsSigningPossible = .SetupData.IsSigningEnabled
                    .IsPDFSkipFirstPage = IsPDFGeneratorSkippingFirstPage(doc, SetupRootElement)

                    If .IsSigningPossible Then
                        If Not .SetupData.SignAllDocuments And Not String.IsNullOrWhiteSpace(.SetupData.IndexBarCode) Then
                            .BarCode = GetIndexValue(doc, .SetupData.IndexBarCode)
                        End If

                        .DetachedSignatureFilePath = GetCSSValue(doc, CSS.DetachedSignatureFilePath)
                        .ClauseFilePath = GetCSSValue(doc, CSS.ClauseFilePath)
                        .PartialCopyData = GetCSSValue(doc, CSS.PartialCopyData)
                        .ClauseContent = GetCSSValue(doc, CSS.ClauseContent)
                        .SignatureLogContent = GetCSSValue(doc, CSS.SignatureLogContent)
                        .IsSignatureNeeded = IsDocumentToBeSigned(documentItem)
                    End If

                    .IsRejected = doc("Rejected")
                    .IsSigned = Converter.StringToBoolean(GetCSSValue(doc, CSS.IsDocumentSigned))
                    .IsSkipped = Converter.StringToBoolean(GetCSSValue(doc, CSS.IsDocumentSkipped))
                    .SkipNote = GetIndexValue(doc, .SetupData.IndexSkipNote)
                    .KofaxPDFFilePath = doc("PDFGenerationFileName")
                    .SignedFilePath = GetSignedDocumentPath(.SetupData, .KofaxPDFFilePath)
                    .UnsignedFilePath = GetUnsignedDocumentPath(.SetupData, .KofaxPDFFilePath)
                    .SignedFileLastTimeStamp = GetCSSDateValue(doc, CSS.SignedFileDateTime)
                    .UnsignedFileLastTimeStamp = GetCSSDateValue(doc, CSS.UnsignedFileDateTime)

                    If .SetupData.FileOverwriteOriginal Then
                        If (Not .IsSigned AndAlso .UnsignedFileLastTimeStamp.HasValue AndAlso File.Exists(.SignedFilePath) AndAlso .UnsignedFileLastTimeStamp.Value < File.GetLastWriteTime(.SignedFilePath)) OrElse
                            (.IsSigned AndAlso .SignedFileLastTimeStamp.HasValue AndAlso File.Exists(.SignedFilePath) AndAlso .SignedFileLastTimeStamp.Value < File.GetLastWriteTime(.SignedFilePath)) Then
                            Dim docOp As New DocumentOperation
                            docOp.PurgeSignatureAndSkipData(documentItem)

                            ' we create the unsigned file
                            If .IsSignatureNeeded AndAlso Not .IsSkipped Then ' Skipped docs need copying too
                                ' by moving the one returned from PDF Generator, if any
                                If File.Exists(.UnsignedFilePath) Then File.Delete(.UnsignedFilePath)
                                If File.Exists(.SignedFilePath) Then File.Move(.SignedFilePath, .UnsignedFilePath)
                            Else
                                ' by copying the one returned from PDF Generator, if any
                                If File.Exists(.SignedFilePath) Then File.Copy(.SignedFilePath, .UnsignedFilePath, True)
                            End If
                        End If
                    End If

                    If .SetupData.FileCreateNew Then
                        If .UnsignedFileLastTimeStamp.HasValue AndAlso .UnsignedFileLastTimeStamp.Value < File.GetLastWriteTime(.UnsignedFilePath) AndAlso File.Exists(documentItem.SignedFilePath) Then
                            Dim docOp As New DocumentOperation
                            docOp.PurgeSignatureAndSkipData(documentItem)

                            ' we simply delete the signed file, if exists 
                            If File.Exists(.SignedFilePath) Then File.Delete(.SignedFilePath)
                        End If
                    End If
                End If

                ' setting icon based on status
                .IconType = GetBatchTreeItemIcon(documentItem)
            End With


            res.AddDocumentToBatch(batchItem, documentItem)

            DocCounter += 1
            Identity += 1



            ' ********** add page nodes **********
            Dim PagesElement As IACDataElement = doc.FindChildElementByName("Pages")
            Dim PagesCollection As IACDataElementCollection = PagesElement.FindChildElementsByName("Page")
            Dim PageCounter As Integer = 1
            Dim IsFirstPage As Boolean = True

            For Each page As IACDataElement In PagesCollection
                If documentItem.IsPDFSkipFirstPage AndAlso IsFirstPage Then
                    IsFirstPage = False
                    Continue For
                End If

                Dim pageItem As New PageItem

                With pageItem
                    .ID = Identity
                    .ParentID = documentItem.ID
                    .OrderNumber = PageCounter
                    .KofaxElement = page
                    .IsRejected = page("Rejected")
                    .IconType = GetBatchTreeItemIcon(pageItem)
                    .DisplayName = String.Format(GUIText.Page_Display_Name, .OrderNumber, page("ImageID"))
                End With

                res.AddPageToDocument(documentItem, pageItem)

                PageCounter += 1
                Identity += 1
            Next
        Next

        Return res
    End Function

    Friend Function GetBatchTreeItemIcon(Item As TreeItem) As IconType
        If Item.GetType Is GetType(BatchItem) Then
            Return IconType.Batch
        End If

        If Item.GetType Is GetType(DocumentItem) Then
            Dim i As DocumentItem = CType(Item, DocumentItem)

            With i
                If .IsRejected Then
                    If .IsSignatureNeeded Then
                        Return IconType.RejectedDocumentToBeSigned
                    Else
                        Return IconType.RejectedDocument
                    End If

                    If Not .IsSigningPossible Then Return IconType.NotSignableRejectedDocument
                Else
                    If .IsSigned Then
                        Return IconType.SignedDocument
                    ElseIf .IsSkipped Then
                        Return IconType.SkippedDocument
                    Else
                        If .IsSignatureNeeded Then
                            Return IconType.DocumentToBeSigned
                        Else
                            Return IconType.Document
                        End If
                    End If

                    If Not .IsSigningPossible Then Return IconType.NotSignableDocument
                End If
            End With
        End If

        If Item.GetType Is GetType(PageItem) Then
            Dim i As PageItem = CType(Item, PageItem)

            If i.IsRejected Then
                Return IconType.RejectedPage
            Else
                Return IconType.Page
            End If
        End If

        ' unreachable: all TreeItem kinds are handled above; explicit value (enum zero) for clarity
        Return IconType.Batch
    End Function

    Private Function GetSignedDocumentPath(DocumentSetup As SetupModel, KofaxPDFFilePath As String) As String
        Dim p As String = String.Empty

        If DocumentSetup.FileOverwriteOriginal Then
            p = KofaxPDFFilePath
        Else
            Dim fn As String = Path.GetFileNameWithoutExtension(KofaxPDFFilePath)

            If Not String.IsNullOrEmpty(DocumentSetup.FileNameAppend) Then
                Dim ext As String = Path.GetExtension(KofaxPDFFilePath)
                p = Path.Combine(BatchPDFPath, fn & DocumentSetup.FileNameAppend & ext)
            End If
            If Not String.IsNullOrEmpty(DocumentSetup.FileExtensionReplace) Then
                p = Path.ChangeExtension(p, DocumentSetup.FileExtensionReplace)
            End If
        End If

        Return p
    End Function

    Private Function GetUnsignedDocumentPath(DocumentSetup As SetupModel, KofaxPDFFilePath As String) As String
        Dim p As String

        If DocumentSetup.FileOverwriteOriginal Then
            p = Path.Combine(BatchPDFPath, Path.GetFileNameWithoutExtension(KofaxPDFFilePath) & "_original" & Path.GetExtension(KofaxPDFFilePath))
        Else
            p = KofaxPDFFilePath
        End If

        Return p
    End Function

    Private Function IsDocumentToBeSigned(Document As DocumentItem) As Boolean
        If Document.SetupData.SignAllDocuments Then Return True
        If Document.SetupData.SignatureMarkerPosition = 0 Then Return False

        If Document.BarCode.Length >= Document.SetupData.SignatureMarkerPosition Then
            If Document.BarCode.Substring(Document.SetupData.SignatureMarkerPosition - 1, 1) = Document.SetupData.SignatureMarker Then
                Return True
            End If
        End If

        Return False
    End Function

    Function IsPDFGeneratorSkippingFirstPage(Document As IACDataElement, SetupRootElement As IACDataElement) As Boolean
        Dim SetupDocClassElement As IACDataElement = SetupDocClassParser.GetSetupDocClassByFormTypeName(SetupRootElement, Document("FormTypeName"))

        Dim IsFirstPageSkipped As Boolean = Converter.StringToBoolean(SetupDocClassElement("PDFSkipFirstPage"))
        Return IsFirstPageSkipped
    End Function

    Private Function GetSetupDocClassList(SetupRootElement As IACDataElement) As String
        Dim sb As New System.Text.StringBuilder

        Dim SetupFormTypesElement As IACDataElement
        Dim SetupFormTypes As IACDataElementCollection
        Dim SetupDocClassesElement As IACDataElement = SetupRootElement.FindChildElementByName("DocumentClasses")
        Dim SetupDocClasses As IACDataElementCollection = SetupDocClassesElement.FindChildElementsByName("DocumentClass")

        For Each DocClass As IACDataElement In SetupDocClasses
            sb.Append(DocClass("Name") + "(")

            SetupFormTypesElement = DocClass.FindChildElementByName("FormTypes")
            SetupFormTypes = SetupFormTypesElement.FindChildElementsByName("FormType")

            For Each FormType As IACDataElement In SetupFormTypes
                sb.Append(FormType("Name") + ",")
            Next

            sb.Append(")")
        Next

        Return sb.ToString
    End Function

End Class
