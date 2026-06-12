Imports TallComponents.PDF
Imports PDFSignerCommon
Imports System.IO

Friend Class TallPDFViewer
    Inherits ViewerBase

    Dim PDFDocument As Document

    Friend Overrides ReadOnly Property SupportsRotate As Boolean
        Get
            Return True
        End Get
    End Property
    Friend Overrides ReadOnly Property SupportsDelete As Boolean
        Get
            Return True
        End Get
    End Property
    Friend Overrides ReadOnly Property HasDocument
        Get
            If PDFDocument Is Nothing Then
                Return False
            Else
                Return True
            End If
        End Get
    End Property
    Friend Overrides ReadOnly Property PageWidth As Integer
        Get
            If HasDocument Then
                Return Math.Round(PDFDocument.Pages(0).Width * 0.352777778)
            Else
                Return -1
            End If
        End Get
    End Property

    Friend Overrides ReadOnly Property PageHeight As Integer
        Get
            If HasDocument Then
                Return Math.Round(PDFDocument.Pages(0).Height * 0.352777778)
            Else
                Return -1
            End If
        End Get
    End Property
    Friend Overrides ReadOnly Property CurrentPageNumber As Integer
        Get
            If HasDocument Then
                Return PageViewer.CurrentPageIndex
            Else
                Return -1
            End If
        End Get
    End Property

#Region "Event handlers"

    Private Sub PageViewer_CurrentPageChanged(sender As Object, e As EventArgs) Handles PageViewer.CurrentPageChanged
        If _RunPageChangedEvent Then OnCurrentPageChangedEvent(sender, e)
    End Sub

    Private Sub GUI_MouseEnter(sender As Object, e As EventArgs) Handles PageViewer.MouseEnter, ThumbnailViewer.MouseEnter
        sender.Focus()
    End Sub

    Private Sub SplitThumbViewer_SplitGroupPanelCollapsed(sender As Object, e As DevExpress.XtraEditors.SplitGroupPanelCollapsedEventArgs) Handles SplitThumbViewer.SplitGroupPanelCollapsed
        If SplitThumbViewer.Collapsed Then
            ThumbnailViewer.Document = Nothing
        Else
            ThumbnailViewer.Document = PDFDocument
        End If
    End Sub

#End Region

    Public Sub New()
        InitializeComponent()
        PageViewer.ZoomMode = TallComponents.Interaction.WinForms.ZoomMode.FitEntirePage
    End Sub

    Private Sub PDFViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitAttachment.Collapsed = True ' we need to do this in code, otherwise column sizes of the attachment list will be weird
    End Sub

    Friend Overrides Sub ActivatePage(PageNumber As Integer)
        If PDFDocument Is Nothing OrElse Not PDFDocument.HasDocument Then Return

        If PDFDocument.Pages.Count >= PageNumber Then
            _RunPageChangedEvent = False
            PageViewer.CurrentPageIndex = PageNumber - 1
            _RunPageChangedEvent = True
        End If
    End Sub

#Region "Open/close document"

    Friend Overrides Sub OpenDocument(PDFDocumentPathAndFileName As String)
        _RunPageChangedEvent = False
        PDFDocument = New Document
        PDFDocument.Open(PDFDocumentPathAndFileName)
        PageViewer.Document = PDFDocument
        PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.Single

        If Not SplitThumbViewer.Collapsed Then
            ThumbnailViewer.Document = PDFDocument
        End If

        SplitThumbViewer.SplitterPosition += 1
        SplitThumbViewer.SplitterPosition -= 1
        _RunPageChangedEvent = True
    End Sub

    Friend Overrides Sub CloseDocument()
        If PDFDocument Is Nothing Then Return

        PDFDocument.Close()
        ThumbnailViewer.Document = Nothing
        PageViewer.Document = Nothing
        PDFDocument = Nothing
    End Sub

    Friend Overrides Sub GetPDFStream(PDFStream As Stream)
        PageViewer.Document.Write(PDFStream, True)
    End Sub

#End Region

#Region "Rotate/delete page"

    Friend Overrides Sub RotatePage(Direction As RotateDirection, SaveToPathWithFileName As String)
        If PDFDocument Is Nothing OrElse Not PDFDocument.HasDocument Then Return
        If PageViewer.CurrentPageIndex < 0 Then Return

        ' perform rotate in control
        If Direction = RotateDirection.Left Then
            PageViewer.RotatePageLeft(PageViewer.CurrentPageIndex)
        Else
            PageViewer.RotatePageRight(PageViewer.CurrentPageIndex)
        End If

        ' write modified PDF
        Using fs = New FileStream(SaveToPathWithFileName, FileMode.Truncate, FileAccess.Write)
            PDFDocument.Write(fs)
        End Using

        Me.Refresh()
    End Sub

    Friend Overrides Sub DeleteCurrentPage(SaveToPathWithFileName As String)
        If PDFDocument Is Nothing OrElse Not PDFDocument.HasDocument OrElse PDFDocument.Pages.Count = 1 Then Return
        If PageViewer.CurrentPageIndex < 0 Then Return

        ' perform delete from control
        PDFDocument.Pages.RemoveAt(PageViewer.CurrentPageIndex)

        ' write modified PDF
        Using fs = New FileStream(SaveToPathWithFileName, FileMode.Truncate, FileAccess.Write)
            PDFDocument.Write(fs)
        End Using
    End Sub

#End Region

#Region "Zooming"
    Friend Overrides Sub ZoomWholePage()
        PageViewer.ZoomMode = TallComponents.Interaction.WinForms.ZoomMode.FitEntirePage
    End Sub

    Friend Overrides Sub ZoomWidth()
        PageViewer.ZoomMode = TallComponents.Interaction.WinForms.ZoomMode.FitPageWidth
    End Sub

    Friend Overrides Sub Zoom1on1()
        PageViewer.ZoomMode = TallComponents.Interaction.WinForms.ZoomMode.ActualSize
    End Sub

    Friend Overrides Sub ZoomIn()
        PageViewer.ZoomFactor += 0.2
    End Sub

    Friend Overrides Sub ZoomOut()
        PageViewer.ZoomFactor -= 0.2
    End Sub
#End Region

#Region "Layout"
    Friend Overrides Sub SinglePageLayout()
        PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.Single
    End Sub

    Friend Overrides Sub DoublePageLayout()
        PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.Double
    End Sub

    Friend Overrides Sub ContinuousSinglePageLayout()
        PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.TopToBottom
    End Sub

    Friend Overrides Sub ContinuousDoublePageLayout()
        PageViewer.PageLayout = TallComponents.Interaction.WinForms.PageLayout.DoubleTopToBottom
    End Sub
#End Region

End Class
