Imports PDFSigner

''' <summary>Verifies TlsHandshakeScope's pending/open bookkeeping: a normal handshake unwinds once, and a failed handshake (no Established) still unwinds via Shutdown or an error.</summary>
<TestClass>
Public Class TlsHandshakeScopeTests
    <TestMethod>
    Public Sub Established_then_shutdown_unwinds_exactly_once()
        Dim scope As New TlsHandshakeScope
        Assert.IsTrue(scope.Begin())
        Assert.IsTrue(scope.InProgress)
        Assert.IsTrue(scope.Established())
        Assert.IsFalse(scope.InProgress)
        Assert.IsFalse(scope.Shutdown())
        Assert.IsFalse(scope.InProgress)
    End Sub

    <TestMethod>
    Public Sub A_failed_handshake_unwinds_on_shutdown_with_no_established()
        Dim scope As New TlsHandshakeScope
        Assert.IsTrue(scope.Begin())
        Assert.IsTrue(scope.InProgress)
        Assert.IsTrue(scope.Shutdown())
        Assert.IsFalse(scope.InProgress)
    End Sub

    <TestMethod>
    Public Sub A_failed_handshake_unwinds_on_error_with_no_shutdown()
        Dim scope As New TlsHandshakeScope
        scope.Begin()
        Assert.IsTrue(scope.InProgress)
        Assert.IsTrue(scope.Failed())
        Assert.IsFalse(scope.InProgress)
    End Sub

    <TestMethod>
    Public Sub Nested_handshakes_stay_in_progress_until_all_resolve()
        Dim scope As New TlsHandshakeScope
        Assert.IsTrue(scope.Begin())
        Assert.IsFalse(scope.Begin())
        Assert.IsFalse(scope.Established())
        Assert.IsTrue(scope.InProgress)
        Assert.IsTrue(scope.Established())
        Assert.IsFalse(scope.InProgress)
    End Sub

    <TestMethod>
    Public Sub Nested_handshakes_can_resolve_through_shutdown_only()
        Dim scope As New TlsHandshakeScope
        scope.Begin()
        scope.Begin()
        Assert.IsFalse(scope.Shutdown())
        Assert.IsTrue(scope.InProgress)
        Assert.IsTrue(scope.Shutdown())
        Assert.IsFalse(scope.InProgress)
    End Sub

    <TestMethod>
    Public Sub Reset_clears_pending_and_open_state()
        Dim scope As New TlsHandshakeScope
        scope.Begin()
        scope.Reset()
        Assert.IsFalse(scope.InProgress)
        Assert.IsFalse(scope.Shutdown())
    End Sub

    <TestMethod>
    Public Sub Shutdown_and_failed_are_no_ops_when_nothing_is_pending()
        Dim scope As New TlsHandshakeScope
        Assert.IsFalse(scope.Shutdown())
        Assert.IsFalse(scope.Failed())
    End Sub
End Class
