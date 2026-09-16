Imports System.Linq
Imports System.Security.Cryptography.X509Certificates

''' <summary>Decides whether a certificate carries the ETSI QcCompliance statement, i.e. is a qualified certificate.</summary>
Friend Module QualifiedCertificateDetector
    Private Const QcStatementsExtensionOid As String = "1.3.6.1.5.5.7.1.3"
    ''' <summary>id-etsi-qcs-QcCompliance (0.4.0.1862.1.1) in DER content form.</summary>
    Private ReadOnly QcComplianceOid As Byte() = {&H4, &H0, &H8E, &H46, &H1, &H1}

    ''' <summary>True when the certificate's QC Statements extension lists QcCompliance.</summary>
    Friend Function IsQualified(Certificate As X509Certificate2) As Boolean
        Dim extension As X509Extension = Certificate.Extensions.Cast(Of X509Extension).FirstOrDefault(Function(x) x.Oid IsNot Nothing AndAlso x.Oid.Value = QcStatementsExtensionOid)
        If extension Is Nothing Then Return False
        Return ContainsQcCompliance(extension.RawData)
    End Function

    ''' <summary>True when the DER QCStatements value (SEQUENCE OF SEQUENCE { statementId, statementInfo }) contains QcCompliance.</summary>
    Friend Function ContainsQcCompliance(QcStatements As Byte()) As Boolean
        Dim offset As Integer = 0
        Dim outer As New DerReader.DerElement()
        If Not DerReader.TryRead(QcStatements, offset, outer) OrElse outer.Tag <> &H30 Then Return False

        For Each statement As DerReader.DerElement In DerReader.Children(outer.Content)
            If statement.Tag <> &H30 Then Continue For
            Dim parts As List(Of DerReader.DerElement) = DerReader.Children(statement.Content)
            If parts.Count > 0 AndAlso DerReader.IsOid(parts(0), QcComplianceOid) Then Return True
        Next
        Return False
    End Function
End Module
