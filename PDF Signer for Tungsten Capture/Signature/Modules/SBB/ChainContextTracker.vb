''' <summary>Subject and issuer common names of the certificate SecureBlackbox is processing; either may be Nothing.</summary>
Friend NotInheritable Class ChainContext
    ''' <summary>Common name of the certificate's subject, or Nothing when unknown.</summary>
    Friend ReadOnly Property SubjectCommonName As String
    ''' <summary>Common name of the certificate's issuer, or Nothing when unknown.</summary>
    Friend ReadOnly Property IssuerCommonName As String

    ''' <summary>Creates a context snapshot from the given subject and issuer common names.</summary>
    Friend Sub New(SubjectCommonName As String, IssuerCommonName As String)
        Me.SubjectCommonName = SubjectCommonName
        Me.IssuerCommonName = IssuerCommonName
    End Sub

    ''' <summary>Context with no observed certificate.</summary>
    Friend Shared ReadOnly Empty As New ChainContext(Nothing, Nothing)

    ''' <summary>True when neither the subject nor the issuer common name is known.</summary>
    Friend ReadOnly Property IsEmpty As Boolean
        Get
            Return String.IsNullOrEmpty(SubjectCommonName) AndAlso String.IsNullOrEmpty(IssuerCommonName)
        End Get
    End Property
End Class

''' <summary>Follows the certificate under validation from chain events so an error event, which names no certificate, can be attributed to it.</summary>
Friend NotInheritable Class ChainContextTracker
    Private _subject As String
    Private _issuer As String

    ''' <summary>Snapshot of the certificate currently being processed.</summary>
    Friend ReadOnly Property Current As ChainContext
        Get
            Return New ChainContext(_subject, _issuer)
        End Get
    End Property

    ''' <summary>Records the certificate (and issuer, when given) named by a chain event; a different subject starts a fresh context.</summary>
    Friend Sub Update(SubjectRdn As String, IssuerRdn As String)
        Dim subject As String = CommonName(SubjectRdn)
        Dim issuer As String = CommonName(IssuerRdn)
        If Not String.IsNullOrEmpty(subject) Then
            If Not String.Equals(subject, _subject, StringComparison.Ordinal) Then _issuer = Nothing
            _subject = subject
        End If
        If Not String.IsNullOrEmpty(issuer) Then _issuer = issuer
    End Sub

    ''' <summary>Clears the tracked subject and issuer, leaving <see cref="Current"/> empty.</summary>
    Friend Sub Reset()
        _subject = Nothing
        _issuer = Nothing
    End Sub

    ''' <summary>Reinstates a snapshot taken from <see cref="Current"/>; Nothing empties the context.</summary>
    Friend Sub Restore(Context As ChainContext)
        _subject = Context?.SubjectCommonName
        _issuer = Context?.IssuerCommonName
    End Sub

    ''' <summary>CN of a SecureBlackbox slash-style RDN ("/C=HU/O=X/CN=Name" gives "Name"); the input itself when it has no CN.</summary>
    Friend Shared Function CommonName(Rdn As String) As String
        If String.IsNullOrEmpty(Rdn) Then Return Rdn
        Dim cnStart As Integer = Rdn.IndexOf("CN=", StringComparison.OrdinalIgnoreCase)
        If cnStart < 0 Then Return Rdn
        Dim cnEnd As Integer = Rdn.IndexOf("/"c, cnStart)
        Return If(cnEnd < 0, Rdn.Substring(cnStart + 3), Rdn.Substring(cnStart + 3, cnEnd - cnStart - 3))
    End Function
End Class
