Imports System.IO
Imports nsoftware.SecureBlackbox
Imports PDFSigner
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

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

    <TestMethod>
    Public Sub Probe_reports_a_password_encrypted_document_as_encrypted()
        Dim outputStream As New MemoryStream()
        Using encryptor As New PDFEncryptor With {.RuntimeLicense = SbbLicense.Key}
            encryptor.InputBytes = TestPdf.Minimal()
            encryptor.OutputStream = outputStream
            encryptor.UserPassword = "secret"
            encryptor.Encrypt()
        End Using
        Dim document As New MemoryTributary(outputStream.ToArray())
        Dim encrypted As Boolean
        Dim entityCount As Integer
        DocumentCertificateHarvester.Probe(document, encrypted, entityCount)
        Assert.IsTrue(encrypted)
    End Sub

    ''' <summary>SecureBlackbox component disposal (CertificateValidator, PDFSigner) does not cascade-dispose certificates only assigned to it; a shared list stays usable across passes.</summary>
    <TestMethod>
    Public Sub Disposing_a_validator_and_a_signer_leaves_a_shared_certificate_list_usable()
        Dim stores As WindowsCertificateStores = WindowsCertificateStores.Load()
        Dim subjectBefore As String = stores.TrustedRoots(0).SubjectRDN
        Using validator As CertificateValidator = SbbLicense.CreateCertificateValidator()
            validator.TrustedCertificates = stores.TrustedRoots
            validator.KnownCertificates = stores.IntermediateCertificates
        End Using
        Using signer As SbbPdfSigner = SbbLicense.CreateSigner()
            signer.KnownCertificates = stores.TrustedRoots
        End Using
        Assert.AreEqual(subjectBefore, stores.TrustedRoots(0).SubjectRDN)
        Assert.IsTrue(stores.TrustedRoots(0).Bytes.Length > 0)
    End Sub

    <TestMethod>
    Public Sub CopyOf_returns_distinct_certificates_with_equal_bytes_and_does_not_mutate_the_input()
        Dim stores As WindowsCertificateStores = WindowsCertificateStores.Load()
        Dim originalCount As Integer = stores.TrustedRoots.Count
        Dim copy As CertificateList = WindowsCertificateStores.CopyOf(stores.TrustedRoots)
        Assert.AreEqual(originalCount, copy.Count)
        Assert.AreNotSame(stores.TrustedRoots(0), copy(0))
        CollectionAssert.AreEqual(stores.TrustedRoots(0).Bytes, copy(0).Bytes)
        Assert.AreEqual(originalCount, stores.TrustedRoots.Count)
    End Sub

    <TestMethod>
    Public Sub BuildKnownCertificates_combines_fresh_copies_of_store_and_harvested_certificates()
        Dim stores As WindowsCertificateStores = WindowsCertificateStores.Load()
        Dim originalStoreCount As Integer = stores.TrustedRoots.Count
        Dim material As New HarvestedMaterial
        material.CertificateBytes.Add(stores.TrustedRoots(0).Bytes)

        Dim combined As CertificateList = material.BuildKnownCertificates(stores.TrustedRoots)

        Assert.AreEqual(originalStoreCount + 1, combined.Count)
        Assert.AreNotSame(stores.TrustedRoots(0), combined(0))
        CollectionAssert.AreEqual(stores.TrustedRoots(0).Bytes, combined(0).Bytes)
        Assert.AreEqual(originalStoreCount, stores.TrustedRoots.Count)
    End Sub
End Class
