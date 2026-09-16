Imports PDFSigner

''' <summary>Verifies SbbEventLogger's pure timestamp-line builder: time re-rendering and validity translation.</summary>
<TestClass>
Public Class SbbEventLoggerTests
    <TestMethod>
    Public Sub Timestamp_line_rerenders_the_sbb_time_and_translates_both_validities()
        Dim line As String = SbbEventLogger.TimestampValidatedLine("S0T0", "/C=HU/CN=TSA CA", "2026-09-08 17:18:20", 0, 0)
        StringAssert.StartsWith(line, "S0T0 (kiadó: 'TSA CA') ellenőrizve ")
        StringAssert.Contains(line, "(2026.09.08. 17:18:20 UTC)")
        StringAssert.EndsWith(line, "érvényesség: az aláírás érvényes, lánc érvényesség: OK")
    End Sub

    <TestMethod>
    Public Sub Unparsable_time_is_echoed_in_quotes()
        StringAssert.Contains(SbbEventLogger.TimestampValidatedLine("T0", "/CN=X", "soon", 0, 0), "ellenőrizve 'soon' időpontban")
    End Sub
End Class
