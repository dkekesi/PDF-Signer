Imports System.IO
Imports PDFSignerCommon

Friend Class DocumentOperation
    Friend Sub SkipSigning(Document As DocumentItem, SkipNote As String)
        Dim frm As New FrmSkip With {
            .SkipNote = SkipNote
        }

        If frm.ShowDialog() = DialogResult.OK Then
            SkipSigningOperation(Document, frm.SkipNote)
        End If
    End Sub

    Private Sub SkipSigningOperation(Document As DocumentItem, SkipNote As String)
        With Document
            PurgeSignatureAndSkipData(Document)

            .IsSkipped = True
            .SkipNote = SkipNote
            SetCSSValue(.KofaxElement, CSS.IsDocumentSkipped, "1")
            SetIndexValue(.KofaxElement, .SetupData.IndexSkipNote, SkipNote)
            .ProgressPercent = 0

            DeleteSignature(Document, False)

            Dim bp As New BatchParser
            .IconType = bp.GetBatchTreeItemIcon(Document)
        End With
    End Sub

    Friend Sub Reject(Document As DocumentItem)
        Dim frm As New FrmReject

        If frm.ShowDialog() = DialogResult.OK Then
            Reject(Document, frm.RejectionNote)
        End If
    End Sub

    Friend Sub Reject(Document As DocumentItem, RejectionNote As String)
        With Document
            If .IsSigned OrElse .IsSkipped Then
                DeleteSignature(Document, True)
            End If

            .IsRejected = True
            .KofaxElement("Rejected") = True
            .KofaxElement("Note") = (.KofaxElement("Note") + " " + RejectionNote).Trim
            .ProgressPercent = 0

            Dim bp As New BatchParser
            .IconType = bp.GetBatchTreeItemIcon(Document)
        End With
    End Sub

    Friend Sub Reject(Page As PageItem)
        Dim frm As New FrmReject

        If frm.ShowDialog() = DialogResult.OK Then
            If Page.Document.IsSigned Then
                DeleteSignature(Page.Document, True)
            End If

            Page.IsRejected = True
            Page.KofaxElement("Rejected") = True
            Page.KofaxElement("Note") = (Page.KofaxElement("Note") + " " + frm.RejectionNote).Trim
            Page.Document.ProgressPercent = 0

            Dim bp As New BatchParser
            Page.IconType = bp.GetBatchTreeItemIcon(Page)
            Page.Document.IconType = bp.GetBatchTreeItemIcon(Page.Document)
        End If
    End Sub

    Friend Sub Unreject(Document As DocumentItem)
        With Document
            .IsRejected = False
            .KofaxElement("Rejected") = False
            .KofaxElement("Note") = String.Empty

            Dim bp As New BatchParser
            .IconType = bp.GetBatchTreeItemIcon(Document)
        End With
    End Sub

    Friend Sub Unreject(Page As PageItem)
        With Page
            .IsRejected = False
            .KofaxElement("Rejected") = False
            .KofaxElement("Note") = String.Empty

            Dim bp As New BatchParser
            .IconType = bp.GetBatchTreeItemIcon(Page)
        End With
    End Sub

    Friend Sub DeleteSignature(Document As DocumentItem, PurgeData As Boolean)
        If PurgeData Then
            PurgeSignatureAndSkipData(Document)
        End If

        With Document
            Try
                If Not String.IsNullOrEmpty(.SignedFilePath) Then
                    If .SetupData.FileOverwriteOriginal Then
                        If .IsSignatureNeeded AndAlso Not .IsSkipped Then
                            If File.Exists(.SignedFilePath) Then File.Delete(.SignedFilePath)
                        Else
                            If File.Exists(.UnsignedFilePath) Then File.Copy(.UnsignedFilePath, .SignedFilePath, True)
                        End If
                    End If

                    If .SetupData.FileCreateNew Then
                        If File.Exists(.SignedFilePath) Then File.Delete(.SignedFilePath)
                    End If
                End If

                If Not String.IsNullOrEmpty(.DetachedSignatureFilePath) AndAlso File.Exists(.DetachedSignatureFilePath) Then File.Delete(.DetachedSignatureFilePath)
                If Not String.IsNullOrEmpty(.ClauseFilePath) AndAlso File.Exists(.ClauseFilePath) Then File.Delete(.ClauseFilePath)
            Catch
            End Try

            Dim bp As New BatchParser
            .IconType = bp.GetBatchTreeItemIcon(Document)
        End With
    End Sub

    Friend Sub PurgeSignatureAndSkipData(Document As DocumentItem)
        With Document
            SetCSSValue(.KofaxElement, CSS.IsDocumentSigned, "0")
            SetCSSValue(.KofaxElement, CSS.IsDocumentSkipped, "0")
            SetCSSValue(.KofaxElement, CSS.SignedFilePath, String.Empty)
            SetCSSValue(.KofaxElement, CSS.DetachedSignatureFilePath, String.Empty)
            SetCSSValue(.KofaxElement, CSS.ClauseFilePath, String.Empty)
            SetCSSValue(.KofaxElement, CSS.ClauseContent, String.Empty)
            SetCSSValue(.KofaxElement, CSS.SignatureLogContent, String.Empty)
            SetCSSValue(.KofaxElement, CSS.SignedFileDateTime, String.Empty)
            SetIndexValue(.KofaxElement, .SetupData.IndexIsSigned, String.Empty)
            SetIndexValue(.KofaxElement, .SetupData.IndexSignedDateTime, String.Empty)
            SetIndexValue(.KofaxElement, .SetupData.IndexSignedBy, String.Empty)
            SetIndexValue(.KofaxElement, .SetupData.IndexSignatureValidUntil, String.Empty)
            SetIndexValue(.KofaxElement, .SetupData.IndexSkipNote, String.Empty)

            .IsSigned = False
            .IsSkipped = False
            .SkipNote = String.Empty
            .ClauseContent = String.Empty
            .SignatureLogContent = String.Empty
            .ProgressPercent = 0
        End With
    End Sub

End Class
