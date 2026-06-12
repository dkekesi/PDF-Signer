Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports SBX509

''' <summary>
''' TLS szerver tanúsítványok ellenőrzése a Windows tanúsítványtár alapján
''' (lánc felépítése + hosztnév egyezés).
''' </summary>
Friend Class ServerCertificateValidator

    ''' <summary>
    ''' Validates a TLS server certificate: chain to the Windows trust store plus
    ''' hostname match. CA certificates (chain members presented during the handshake)
    ''' only need a valid chain.
    ''' </summary>
    Friend Shared Function Validate(Certificate As TElX509Certificate, TargetHost As String, ByRef FailureReason As String) As Boolean
        FailureReason = Nothing

        If String.IsNullOrEmpty(TargetHost) Then
            FailureReason = "nincs megadva cél kiszolgálónév"
            Return False
        End If

        Dim winCert As X509Certificate2
        Try
            winCert = Certificate.ToX509Certificate2(False)
        Catch ex As Exception
            FailureReason = $"a tanúsítvány nem konvertálható: {ex.Message}"
            Return False
        End Try

        ' chain validation against the Windows trust store
        ' (revocation checking at the TLS layer is intentionally off; document-level
        ' revocation checking is governed separately by the provider settings)
        Using chain As New X509Chain()
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck
            If Not chain.Build(winCert) Then
                Dim sb As New StringBuilder
                For Each status As X509ChainStatus In chain.ChainStatus
                    If sb.Length > 0 Then sb.Append(", ")
                    sb.Append(status.StatusInformation.Trim())
                Next
                FailureReason = $"a tanúsítványlánc nem érvényes: {sb}"
                Return False
            End If
        End Using

        ' the TLS event fires for every certificate of the server's chain;
        ' hostname matching only applies to the leaf (non-CA) certificate
        If IsCertificateAuthority(winCert) Then Return True

        If Not MatchesHost(winCert, TargetHost) Then
            FailureReason = $"a tanúsítvány nem a(z) '{TargetHost}' kiszolgálóhoz tartozik"
            Return False
        End If

        Return True
    End Function

    Private Shared Function IsCertificateAuthority(Certificate As X509Certificate2) As Boolean
        For Each ext As X509Extension In Certificate.Extensions
            Dim basicConstraints As X509BasicConstraintsExtension = TryCast(ext, X509BasicConstraintsExtension)
            If basicConstraints IsNot Nothing Then
                Return basicConstraints.CertificateAuthority
            End If
        Next
        Return False
    End Function

    Private Shared Function MatchesHost(Certificate As X509Certificate2, TargetHost As String) As Boolean
        Dim names As List(Of String) = GetSubjectAlternativeDnsNames(Certificate)

        If names.Count = 0 Then
            ' no SAN extension: fall back to the subject name (CN)
            Dim cn As String = Certificate.GetNameInfo(X509NameType.DnsName, False)
            If Not String.IsNullOrEmpty(cn) Then names.Add(cn)
        End If

        For Each name As String In names
            If HostNameMatches(name, TargetHost) Then Return True
        Next

        Return False
    End Function

    Private Shared Function HostNameMatches(Pattern As String, Host As String) As Boolean
        If String.Equals(Pattern, Host, StringComparison.OrdinalIgnoreCase) Then Return True

        ' wildcard: "*.example.com" matches exactly one leading label
        If Pattern.StartsWith("*.", StringComparison.Ordinal) Then
            Dim dotIndex As Integer = Host.IndexOf("."c)
            If dotIndex > 0 Then
                Return String.Equals(Pattern.Substring(2), Host.Substring(dotIndex + 1), StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Extracts dNSName entries from the SAN extension (OID 2.5.29.17) at the DER
    ''' level, independent of OS display language.
    ''' </summary>
    Private Shared Function GetSubjectAlternativeDnsNames(Certificate As X509Certificate2) As List(Of String)
        Dim res As New List(Of String)

        For Each ext As X509Extension In Certificate.Extensions
            If ext.Oid Is Nothing OrElse ext.Oid.Value <> "2.5.29.17" Then Continue For

            Dim data As Byte() = ext.RawData
            ' DER SEQUENCE of GeneralName; dNSName is context tag [2] = &H82 (IA5String)
            Dim pos As Integer = 0
            Dim seqLen As Integer
            If Not ReadTagAndLength(data, pos, &H30, seqLen) Then Continue For

            Dim seqEnd As Integer = pos + seqLen
            While pos < seqEnd
                Dim tag As Byte = data(pos)
                Dim len As Integer
                If Not ReadTagAndLength(data, pos, tag, len) Then Exit While

                If tag = &H82 Then
                    res.Add(Encoding.ASCII.GetString(data, pos, len))
                End If
                pos += len
            End While
        Next

        Return res
    End Function

    ''' <summary>
    ''' Reads a DER tag + length header; on success Position points at the content.
    ''' </summary>
    Private Shared Function ReadTagAndLength(Data As Byte(), ByRef Position As Integer, ExpectedTag As Byte, ByRef Length As Integer) As Boolean
        Length = 0
        If Position >= Data.Length OrElse Data(Position) <> ExpectedTag Then Return False
        Position += 1

        If Position >= Data.Length Then Return False
        Dim first As Byte = Data(Position)
        Position += 1

        If first < &H80 Then
            Length = first
        Else
            Dim byteCount As Integer = first And &H7F
            If byteCount = 0 OrElse byteCount > 4 OrElse Position + byteCount > Data.Length Then Return False
            For i As Integer = 1 To byteCount
                Length = (Length << 8) Or CInt(Data(Position))
                Position += 1
            Next
        End If

        Return Length >= 0 AndAlso Position + Length <= Data.Length
    End Function

End Class

''' <summary>
''' Egy adott kiszolgálóhoz kötött TLS tanúsítvány-ellenőrző az OCSP/CRL letöltő
''' HTTP kliensekhez. (Lambdák ByRef paraméterekkel nem használhatók eseménykezelőként,
''' ezért kell külön osztály.)
''' </summary>
Friend Class HostBoundCertificateValidator
    Private ReadOnly _targetHost As String
    Private ReadOnly _log As StringBuilder

    Friend Sub New(TargetHost As String, Log As StringBuilder)
        _targetHost = TargetHost
        _log = Log
    End Sub

    Friend Sub OnCertificateValidate(Sender As Object, X509Certificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        Dim failureReason As String = Nothing
        If ServerCertificateValidator.Validate(X509Certificate, _targetHost, failureReason) Then
            Validity = TSBCertificateValidity.cvOk
        Else
            Validity = TSBCertificateValidity.cvInvalid
            _log.AppendLine($"  TLS tanúsítvány hiba ({_targetHost}): {failureReason}")
        End If
    End Sub
End Class
