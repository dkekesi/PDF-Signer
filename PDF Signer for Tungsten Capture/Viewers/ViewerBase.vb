Imports System.ComponentModel
Imports PDFSignerCommon

Friend Class ViewerBase
    Friend _RunPageChangedEvent As Boolean = True

    <Browsable(True)> _
    Public Property ViewerDocType As DocumentType
    Friend Overridable ReadOnly Property SupportsRotate As Boolean
        Get
            Return False
        End Get
    End Property
    Friend Overridable ReadOnly Property SupportsDelete As Boolean
        Get
            Return False
        End Get
    End Property
    Friend ReadOnly Property SupportsDocumentEditing As Boolean
        Get
            Return SupportsRotate Or SupportsDelete
        End Get
    End Property
    Friend Overridable ReadOnly Property HasDocument
        Get
            Return False
        End Get
    End Property
    Friend Overridable ReadOnly Property PageWidth As Integer
        Get
            Return -1
        End Get
    End Property
    Friend Overridable ReadOnly Property PageHeight As Integer
        Get
            Return -1
        End Get
    End Property
    Friend Overridable ReadOnly Property CurrentPageNumber As Integer
        Get
            Return -1
        End Get
    End Property

#Region "Events"

    Friend Event CurrentPageChangedEvent(sender As Object, e As EventArgs)

    Friend Sub OnCurrentPageChangedEvent(sender As Object, e As EventArgs)
        RaiseEvent CurrentPageChangedEvent(sender, e)
    End Sub

#End Region

    Friend Overridable Sub ActivatePage(PageNumber As Integer)
    End Sub

#Region "Open/close document"

    Friend Overridable Sub OpenDocument(PDFDocumentPathAndFileName As String)
    End Sub

    Friend Overridable Sub CloseDocument()
    End Sub

    Friend Overridable Sub GetPDFStream(PDFStream As IO.Stream)
    End Sub

#End Region

#Region "Rotate/delete page"

    Friend Overridable Sub RotatePage(Direction As RotateDirection, SaveToPathWithFileName As String)
    End Sub

    Friend Overridable Sub DeleteCurrentPage(SaveToPathWithFileName As String)
    End Sub

#End Region

#Region "Zooming"
    Friend Overridable Sub ZoomWholePage()
    End Sub

    Friend Overridable Sub ZoomWidth()
    End Sub

    Friend Overridable Sub Zoom1on1()
    End Sub

    Friend Overridable Sub ZoomIn()
    End Sub

    Friend Overridable Sub ZoomOut()
    End Sub
#End Region

#Region "Layout"
    Friend Overridable Sub SinglePageLayout()
    End Sub

    Friend Overridable Sub DoublePageLayout()
    End Sub

    Friend Overridable Sub ContinuousSinglePageLayout()
    End Sub

    Friend Overridable Sub ContinuousDoublePageLayout()
    End Sub
#End Region

End Class
