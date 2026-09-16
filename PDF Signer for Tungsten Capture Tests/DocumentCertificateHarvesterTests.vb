Imports PDFSigner

''' <summary>Verifies the parse-only document probe/harvest and that the Windows Root store yields trust anchors.</summary>
<TestClass>
Public Class DocumentCertificateHarvesterTests
    <TestMethod>
    Public Sub Probe_reports_an_unsigned_document_as_unencrypted_with_no_entities()
        Dim document As New MemoryTributary(TestPdf.Minimal())
        Dim encrypted As Boolean
        Dim entityCount As Integer
        DocumentCertificateHarvester.Probe(document, encrypted, entityCount)
        Assert.IsFalse(encrypted)
        Assert.AreEqual(0, entityCount)
    End Sub

    <TestMethod>
    Public Sub Harvest_of_an_unsigned_document_returns_empty_lists_and_no_labels()
        Dim document As New MemoryTributary(TestPdf.Minimal())
        Dim harvest As HarvestedMaterial = DocumentCertificateHarvester.Harvest(document)
        Assert.AreEqual(0, harvest.CertificateBytes.Count)
        Assert.AreEqual(0, harvest.CrlBytes.Count)
        Assert.AreEqual(0, harvest.OcspBytes.Count)
        Assert.IsNull(harvest.LastSignatureEntityLabel)
        Assert.IsNull(harvest.SignatureTimestampCertificateBytes)
    End Sub

    <TestMethod>
    Public Sub Windows_root_store_loads_at_least_one_trusted_root()
        Dim stores As WindowsCertificateStores = WindowsCertificateStores.Load()
        Assert.IsTrue(stores.TrustedRoots.Count > 0)
    End Sub
End Class
