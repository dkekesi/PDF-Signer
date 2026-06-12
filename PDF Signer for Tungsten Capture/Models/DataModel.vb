Imports Kofax.Capture.SDK.Data
Imports PDFSignerCommon
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Friend Class BatchContents
    Public Property Content As BindingList(Of TreeItem)

    Public Sub New()
        Content = New BindingList(Of TreeItem)
    End Sub

    Friend Sub AddBatch(Batch As BatchItem)
        Content.Add(Batch)
    End Sub

    Friend Sub AddDocumentToBatch(Batch As BatchItem, Document As DocumentItem)
        Document.Batch = Batch
        Batch.Documents.Add(Document)
        Content.Add(Document)
    End Sub

    Friend Sub AddPageToDocument(Document As DocumentItem, Page As PageItem)
        Page.Document = Document
        Document.Pages.Add(Page)
        Content.Add(Page)
    End Sub
End Class

Friend Class TreeItem
    Implements INotifyPropertyChanged

    Public Property ID As Integer
    Public Property ParentID As Integer
    Public Property KofaxElement As IACDataElement
    Public Property IconType As Integer
    Public Property OrderNumber As Integer
    Public Property DisplayName As String
    Public Property FormTypeName As String
    Public Property IsRejected As Boolean

    Private _progressPercent As Integer
    Public Property ProgressPercent As Integer
        Get
            Return _progressPercent
        End Get

        Set(value As Integer)
            If Not value = _progressPercent Then
                _progressPercent = value
                NotifyPropertyChanged()
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ' This method is called by the Set accessor of each property.
    ' The CallerMemberName attribute that is applied to the optional propertyName
    ' parameter causes the property name of the caller to be substituted as an argument.
    Private Sub NotifyPropertyChanged(<CallerMemberName()> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

Friend Class BatchItem
    Inherits TreeItem

    Public Property Contents As BindingList(Of TreeItem)
    Public Property Documents As BindingList(Of DocumentItem)
    Public Property SetupRootElement As IACDataElement

    Public Sub New()
        Contents = New BindingList(Of TreeItem)
        Documents = New BindingList(Of DocumentItem)
    End Sub
End Class

Friend Class DocumentItem
    Inherits TreeItem

    Public Property Batch As BatchItem
    Public Property Pages As List(Of PageItem)
    Public Property SetupData As SetupModel
    Public Property KofaxDocumentGUID As String
    Public Property KofaxPDFFilePath As String
    Public Property BarCode As String
    Public Property IsSigned As Boolean
    Public Property IsSignatureNeeded As Boolean
    Public Property IsSigningPossible As Boolean
    Public Property IsSkipped As Boolean
    Public Property SkipNote As String
    Public Property IsPDFSkipFirstPage As Boolean
    Public Property PartialCopyData As String
    Public Property ClauseFilePath As String
    Public Property ClauseContent As String
    Public Property SignatureLogContent As String
    Public Property ErrorMessage As String
    Public Property SignedFilePath As String
    Public Property UnsignedFilePath As String
    Public Property UnsignedFileSize As Integer
    Public Property DetachedSignatureFilePath As String
    Public Property SignedFileLastTimeStamp As Date?
    Public Property UnsignedFileLastTimeStamp As Date?
    Public Property FileSize As Integer

    Public ReadOnly Property Available As Boolean
        Get
            ' If String.IsNullOrEmpty(KofaxPDFFilePath) Then Return False
            If ProgressPercent = 0 OrElse ProgressPercent = 100 Then Return True
            Return False
        End Get
    End Property
    Public ReadOnly Property HasError As Boolean
        Get
            If String.IsNullOrWhiteSpace(ErrorMessage) Then Return False
            Return True
        End Get
    End Property
    Public ReadOnly Property ProgressBarColor As Color
        Get
            If ProgressPercent < 100 Then Return SystemColors.Highlight
            If ProgressPercent = 100 Then
                If HasError Then
                    Return Color.Red
                Else
                    Return Color.Green
                End If
            End If
        End Get
    End Property

    Public Sub New()
        Pages = New List(Of PageItem)
    End Sub
End Class

Friend Class PageItem
    Inherits TreeItem
    Public Property Document As DocumentItem

    Friend Sub Delete()
        KofaxElement.Delete()
        Document.Pages.Remove(Me)
        Document.Batch.Contents.Remove(Me)
    End Sub
End Class
