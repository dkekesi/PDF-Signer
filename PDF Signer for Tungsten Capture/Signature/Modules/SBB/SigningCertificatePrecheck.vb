Imports System.Linq
Imports System.Security.Cryptography.X509Certificates

''' <summary>Offline checks of the operator's signing certificate before any SecureBlackbox pass; every failure is a Hungarian operator message.</summary>
Friend Module SigningCertificatePrecheck
    Private Const AuthorityInformationAccessOid As String = "1.3.6.1.5.5.7.1.1"
    ''' <summary>id-ad-ocsp (1.3.6.1.5.5.7.48.1) in DER content form.</summary>
    Private ReadOnly OcspAccessMethodOid As Byte() = {&H2B, &H6, &H1, &H5, &H5, &H7, &H30, &H1}

    ''' <summary>Returns the first failing check's message, or Nothing when the certificate can sign now.</summary>
    Friend Function Check(Certificate As X509Certificate2, RequireOcspResponder As Boolean) As String
        Dim name As String = CommonName(Certificate)
        If Certificate.NotBefore > Date.Now Then Return $"'{name}' tanúsítvány csak {Certificate.NotBefore} után érvényes!"
        If Certificate.NotAfter < Date.Now Then Return $"'{name}' tanúsítvány már nem érvényes (lejárt: {Certificate.NotAfter})!"
        If Not Certificate.HasPrivateKey Then Return $"'{name}' tanúsítvány nem tartalmaz privát kulcsot! Privát kulcs nélkül nem lehet aláírást létrehozni!"
        If Not HasSigningKeyUsage(Certificate) Then Return $"'{name}' tanúsítvány nem használható elektronikus aláírás készítésére (nincs beállítva a 'Digital Signature' vagy 'Non Repudiation' flag)!"
        If Not ChainIsComplete(Certificate) Then Return $"'{name}' tanúsítványhoz tartozó tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig!"
        If RequireOcspResponder AndAlso Not HasOcspResponder(Certificate) Then Return $"'{name}' tanúsítvány visszavonási állapota nem ellenőrizhető OCSP-vel, de a dokumentumtípus beállításai megkövetelik!"
        Return Nothing
    End Function

    ''' <summary>Subject common name, or the whole subject when it has none.</summary>
    Friend Function CommonName(Certificate As X509Certificate2) As String
        Try
            Dim cn As String = Certificate.GetNameInfo(X509NameType.SimpleName, False)
            Return If(String.IsNullOrEmpty(cn), Certificate.Subject, cn)
        Catch
            Return Certificate.Subject
        End Try
    End Function

    ''' <summary>True when a KeyUsage extension is present and allows DigitalSignature or NonRepudiation; an absent extension does not qualify.</summary>
    Friend Function HasSigningKeyUsage(Certificate As X509Certificate2) As Boolean
        For Each extension As X509Extension In Certificate.Extensions
            Dim usage As X509KeyUsageExtension = TryCast(extension, X509KeyUsageExtension)
            If usage IsNot Nothing Then
                Return (usage.KeyUsages And X509KeyUsageFlags.DigitalSignature) <> 0 OrElse
                       (usage.KeyUsages And X509KeyUsageFlags.NonRepudiation) <> 0
            End If
        Next
        Return False
    End Function

    ''' <summary>True when the Windows stores let the chain reach a trusted root; time and revocation findings are ignored here.</summary>
    Friend Function ChainIsComplete(Certificate As X509Certificate2) As Boolean
        Using chain As New X509Chain()
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid Or
                                                  X509VerificationFlags.IgnoreCtlNotTimeValid Or
                                                  X509VerificationFlags.IgnoreNotTimeNested
            chain.Build(Certificate)
            For Each status As X509ChainStatus In chain.ChainStatus
                If (status.Status And X509ChainStatusFlags.PartialChain) <> 0 OrElse
                   (status.Status And X509ChainStatusFlags.UntrustedRoot) <> 0 OrElse
                   (status.Status And X509ChainStatusFlags.NotSignatureValid) <> 0 Then
                    Return False
                End If
            Next
            Return True
        End Using
    End Function

    ''' <summary>True when any Authority Information Access extension names an OCSP responder.</summary>
    Friend Function HasOcspResponder(Certificate As X509Certificate2) As Boolean
        For Each extension As X509Extension In Certificate.Extensions
            If extension.Oid IsNot Nothing AndAlso extension.Oid.Value = AuthorityInformationAccessOid AndAlso AiaContainsOcsp(extension.RawData) Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>True when the DER AuthorityInfoAccessSyntax (SEQUENCE OF SEQUENCE { accessMethod, accessLocation }) has an id-ad-ocsp entry.</summary>
    Friend Function AiaContainsOcsp(Aia As Byte()) As Boolean
        Dim offset As Integer = 0
        Dim outer As New DerReader.DerElement()
        If Not DerReader.TryRead(Aia, offset, outer) OrElse outer.Tag <> &H30 Then Return False

        For Each description As DerReader.DerElement In DerReader.Children(outer.Content)
            If description.Tag <> &H30 Then Continue For
            Dim parts As List(Of DerReader.DerElement) = DerReader.Children(description.Content)
            If parts.Count > 0 AndAlso DerReader.IsOid(parts(0), OcspAccessMethodOid) Then Return True
        Next
        Return False
    End Function
End Module
