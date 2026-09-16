Imports nsoftware.SecureBlackbox

''' <summary>Display ids of a document's signing entities for the log: signatures S0, S1…, document timestamps T0, T1…, a signature's own timestamps S0T0….</summary>
Friend NotInheritable Class EntityNaming
    ''' <summary>One PDFSignature entity in document order.</summary>
    Friend Structure EntityDescriptor
        ''' <summary>The SecureBlackbox entity label, or an empty string when SecureBlackbox assigned none.</summary>
        Friend ReadOnly SbbLabel As String
        ''' <summary>True when the entity is a document-level timestamp rather than a signature.</summary>
        Friend ReadOnly IsDocumentTimestamp As Boolean

        ''' <summary>Describes one entity by its SecureBlackbox label and whether it is a document timestamp.</summary>
        Friend Sub New(SbbLabel As String, IsDocumentTimestamp As Boolean)
            Me.SbbLabel = SbbLabel
            Me.IsDocumentTimestamp = IsDocumentTimestamp
        End Sub
    End Structure

    ''' <summary>One TimestampInfo in SecureBlackbox list order with the label of the entity it seals.</summary>
    Friend Structure TimestampDescriptor
        ''' <summary>The SecureBlackbox label of the timestamp entity itself.</summary>
        Friend ReadOnly SbbLabel As String
        ''' <summary>The SecureBlackbox label of the signature or document timestamp this timestamp seals.</summary>
        Friend ReadOnly ParentSbbLabel As String

        ''' <summary>Describes one timestamp by its own label and the label of the entity it seals.</summary>
        Friend Sub New(SbbLabel As String, ParentSbbLabel As String)
            Me.SbbLabel = SbbLabel
            Me.ParentSbbLabel = ParentSbbLabel
        End Sub
    End Structure

    ''' <summary>Maps nothing: every label passes through.</summary>
    Friend Shared ReadOnly Empty As New EntityNaming(New Dictionary(Of String, String)(StringComparer.Ordinal), New HashSet(Of String)(StringComparer.Ordinal))

    Private ReadOnly _display As Dictionary(Of String, String)
    Private ReadOnly _documentTimestamps As HashSet(Of String)

    Private Sub New(Display As Dictionary(Of String, String), DocumentTimestamps As HashSet(Of String))
        _display = Display
        _documentTimestamps = DocumentTimestamps
    End Sub

    ''' <summary>The key an entity is tracked by: its SecureBlackbox label, or "S"/"DT" plus document order when it has none.</summary>
    Friend Shared Function EntityKey(SbbLabel As String, DocumentOrder As Integer, IsDocumentTimestamp As Boolean) As String
        If Not String.IsNullOrEmpty(SbbLabel) Then Return SbbLabel
        Return If(IsDocumentTimestamp, $"DT{DocumentOrder}", $"S{DocumentOrder}")
    End Function

    ''' <summary>Builds the map from entities (document order) and timestamps (SecureBlackbox order); a timestamp label never overrides an entity label.</summary>
    Friend Shared Function Build(Entities As IList(Of EntityDescriptor), Timestamps As IList(Of TimestampDescriptor)) As EntityNaming
        Dim display As New Dictionary(Of String, String)(StringComparer.Ordinal)
        Dim documentTimestamps As New HashSet(Of String)(StringComparer.Ordinal)
        Dim signatures As Integer = 0
        Dim docTimestamps As Integer = 0
        For Each e As EntityDescriptor In Entities
            If String.IsNullOrEmpty(e.SbbLabel) Then Continue For
            If e.IsDocumentTimestamp Then
                display(e.SbbLabel) = $"T{docTimestamps}"
                docTimestamps += 1
                documentTimestamps.Add(e.SbbLabel)
            Else
                display(e.SbbLabel) = $"S{signatures}"
                signatures += 1
            End If
        Next

        Dim naming As New EntityNaming(display, documentTimestamps)
        Dim ordinalByParent As New Dictionary(Of String, Integer)(StringComparer.Ordinal)
        For Each t As TimestampDescriptor In Timestamps
            If String.IsNullOrEmpty(t.SbbLabel) OrElse String.IsNullOrEmpty(t.ParentSbbLabel) Then Continue For
            Dim ordinal As Integer = 0
            ordinalByParent.TryGetValue(t.ParentSbbLabel, ordinal)
            ordinalByParent(t.ParentSbbLabel) = ordinal + 1
            If Not display.ContainsKey(t.SbbLabel) Then display(t.SbbLabel) = naming.DisplayTimestamp(t.ParentSbbLabel, ordinal)
        Next
        Return naming
    End Function

    ''' <summary>Display id of a label; an unknown, empty or Nothing label is returned as is.</summary>
    Friend Function Display(SbbLabel As String) As String
        Dim id As String = Nothing
        If SbbLabel IsNot Nothing AndAlso _display.TryGetValue(SbbLabel, id) Then Return id
        Return SbbLabel
    End Function

    ''' <summary>Display id of the Ordinal-th (0-based) timestamp sealing the parent: "S0T1" under a signature, the parent's own id under a document timestamp.</summary>
    Friend Function DisplayTimestamp(ParentSbbLabel As String, Ordinal As Integer) As String
        If ParentSbbLabel IsNot Nothing AndAlso _documentTimestamps.Contains(ParentSbbLabel) Then Return Display(ParentSbbLabel)
        Return $"{Display(ParentSbbLabel)}T{Ordinal}"
    End Function

    ''' <summary>Reads the entity and timestamp lists of a verifier that has run Verify() (parse-only is enough).</summary>
    Friend Shared Function FromVerifier(Verifier As PDFVerifier) As EntityNaming
        Dim entities As New List(Of EntityDescriptor)
        For i As Integer = 0 To Verifier.Signatures.Count - 1
            Dim s As PDFSignature = Verifier.Signatures(i)
            Dim isDocTs As Boolean = s.SignatureType = PDFSignatureTypes.pstDocumentTimestamp
            entities.Add(New EntityDescriptor(EntityKey(s.EntityLabel, i, isDocTs), isDocTs))
        Next
        Dim timestamps As New List(Of TimestampDescriptor)
        For j As Integer = 0 To Verifier.Timestamps.Count - 1
            Dim t As TimestampInfo = Verifier.Timestamps(j)
            timestamps.Add(New TimestampDescriptor(t.EntityLabel, t.ParentEntity))
        Next
        Return Build(entities, timestamps)
    End Function
End Class
