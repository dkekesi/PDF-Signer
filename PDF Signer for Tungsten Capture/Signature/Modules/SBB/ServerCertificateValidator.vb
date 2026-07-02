Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports SBX509

''' <summary>
''' Validates TLS server certificates against the Windows trust store
''' (chain building + hostname matching).
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
        Dim hasSan As Boolean
        Dim names As List(Of String) = GetSubjectAlternativeDnsNames(Certificate, hasSan)

        If Not hasSan Then
            ' RFC 6125: fall back to the subject CN only when the certificate has no SAN extension at all
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
            Dim base As String = Pattern.Substring(2)
            If base.IndexOf("."c) < 0 Then Return False ' reject single-label bases like "*.com"

            Dim dotIndex As Integer = Host.IndexOf("."c)
            If dotIndex > 0 Then
                Return String.Equals(base, Host.Substring(dotIndex + 1), StringComparison.OrdinalIgnoreCase)
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Extracts dNSName entries from the SAN extension (OID 2.5.29.17) at the DER
    ''' level, independent of OS display language.
    ''' </summary>
    Private Shared Function GetSubjectAlternativeDnsNames(Certificate As X509Certificate2, ByRef HasSanExtension As Boolean) As List(Of String)
        Dim res As New List(Of String)
        HasSanExtension = False

        For Each ext As X509Extension In Certificate.Extensions
            If ext.Oid Is Nothing OrElse ext.Oid.Value <> "2.5.29.17" Then Continue For
            HasSanExtension = True

            Dim data As Byte() = ext.RawData
            ' DER SEQUENCE of GeneralName; dNSName is context tag [2] = &H82 (IA5String)
            Dim pos As Integer = 0
            Dim seqLen As Integer
            If Not ReadTagAndLength(data, pos, &H30, seqLen) Then Continue For

            Dim seqEnd As Integer = pos + seqLen
            While pos < seqEnd
                Dim tag As Byte = data(pos)
                Dim len As Integer
                ' the tag byte was already read into "tag"; the call only validates and consumes the header
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
            ' high bit set: the low 7 bits give the count of following big-endian length bytes
            Dim byteCount As Integer = first And &H7F
            If byteCount = 0 OrElse byteCount > 4 OrElse Position + byteCount > Data.Length Then Return False
            For i As Integer = 1 To byteCount
                Length = (Length << 8) Or Data(Position)
                Position += 1
            Next
        End If

        Return Length >= 0 AndAlso Position + Length <= Data.Length
    End Function

End Class

''' <summary>
''' A host-bound TLS certificate validator for the OCSP/CRL download HTTP clients.
''' (Lambdas with ByRef parameters cannot be used as event handlers, hence the
''' separate class.)
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
            Reason = Reason Or SBX509.__Global.vrUnknownCA
            _log.AppendLine($"  TLS tanúsítvány hiba ({_targetHost}): {failureReason}")
        End If
    End Sub
End Class
