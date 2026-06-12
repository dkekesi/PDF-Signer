Imports DevExpress.XtraPdfViewer.Localization

Friend Class DevExpressPDFViewer
    Inherits ViewerBase

    Friend Overrides ReadOnly Property SupportsRotate As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overrides ReadOnly Property SupportsDelete As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overrides ReadOnly Property HasDocument
        Get
            Return PageViewer.IsDocumentOpened
        End Get
    End Property
    Friend Overrides ReadOnly Property PageWidth As Integer
        Get
            If HasDocument Then
                Return Math.Round(PageViewer.GetPageSize(1).Width * 25.4)
            Else
                Return -1
            End If
        End Get
    End Property
    Friend Overrides ReadOnly Property PageHeight As Integer
        Get
            If HasDocument Then
                Return Math.Round(PageViewer.GetPageSize(1).Height * 25.4)
            Else
                Return -1
            End If
        End Get
    End Property
    Friend Overrides ReadOnly Property CurrentPageNumber As Integer
        Get
            If HasDocument Then
                Return PageViewer.CurrentPageNumber
            Else
                Return -1
            End If
        End Get
    End Property

#Region "Event handlers"

    Private Sub PageViewer_CurrentPageChanged(sender As Object, e As DevExpress.XtraPdfViewer.PdfCurrentPageChangedEventArgs) Handles PageViewer.CurrentPageChanged
        If _RunPageChangedEvent Then OnCurrentPageChangedEvent(sender, e)
    End Sub

    Private Sub PageViewer_PopupMenuShowing(sender As Object, e As DevExpress.XtraPdfViewer.PdfPopupMenuShowingEventArgs) Handles PageViewer.PopupMenuShowing
        e.ItemLinks.Clear()
    End Sub

    Private Sub GUI_MouseEnter(sender As Object, e As EventArgs) Handles PageViewer.MouseEnter
        sender.Focus()
    End Sub

#End Region

    Public Sub New()
        DevExpress.XtraPdfViewer.Localization.XtraPdfViewerLocalizer.Active = New ViewerLocalizer

        InitializeComponent()
        PageViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel
        PageViewer.NavigationPaneInitialVisibility = DevExpress.XtraPdfViewer.PdfNavigationPaneVisibility.Hidden
        PageViewer.CursorMode = DevExpress.XtraPdfViewer.PdfCursorMode.HandTool

        Dim loc As New XtraPdfViewerLocalizer
        loc.GetLocalizedString(XtraPdfViewerStringId.MessageSaveAttachmentError)
    End Sub

    Private Sub PDFViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SplitAttachment.Collapsed = True ' we need to do this in code, otherwise column sizes of the attachment list will be weird
    End Sub

    Friend Overrides Sub ActivatePage(PageNumber As Integer)
        If Not HasDocument Then Return

        If PageViewer.PageCount >= PageNumber Then
            _RunPageChangedEvent = False
            PageViewer.CurrentPageNumber = PageNumber
            _RunPageChangedEvent = True
        End If
    End Sub

#Region "Open/close document"

    Friend Overrides Sub OpenDocument(PDFDocumentPathAndFileName As String)
        _RunPageChangedEvent = False

        ' do not load large files completely into memory, just the part that is being rendered
        Dim fileSize As Integer = New IO.FileInfo(PDFDocumentPathAndFileName).Length
        If fileSize > Constant.MemoryStreamSizeLimit Then
            PageViewer.DetachStreamAfterLoadComplete = False ' stream file as needed
        Else
            PageViewer.DetachStreamAfterLoadComplete = True ' completely load file
        End If

        PageViewer.LoadDocument(PDFDocumentPathAndFileName)

        _RunPageChangedEvent = True
    End Sub

    Friend Overrides Sub CloseDocument()
        If HasDocument Then
            PageViewer.CloseDocument()
        End If
    End Sub

    Friend Overrides Sub GetPDFStream(PDFStream As IO.Stream)
        PageViewer.SaveDocument(PDFStream)
    End Sub

#End Region

#Region "Zooming"
    Friend Overrides Sub ZoomWholePage()
        PageViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.PageLevel
    End Sub

    Friend Overrides Sub ZoomWidth()
        PageViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.FitToWidth
    End Sub

    Friend Overrides Sub Zoom1on1()
        PageViewer.ZoomMode = DevExpress.XtraPdfViewer.PdfZoomMode.ActualSize
    End Sub

    Friend Overrides Sub ZoomIn()
        PageViewer.ZoomFactor += 10
    End Sub

    Friend Overrides Sub ZoomOut()
        PageViewer.ZoomFactor -= 10
    End Sub
#End Region

End Class

Friend Class ViewerLocalizer
    Inherits DevExpress.XtraPdfViewer.Localization.XtraPdfViewerLocalizer

    'Public Overrides Function GetLocalizedString(id As XtraPdfViewerStringId) As String
    '    'If Object.ReferenceEquals(id, DevExpress.XtraPdfViewer.Localization.XtraPdfViewerStringId.MessageLoadingError) Then
    '    '    Return ""
    '    'End If

    '    Return MyBase.GetLocalizedString(id)
    'End Function
End Class