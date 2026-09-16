Imports PDFSigner

''' <summary>Verifies MomentFormat's local/UTC log rendering and the compact single-half display.</summary>
<TestClass>
Public Class MomentFormatTests
    <TestMethod>
    Public Sub Log_form_carries_local_then_utc()
        Dim text As String = MomentFormat.Log(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero))
        StringAssert.EndsWith(text, "(2026.09.08. 17:18:20 UTC)")
        StringAssert.Matches(text, New Text.RegularExpressions.Regex("^\d{4}\.\d{2}\.\d{2}\. \d{2}:\d{2}:\d{2} \("))
    End Sub

    <TestMethod>
    Public Sub Display_utc_is_the_compact_form()
        Assert.AreEqual("2026.09.08. 17:18:20 UTC", MomentFormat.Display(New DateTimeOffset(2026, 9, 8, 17, 18, 20, TimeSpan.Zero), Utc:=True))
        Assert.AreEqual(TimeSpan.Zero, MomentFormat.FromUtc(New Date(2026, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)).Offset)
    End Sub
End Class
