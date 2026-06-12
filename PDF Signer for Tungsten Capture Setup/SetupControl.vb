Imports System.Runtime.InteropServices
Imports PDFSignerSetup.My.Resources
Imports PDFSignerCommon
Imports Kofax.Capture.AdminModule.InteropServices

<ProgId("PDFSignerSetup.Setup")> _
<Guid("5ED75FCE-D4D3-46D8-91BB-386D829379B1")> _
Public Class SetupControl
    Inherits UserControl

    Private Const ModuleName = "PDF Signer"
    Private AdminApp As AdminApplication

    Public WriteOnly Property Application() As AdminApplication
        Set(Value As AdminApplication)
            Try
                AdminApp = Value
            Catch ex As Exception
                MsgBox("Application error")
            End Try
        End Set
    End Property

    Public Function ActionEvent(nActionNumber As Integer, ByRef vArgument As Object, ByRef pnCancel As Integer) As Integer
        pnCancel = 0
        ActionEvent = 0

        Dim IsAnyDocClassEnabled As Boolean

        Select Case nActionNumber

            Case KfxOcxEvent.KfxOcxEventMenuClicked

                If CType(vArgument, String) = "PDFSignerSetupMenu" Then
                    Dim frm = New FrmSetup With {
                        .AdminApp = AdminApp
                    }
                    frm.Text += " - " & GetType(SetupControl).Assembly.GetName.Version.ToString
                    frm.ShowDialog()
                End If

            Case KfxOcxEvent.KfxOcxEventPublishWarningsGet

                For i = 1 To AdminApp.ActiveBatchClass.AssignedQueues.Count
                    If AdminApp.ActiveBatchClass.AssignedQueues.Item(i).Name = ModuleName Then

                        For Each doc As DocumentClass In AdminApp.ActiveBatchClass.DocumentClasses
                            Dim hnd = New SetupCSSParser(doc)
                            If hnd.ReadSetupCSS(CSS.IsEnabled) = "1" Then
                                IsAnyDocClassEnabled = True
                                Exit For
                            End If
                        Next

                        If Not IsAnyDocClassEnabled Then
                            MsgBox(Messages.Publish_No_Doc_Class_Has_Signing_Enabled, MsgBoxStyle.Exclamation, ModuleName)
                        End If

                    End If
                Next

            Case KfxOcxEvent.KfxOcxEventPublishErrorsGet

                For i = 1 To AdminApp.ActiveBatchClass.AssignedQueues.Count
                    If AdminApp.ActiveBatchClass.AssignedQueues.Item(i).Name = ModuleName Then

                        For Each doc As DocumentClass In AdminApp.ActiveBatchClass.DocumentClasses
                            Dim hnd As New SetupCSSParser(doc)

                            If hnd.ReadSetupCSS(CSS.IsEnabled) = "1" Then
                                If Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexBarCode) OrElse
                                   Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexIsSigned) OrElse
                                   Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexSignedDateTime) OrElse
                                   Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexSignedBy) OrElse
                                   Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexSignatureValidUntil) OrElse
                                   Not hnd.IsIndexNameStoredInCSSValid(CSS.IndexDocumentName) Then
                                    MsgBox(String.Format(Messages.Publish_Invalid_Index_Field, doc.Name), MsgBoxStyle.Exclamation, ModuleName)
                                End If
                            End If
                        Next
                    End If
                Next

        End Select

    End Function
End Class
