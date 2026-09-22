Imports Kofax.Capture.SDK.Workflow
Imports Kofax.Capture.SDK.Data
Imports System.Runtime.InteropServices
Imports PDFSignerCommon

' Capture activates the agent through the ProgID named in the .aex, so the COM attributes are
' load-bearing; the assembly is ComVisible(False), so the class has to opt in on its own.
<Guid("A4F33D6B-674C-4AFA-8747-9FF5137FF82A"),
ComVisible(True),
ClassInterface(ClassInterfaceType.None),
ProgId("DocSoft.PDFSignerWFA"),
CLSCompliant(False)>
Public Class WFAgent
    Implements IACWorkflowAgent

    Private Const PDFSIGNER_ID As String = "DocSoft.PDFSigner"

    Public Sub ProcessWorkflow(ByRef WorkflowData As IACWorkflowData) Implements IACWorkflowAgent.ProcessWorkflow
        SetupDocClassParser.ResetSetupDocClassData()
        SetupParser.ResetSetupData()

        '*** Next module is PDF Signer
        If (UCase(WorkflowData.NextModule.ID) = UCase(PDFSIGNER_ID)) Then
            Dim SkipPDFSigner As Boolean = True

            Dim RootElement As IACDataElement = WorkflowData.ExtractRuntimeACDataElement(0)
            Dim SetupRootElement As IACDataElement = WorkflowData.ExtractSetupACDataElement(0)

            Dim BatchElement As IACDataElement = RootElement.FindChildElementByName("Batch")
            Dim DocumentsElement As IACDataElement = BatchElement.FindChildElementByName("Documents")
            Dim DocumentsCollection As IACDataElementCollection = DocumentsElement.FindChildElementsByName("Document")

            For Each doc As IACDataElement In DocumentsCollection
                Dim FormTypeName As String = doc("FormTypeName")
                Dim SetupDocClassData As IACDataElement = SetupDocClassParser.GetSetupDocClassByFormTypeName(SetupRootElement, FormTypeName)
                Dim SetupData As SetupModel = SetupParser.GetSetupDataAtRuntime(SetupDocClassData, SetupExtractMode.WFADataExtract)
                Dim BarCode As String = GetIndexValue(doc, SetupData.IndexBarCode)

                If IsDocumentToBeSigned(SetupData, BarCode) Then
                    SkipPDFSigner = False
                    Exit For
                End If
            Next

            If SkipPDFSigner Then
                AlterWorkflow(WorkflowData)
            End If
        End If
    End Sub

    Private Function IsDocumentToBeSigned(SetupData As SetupModel, BarCode As String) As Boolean
        If SetupData.SignAllDocuments Then Return True
        If SetupData.SignatureMarkerPosition = 0 Then Return False

        If BarCode.Length >= SetupData.SignatureMarkerPosition Then
            If BarCode.Substring(SetupData.SignatureMarkerPosition - 1, 1) = SetupData.SignatureMarker Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Sub AlterWorkflow(ByRef WorkflowData As IACWorkflowData)
        Dim PossibleModules As IACWorkflowModules
        Dim PossibleModule As IACWorkflowModule

        PossibleModules = WorkflowData.PossibleModules

        '*** Find PDFSigner in possible modules
        Dim FoundPDFSigner As Boolean

        For Each PossibleModule In PossibleModules
            If UCase(PossibleModule.ID) = UCase(PDFSIGNER_ID) Then FoundPDFSigner = True

            '*** Find the first module after PDFSigner
            If FoundPDFSigner AndAlso UCase(PossibleModule.ID) <> UCase(PDFSIGNER_ID) Then

                '*** Set to next module and Ready
                WorkflowData.NextModule = PossibleModule
                WorkflowData.NextState = WorkflowData.PossibleStates("Ready")

                Return
            End If
        Next

        '*** No more modules, batch is completed
        WorkflowData.NextState = WorkflowData.PossibleStates("Completed")
    End Sub

End Class