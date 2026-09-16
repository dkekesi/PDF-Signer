''' <summary>Tracks nested TLS handshakes for the signature log: a handshake stays pending until OnTLSEstablished succeeds it or OnTLSShutdown/OnError ends it unresolved; unwinding an already-established connection on Shutdown is a no-op, so a normal pair unwinds exactly once.</summary>
Friend NotInheritable Class TlsHandshakeScope
    Private _pending As Integer
    Private _open As Integer

    ''' <summary>True while at least one handshake has started but not yet resolved; chain events are demoted to trace during this time.</summary>
    Friend ReadOnly Property InProgress As Boolean
        Get
            Return _pending > 0
        End Get
    End Property

    ''' <summary>Clears all tracked handshakes and open connections.</summary>
    Friend Sub Reset()
        _pending = 0
        _open = 0
    End Sub

    ''' <summary>Records a handshake starting; returns True the first time (none was already pending), when the caller should snapshot state to restore later.</summary>
    Friend Function Begin() As Boolean
        Dim first As Boolean = _pending = 0
        _pending += 1
        Return first
    End Function

    ''' <summary>Records a handshake completing successfully, moving it from pending to open; returns True when that was the last pending handshake.</summary>
    Friend Function Established() As Boolean
        Dim wasPending As Boolean = _pending > 0
        If wasPending Then _pending -= 1
        _open += 1
        Return wasPending AndAlso _pending = 0
    End Function

    ''' <summary>Records a connection shutting down: closes an open (already-established) connection with no further unwind, or ends a still-pending handshake that never reached Established; returns True when that was the last pending handshake.</summary>
    Friend Function Shutdown() As Boolean
        If _open > 0 Then
            _open -= 1
            Return False
        End If
        If _pending = 0 Then Return False
        _pending -= 1
        Return _pending = 0
    End Function

    ''' <summary>Records an error ending a still-pending handshake; returns True when that was the last pending handshake.</summary>
    Friend Function Failed() As Boolean
        If _pending = 0 Then Return False
        _pending -= 1
        Return _pending = 0
    End Function
End Class
