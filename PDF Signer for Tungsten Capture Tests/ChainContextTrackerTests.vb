Imports PDFSigner

''' <summary>Verifies ChainContextTracker's CN extraction and its subject/issuer tracking across chain events.</summary>
<TestClass>
Public Class ChainContextTrackerTests
    <TestMethod>
    Public Sub Common_name_is_cut_out_of_a_slash_rdn()
        Assert.AreEqual("Name", ChainContextTracker.CommonName("/C=HU/O=DocSoft/CN=Name"))
        Assert.AreEqual("Name", ChainContextTracker.CommonName("/CN=Name/O=DocSoft"))
        Assert.AreEqual("/O=NoCn", ChainContextTracker.CommonName("/O=NoCn"))
        Assert.IsNull(ChainContextTracker.CommonName(Nothing))
    End Sub

    <TestMethod>
    Public Sub A_new_subject_drops_the_old_issuer_and_an_empty_issuer_keeps_it()
        Dim t As New ChainContextTracker
        t.Update("/CN=Leaf", "/CN=CA")
        Assert.AreEqual("CA", t.Current.IssuerCommonName)
        t.Update("/CN=Leaf", Nothing)
        Assert.AreEqual("CA", t.Current.IssuerCommonName)
        t.Update("/CN=Other", Nothing)
        Assert.AreEqual("Other", t.Current.SubjectCommonName)
        Assert.IsNull(t.Current.IssuerCommonName)
        t.Reset()
        Assert.IsTrue(t.Current.IsEmpty)
        t.Restore(New ChainContext("S", "I"))
        Assert.AreEqual("I", t.Current.IssuerCommonName)
    End Sub
End Class
