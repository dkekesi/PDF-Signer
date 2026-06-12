Imports System.Security.Cryptography.X509Certificates
Imports nsoftware.SecureBlackbox
Imports SBX509

Friend Class CertificateTranslator
    Friend Function GetIssuedToName(cert As TElX509Certificate) As String
        Dim res As String = cert.SubjectName.CommonName
        If String.IsNullOrEmpty(res) Then
            res = cert.SubjectName.OrganizationUnit
        End If
        If String.IsNullOrEmpty(res) Then
            res = cert.SubjectName.Organization
        End If
        Return res
    End Function

    Friend Function GetIssuedToName(cert As X509Certificate2) As String
        Dim res As String = String.Empty

        Dim items As String() = cert.Subject.Split(",")

        For Each item As String In items
            item = item.Trim
            If item.StartsWith("CN=") Then
                res = item.Substring(3, item.Length - 3)
            End If
            If String.IsNullOrEmpty(res) AndAlso item.StartsWith("OU=") Then
                res = item.Substring(3, item.Length - 3)
            End If
            If String.IsNullOrEmpty(res) AndAlso item.StartsWith("O=") Then
                res = item.Substring(2, item.Length - 2)
            End If

            If Not String.IsNullOrEmpty(res) Then Exit For
        Next

        If String.IsNullOrEmpty(res) Then res = cert.Subject

        Return res
    End Function

    Friend Function GetIssuerName(cert As TElX509Certificate) As String
        Dim res As String = cert.IssuerName.CommonName
        If String.IsNullOrEmpty(res) Then
            res = cert.IssuerName.OrganizationUnit
        End If
        If String.IsNullOrEmpty(res) Then
            res = cert.IssuerName.Organization
        End If
        Return res
    End Function

End Class
