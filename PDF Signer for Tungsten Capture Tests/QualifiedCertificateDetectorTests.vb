Imports PDFSigner

''' <summary>Verifies detection of the ETSI QcCompliance statement in the QC Statements certificate extension.</summary>
<TestClass>
Public Class QualifiedCertificateDetectorTests
    ' QCStatements ::= SEQUENCE OF QCStatement; second statement is id-etsi-qcs-QcCompliance (0.4.0.1862.1.1).
    Private Shared ReadOnly WithCompliance As Byte() = {
        &H30, &H12,
        &H30, &H6, &H6, &H4, &H2A, &H3, &H4, &H5,
        &H30, &H8, &H6, &H6, &H4, &H0, &H8E, &H46, &H1, &H1}

    Private Shared ReadOnly WithoutCompliance As Byte() = {
        &H30, &H8,
        &H30, &H6, &H6, &H4, &H2A, &H3, &H4, &H5}

    <TestMethod>
    Public Sub Detects_the_QcCompliance_statement()
        Assert.IsTrue(QualifiedCertificateDetector.ContainsQcCompliance(WithCompliance))
    End Sub

    <TestMethod>
    Public Sub Other_statements_do_not_count()
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(WithoutCompliance))
    End Sub

    <TestMethod>
    Public Sub Malformed_or_empty_input_is_not_qualified()
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(New Byte() {}))
        Assert.IsFalse(QualifiedCertificateDetector.ContainsQcCompliance(New Byte() {&H30, &H9, &H1}))
    End Sub

    <TestMethod>
    Public Sub Certificate_without_the_extension_is_not_qualified()
        Using cert = TestCertificates.SelfSigned("CN=Plain", signing:=True, notAfter:=Date.Now.AddYears(1))
            Assert.IsFalse(QualifiedCertificateDetector.IsQualified(cert))
        End Using
    End Sub
End Class
