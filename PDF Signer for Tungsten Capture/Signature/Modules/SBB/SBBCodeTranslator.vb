Imports PDFSignerCommon
Imports SBX509
Imports System.Linq

Friend Class SBBCodeTranslator
    Friend Function GetCertError(code As TSBCertificateValidity) As String
        If code = TSBCertificateValidity.cvInvalid Then Return "a tanúsítvány érvénytelen"
        If code = TSBCertificateValidity.cvChainUnvalidated Then Return "a tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig"
        If code = TSBCertificateValidity.cvSelfSigned Then Return "a tanúsítvány önmagával van aláírva (self-signed)"
        If code = TSBCertificateValidity.cvStorageError Then Return "tanúsítványtár elérési hiba"
        Return "OK"
    End Function

    Friend Function GetCertErrorReasonList(code As Integer) As String
        Dim errList As New List(Of String)

        If (code And SBX509.Unit.vrBadData) = SBX509.Unit.vrBadData Then errList.Add("hibás tanúsítvány-formátum vagy sérült a tanúsítvány")
        If (code And SBX509.Unit.vrRevoked) = SBX509.Unit.vrRevoked Then errList.Add("a tanúsítványt a kibocsájtó visszavonta")
        If (code And SBX509.Unit.vrNotYetValid) = SBX509.Unit.vrNotYetValid Then errList.Add("a tanúsítvány még nem érvényes")
        If (code And SBX509.Unit.vrExpired) = SBX509.Unit.vrExpired Then errList.Add("a tanúsítvány lejárt")
        If (code And SBX509.Unit.vrInvalidSignature) = SBX509.Unit.vrInvalidSignature Then errList.Add("a tanúsítványon lévő aláírás érvénytelen vagy sérült")
        If (code And SBX509.Unit.vrUnknownCA) = SBX509.Unit.vrUnknownCA Then errList.Add("a tanúsítványban szereplő kibocsájtó (CA) tanúsítványa nem megbízható gyökértanúsítvány")
        If (code And SBX509.Unit.vrCAUnauthorized) = SBX509.Unit.vrCAUnauthorized Then errList.Add("a tanúsítványban szereplő kibocsájtó (CA) nem jogosult tanúsítvány kibocsájtására")
        If (code And SBX509.Unit.vrCRLNotVerified) = SBX509.Unit.vrCRLNotVerified Then errList.Add("a visszavonási lista (CRL) nem érhető el vagy nem ellenőrizhető")
        If (code And SBX509.Unit.vrOCSPNotVerified) = SBX509.Unit.vrOCSPNotVerified Then errList.Add("az OCSP válasz nem érhető el vagy nem ellenőrizhető")
        If (code And SBX509.Unit.vrIdentityMismatch) = SBX509.Unit.vrIdentityMismatch Then errList.Add("a tanúsítvány nem a szükséges gépnevet vagy IP címet tartalmazza")
        If (code And SBX509.Unit.vrNoKeyUsage) = SBX509.Unit.vrNoKeyUsage Then errList.Add("a tanúsítvány nem használható aláírásra")
        If (code And SBX509.Unit.vrBlocked) = SBX509.Unit.vrBlocked Then errList.Add("a tanúsítványt visszavonták (blokkolt tanúsítvány)")

        If errList.Count = 0 Then Return String.Empty

        Return ", hiba oka: " + errList.Aggregate(Function(a, b) a & ", " & b)
    End Function

    Friend Function GetCRLError(code As Integer) As String
        If code = 1001 Then Return "hiba a CRL aláírásának ellenőrzésekor"
        If code = 1002 Then Return "a CRL letöltő kliens nem hozható létre vagy nem található"
        If code = 1003 Then Return "hiba a CRL letöltésekor"
        If code = 1004 Then Return "a CRL letöltése nem sikerült a megadott helyről"
        If code = 1005 Then Return "a CRL aláíró tanúsítványát visszavonták"
        Return "-" ' OK
    End Function

    Friend Function GetOCSPError(code As Integer) As String
        If code = 2001 Then Return "hiba az OCSP válasz aláírásának ellenőrzésekor"
        If code = 2002 Then Return "az OCSP kliens nem hozható létre vagy nem található"
        If code = 2003 Then Return "hiba az OCSP válasz letöltésekor"
        If code = 2004 Then Return "hibás OCSP válasz, vagy a válasz hibakódot tartalmaz"
        If code = 2005 Then Return "az OCSP válasz aláíró tanúsítványát visszavonták"
        Return "-" ' OK
    End Function

    Friend Function GetTSPError(code As Integer) As String
        If code = 83969 Then Return "művelet megszakítva"
        If code = 83970 Then Return "nem érkezett válasz az időbélyeg szervertől"
        If code = 83971 Then Return "hiányzó paraméter(ek)"
        If code = 83972 Then Return "hiányzó tanúsítvány"
        If code = 83973 Then Return "hibás adat a válaszban"
        If code = 83974 Then Return "hibás üzenetlenyomat"
        If code = 83975 Then Return "a szervetől hibás NONCE érkezett"
        If code = 83976 Then Return "váratlan tanúsítványok"
        If code = 81921 Then Return "ismeretlen formátum"
        If code = 81922 Then Return "hibás adat (túl hosszú)"
        If code = 81923 Then Return "nem támogatott válaszformátum"
        If code = 81924 Then Return "általános hiba"
        If code = 81925 Then Return "elutasított kérelem"
        Return "-" ' OK
    End Function

    Friend Function GetPKIStatusCode(code As Integer) As String
        If code = 1 Then Return "időbélyeg token létrejött, de módosításokkal"
        If code = 2 Then Return "kérés elutasítva"
        If code = 3 Then Return "kérés felfüggesztve"
        If code = 4 Then Return "a tanúsítvány hamarosan visszavonásra kerül"
        If code = 5 Then Return "a tanúsítvány visszavonásra került"
        If code = 6 Then Return "key update warning"
        Return "időbélyeg token létrejött" ' OK
    End Function

    Friend Function ProxyAuthMethodToString(code As Integer) As String
        If code = ProxyAuthenticationMethod.UserPassword Then Return "user/password"
        If code = ProxyAuthenticationMethod.Digest Then Return "digest"
        If code = ProxyAuthenticationMethod.NTLM Then Return "NTLM"
        Return "nincs"
    End Function
End Class
