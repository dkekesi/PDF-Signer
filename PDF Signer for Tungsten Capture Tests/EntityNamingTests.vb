Imports PDFSigner

''' <summary>Verifies EntityNaming's display id assignment for signatures, document timestamps and signature timestamps.</summary>
<TestClass>
Public Class EntityNamingTests
    <TestMethod>
    Public Sub Signatures_document_timestamps_and_own_timestamps_get_display_ids()
        Dim naming = EntityNaming.Build(
            {New EntityNaming.EntityDescriptor("Signature1", False), New EntityNaming.EntityDescriptor("DocTS1", True)},
            {New EntityNaming.TimestampDescriptor("TS1", "Signature1"), New EntityNaming.TimestampDescriptor("TS2", "DocTS1")})
        Assert.AreEqual("S0", naming.Display("Signature1"))
        Assert.AreEqual("T0", naming.Display("DocTS1"))
        Assert.AreEqual("S0T0", naming.Display("TS1"))
        Assert.AreEqual("T0", naming.Display("TS2"))
        Assert.AreEqual("S0T1", naming.DisplayTimestamp("Signature1", 1))
        Assert.AreEqual("Unknown", naming.Display("Unknown"))
        Assert.IsNull(naming.Display(Nothing))
    End Sub

    <TestMethod>
    Public Sub Entity_key_falls_back_to_the_document_order()
        Assert.AreEqual("Sig", EntityNaming.EntityKey("Sig", 3, False))
        Assert.AreEqual("S3", EntityNaming.EntityKey("", 3, False))
        Assert.AreEqual("DT2", EntityNaming.EntityKey(Nothing, 2, True))
        Assert.AreEqual("x", EntityNaming.Empty.Display("x"))
    End Sub
End Class
