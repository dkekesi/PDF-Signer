Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

''' <summary>Self-signed test certificates built with CertificateRequest (available from .NET Framework 4.7.2).</summary>
Friend Module TestCertificates
    Friend Function SelfSigned(subject As String, signing As Boolean, notAfter As Date, Optional notBefore As Date? = Nothing) As X509Certificate2
        Using rsa As RSA = RSA.Create(2048)
            Dim request As New CertificateRequest(subject, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            Dim usage As X509KeyUsageFlags = If(signing, X509KeyUsageFlags.DigitalSignature, X509KeyUsageFlags.KeyEncipherment)
            request.CertificateExtensions.Add(New X509KeyUsageExtension(usage, critical:=True))
            Dim start As Date = If(notBefore, Date.Now.AddDays(-1))
            Dim cert As X509Certificate2 = request.CreateSelfSigned(New DateTimeOffset(start), New DateTimeOffset(notAfter))
            ' Re-import so the private key is attached in the persisted form SecureBlackbox and X509Chain expect.
            Return New X509Certificate2(cert.Export(X509ContentType.Pfx, "t"), "t", X509KeyStorageFlags.Exportable)
        End Using
    End Function
End Module
