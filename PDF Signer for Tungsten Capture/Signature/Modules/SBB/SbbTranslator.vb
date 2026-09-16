Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon

''' <summary>Maps the provider's integer/enum settings onto SecureBlackbox names and enums, and SecureBlackbox enums onto Hungarian log text.</summary>
Friend Module SbbTranslator
    Private ReadOnly HashNames As New Dictionary(Of Integer, String) From {
        {HashType.SHA1, "SHA1"},
        {HashType.SHA224, "SHA224"},
        {HashType.SHA256, "SHA256"},
        {HashType.SHA384, "SHA384"},
        {HashType.SHA512, "SHA512"}}

    ''' <summary>SecureBlackbox hash algorithm name of a <see cref="HashType"/> value.</summary>
    Friend Function HashAlgorithmName(HashMethod As Integer) As String
        Dim name As String = Nothing
        If HashNames.TryGetValue(HashMethod, name) Then Return name
        Throw New NotSupportedException($"Nem támogatott lenyomatképző algoritmus azonosító: {HashMethod}")
    End Function

    ''' <summary>Provider <see cref="RevocationType"/> mapped onto the PDFSigner component's revocation-check enum.</summary>
    Friend Function RevocationCheck(Protocol As RevocationType) As PDFSignerRevocationChecks
        Select Case Protocol
            Case RevocationType.None : Return PDFSignerRevocationChecks.crcNone
            Case RevocationType.CRL : Return PDFSignerRevocationChecks.crcAnyCRL
            Case RevocationType.OCSP : Return PDFSignerRevocationChecks.crcAnyOCSP
            Case RevocationType.OCSPWithCRLFallback : Return PDFSignerRevocationChecks.crcAnyOCSPOrCRL
            Case Else : Throw New NotSupportedException($"Nem támogatott visszavonás-ellenőrzési típus: {Protocol}")
        End Select
    End Function

    ''' <summary>Same mapping as <see cref="RevocationCheck"/> onto the CertificateValidator enum.</summary>
    Friend Function RevocationCheckForValidator(Protocol As RevocationType) As CertificateValidatorRevocationChecks
        Select Case Protocol
            Case RevocationType.None : Return CertificateValidatorRevocationChecks.crcNone
            Case RevocationType.CRL : Return CertificateValidatorRevocationChecks.crcAnyCRL
            Case RevocationType.OCSP : Return CertificateValidatorRevocationChecks.crcAnyOCSP
            Case RevocationType.OCSPWithCRLFallback : Return CertificateValidatorRevocationChecks.crcAnyOCSPOrCRL
            Case Else : Throw New NotSupportedException($"Nem támogatott visszavonás-ellenőrzési típus: {Protocol}")
        End Select
    End Function

    ''' <summary>Provider <see cref="ProxyAuthenticationMethod"/> mapped onto the SecureBlackbox proxy authentication type.</summary>
    Friend Function ProxyAuth(Method As ProxyAuthenticationMethod) As ProxyAuthTypes
        Select Case Method
            Case ProxyAuthenticationMethod.NoAuthentication : Return ProxyAuthTypes.patNoAuthentication
            Case ProxyAuthenticationMethod.UserPassword : Return ProxyAuthTypes.patBasic
            Case ProxyAuthenticationMethod.Digest : Return ProxyAuthTypes.patDigest
            Case ProxyAuthenticationMethod.NTLM : Return ProxyAuthTypes.patNTLM
            Case Else : Throw New NotSupportedException($"Nem támogatott proxy hitelesítési mód: {Method}")
        End Select
    End Function

    ''' <summary>Log name of a proxy authentication method.</summary>
    Friend Function ProxyAuthName(Method As ProxyAuthenticationMethod) As String
        Select Case Method
            Case ProxyAuthenticationMethod.UserPassword : Return "user/password"
            Case ProxyAuthenticationMethod.Digest : Return "digest"
            Case ProxyAuthenticationMethod.NTLM : Return "NTLM"
            Case Else : Return "nincs"
        End Select
    End Function

    ''' <summary>Hungarian name of a chain element kind reported by the download/needed events (1 certificate, 2 CRL, 3 OCSP).</summary>
    Friend Function ChainKind(Kind As Integer) As String
        Select Case Kind
            Case 1 : Return "tanúsítvány"
            Case 2 : Return "CRL"
            Case 3 : Return "OCSP válasz"
            Case Else : Return $"lánc elem ({Kind})"
        End Select
    End Function

    Private ReadOnly EventKinds As New Dictionary(Of String, String) From {
        {"CertProcessingStarted", "tanúsítvány feldolgozása"},
        {"CertProcessingCompleted", "tanúsítvány feldolgozva"},
        {"CertValidationStarted", "tanúsítvány ellenőrzése"},
        {"CertValidationCompleted", "tanúsítvány ellenőrizve"},
        {"CACertLocated", "CA tanúsítvány megtalálva"},
        {"CRLRetrieved", "CRL letöltve"},
        {"OCSPCheckCompleted", "OCSP ellenőrzés befejezve"},
        {"CertificateValidationBegin", "tanúsítvány ellenőrzése"},
        {"CertificateValidationEnd", "tanúsítvány ellenőrizve"},
        {"CertificateProcessingBegin", "tanúsítvány feldolgozása"},
        {"CertificateProcessingEnd", "tanúsítvány feldolgozva"},
        {"CertificateCRLRevocationCheckBegin", "CRL ellenőrzése"},
        {"CertificateCRLRevocationCheckEnd", "CRL ellenőrzés befejezve"},
        {"CertificateOCSPRevocationCheckBegin", "OCSP ellenőrzés"},
        {"CertificateOCSPRevocationCheckEnd", "OCSP ellenőrzés befejezve"},
        {"CRLValidationBegin", "CRL ellenőrzése"},
        {"CRLValidationEnd", "CRL ellenőrizve"},
        {"OCSPValidationBegin", "OCSP ellenőrzése"},
        {"OCSPValidationEnd", "OCSP ellenőrizve"}}

    ''' <summary>Hungarian name of a chain-validation progress step; unknown steps are returned unchanged because they are log-only.</summary>
    Friend Function EventKind(Kind As String) As String
        Dim text As String = Nothing
        If Kind IsNot Nothing AndAlso EventKinds.TryGetValue(Kind, text) Then Return text
        Return Kind
    End Function

    ''' <summary>Hungarian text of a PDFSigner chain validation result.</summary>
    Friend Function ChainValidity(Validity As ChainValidities) As String
        Select Case Validity
            Case ChainValidities.cvtValid : Return "OK"
            Case ChainValidities.cvtValidButUntrusted : Return "a tanúsítvány érvényes, de nem megbízható"
            Case ChainValidities.cvtInvalid : Return "a tanúsítvány érvénytelen"
            Case ChainValidities.cvtCantBeEstablished : Return "a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig"
            Case Else : Return Validity.ToString()
        End Select
    End Function

    ''' <summary>Hungarian text of a CertificateValidator chain validation result.</summary>
    Friend Function ValidatorChainValidity(Validity As CertificateValidatorChainValidationResults) As String
        Select Case Validity
            Case CertificateValidatorChainValidationResults.cvtValid : Return "OK"
            Case CertificateValidatorChainValidationResults.cvtValidButUntrusted : Return "a tanúsítvány érvényes, de nem megbízható"
            Case CertificateValidatorChainValidationResults.cvtInvalid : Return "a tanúsítvány érvénytelen"
            Case CertificateValidatorChainValidationResults.cvtCantBeEstablished : Return "a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig"
            Case Else : Return Validity.ToString()
        End Select
    End Function

    ''' <summary>Hungarian text of a signature validation result.</summary>
    Friend Function SignatureValidity(Validity As SignatureValidities) As String
        Select Case Validity
            Case SignatureValidities.svtValid : Return "az aláírás érvényes"
            Case SignatureValidities.svtUnknown : Return "az aláírás érvényessége ismeretlen"
            Case SignatureValidities.svtCorrupted : Return "az aláírás sérült"
            Case SignatureValidities.svtSignerNotFound : Return "az aláíró tanúsítvány nem található"
            Case SignatureValidities.svtFailure : Return "az aláírás ellenőrzése meghiúsult"
            Case SignatureValidities.svtReferenceCorrupted : Return "a hivatkozott adat sérült"
            Case Else : Return Validity.ToString()
        End Select
    End Function
End Module
