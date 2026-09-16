Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>Turns a SecureBlackbox error (code + description) or exception into one Hungarian log line; raw codes stay in a trailing "[SBB …]" suffix.</summary>
Friend Module SbbErrorTranslator
    ''' <summary>The two revocation templates SecureBlackbox formats: "OCSP error 2002 (location: http://…)" and "CRL error 1001 (location: )".</summary>
    Private ReadOnly RevocationTemplate As New Regex(
        "^\s*(?<family>OCSP|CRL)\s+error\s+(?<sub>[0-9]{1,9})\s*\(location:\s*(?<loc>[^)]*)\)\s*$",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

    Private ReadOnly OcspSubCodes As New Dictionary(Of Integer, String) From {
        {2001, "a kapott OCSP válasz elutasítva (a válaszadó tanúsítványlánca nem megbízható, vagy az aláírása érvénytelen)"},
        {2002, "nincs használható OCSP válasz ehhez a válaszadóhoz (offline módban: nincs megfelelő beágyazott válasz)"},
        {2003, "az OCSP válaszadó nem érhető el vagy elutasította a kérést"},
        {2004, "egyetlen OCSP válaszadó sem adott elfogadható választ"},
        {2005, "az OCSP válaszadó tanúsítványát visszavonták"}}

    Private ReadOnly CrlSubCodes As New Dictionary(Of Integer, String) From {
        {1001, "a CRL elutasítva (a kibocsátó lánca nem megbízható, vagy az aláírása érvénytelen)"},
        {1002, "nincs használható CRL ehhez a kibocsátóhoz (offline módban: nincs megfelelő beágyazott CRL)"},
        {1003, "a CRL nem tölthető le erről a helyről"},
        {1004, "egyetlen CRL elosztási pont sem adott használható CRL-t"},
        {1005, "a CRL aláírójának tanúsítványát visszavonták"}}

    ''' <summary>Module-level wrapper codes: the general SB_ERROR_* set, the PDF set, the PAdES chain failure and the validator revocation wrapper.</summary>
    Private ReadOnly WrapperCodes As New Dictionary(Of Integer, String) From {
        {1048577, "érvénytelen paraméter"},
        {1048578, "érvénytelen beállítás"},
        {1048579, "érvénytelen állapot"},
        {1048580, "érvénytelen érték"},
        {1048581, "a privát kulcs nem található"},
        {1048582, "a felhasználó megszakította"},
        {1048583, "a fájl nem található"},
        {1048584, "nem támogatott funkció vagy művelet"},
        {1048585, "általános hiba"},
        {26214401, "a bemeneti fájl nem létezik"},
        {26214402, "a fájl már titkosított"},
        {26214403, "a fájl nincs titkosítva"},
        {26214405, "érvénytelen jelszó"},
        {26214406, "a fájl visszafejtése sikertelen"},
        {26214407, "a dokumentum már alá van írva"},
        {26214408, "a dokumentum nincs aláírva"},
        {26214409, "ez az aláírástípus nem frissíthető"},
        {26214410, "nem támogatott PDF funkció vagy művelet"},
        {26214411, "nincs megadva időbélyeg szerver"},
        {26214412, "a komponens nincs szerkesztő módban"},
        {721417, "a tanúsítványlánc ellenőrzése sikertelen (PAdES)"},
        {28311556, "tanúsítványlánc-ellenőrzési hiba"},
        {28311557, "tanúsítványlánc visszavonás-ellenőrzési hiba"}}

    ''' <summary>Builds the log line; EntityPrefix is prepended when given. A non-template description omits the certificate clause when the context has no subject.</summary>
    Friend Function Describe(ErrorCode As Integer, Description As String, Context As ChainContext, Optional EntityPrefix As String = Nothing) As String
        Dim sb As New StringBuilder
        If Not String.IsNullOrEmpty(EntityPrefix) Then sb.Append(EntityPrefix).Append(": ")
        sb.Append(WrapperText(ErrorCode)).Append(": ")

        Dim m As Match = RevocationTemplate.Match(If(Description, String.Empty))
        If m.Success Then
            Dim ocsp As Boolean = m.Groups("family").Value.Equals("OCSP", StringComparison.OrdinalIgnoreCase)
            Dim subCode As Integer = Integer.Parse(m.Groups("sub").Value, CultureInfo.InvariantCulture)
            Dim location As String = m.Groups("loc").Value.Trim()
            Dim table As Dictionary(Of Integer, String) = If(ocsp, OcspSubCodes, CrlSubCodes)
            Dim text As String = Nothing
            sb.Append(If(table.TryGetValue(subCode, text), text, If(ocsp, "ismeretlen OCSP hiba", "ismeretlen CRL hiba")))
            sb.Append(" "c).Append(CertificateClause(Context)).Append(", ")
            If location.Length = 0 Then
                sb.Append(If(ocsp, "válaszadó: nincs megadva (már meglévő válasz)", "elosztási pont: nincs megadva (már meglévő CRL)"))
            Else
                sb.Append(If(ocsp, "válaszadó: ", "elosztási pont: ")).Append(StripUserInfo(location))
            End If
            sb.Append(" [SBB ").Append(ErrorCode).Append(If(ocsp, " / OCSP ", " / CRL ")).Append(subCode).Append("]"c)
        Else
            sb.Append(If(String.IsNullOrWhiteSpace(Description), "(nincs leírás)", Description.Trim()))
            If Context IsNot Nothing AndAlso Not String.IsNullOrEmpty(Context.SubjectCommonName) Then
                sb.Append(" – ").Append(CertificateClause(Context))
            End If
            sb.Append(" [SBB ").Append(ErrorCode).Append("]"c)
        End If
        Return sb.ToString()
    End Function

    ''' <summary>Hungarian text of a wrapper code; unknown codes are decoded into module (high 16 bits) and sub-error (low 16 bits).</summary>
    Friend Function WrapperText(ErrorCode As Integer) As String
        Dim text As String = Nothing
        If WrapperCodes.TryGetValue(ErrorCode, text) Then Return text
        Dim moduleId As Integer = (ErrorCode >> 16) And &HFFFF
        Dim subError As Integer = ErrorCode And &HFFFF
        Return $"ismeretlen hiba (modul 0x{moduleId:X}, hiba {subError})"
    End Function

    ''' <summary>Hungarian clause naming the certificate a chain error is about, or a placeholder when the context has no subject.</summary>
    Private Function CertificateClause(Context As ChainContext) As String
        If Context Is Nothing OrElse String.IsNullOrEmpty(Context.SubjectCommonName) Then Return "ismeretlen tanúsítványhoz"
        If String.IsNullOrEmpty(Context.IssuerCommonName) Then Return $"a(z) '{Context.SubjectCommonName}' tanúsítványhoz"
        Return $"a(z) '{Context.SubjectCommonName}' tanúsítványhoz (kiadó: '{Context.IssuerCommonName}')"
    End Function

    ''' <summary>Message of an exception without the SecureBlackbox unit prefix it opens with.</summary>
    Friend Function ExceptionText(Ex As Exception) As String
        Return StripComponentPrefix(Ex?.Message)
    End Function

    ''' <summary>Strips every leading bracket holding a dotted, whitespace-free "SB…" unit name; any other leading bracket is message text.</summary>
    Friend Function StripComponentPrefix(Message As String) As String
        If String.IsNullOrEmpty(Message) Then Return Message
        Dim text As String = Message.TrimStart()
        While text.Length > 0 AndAlso text(0) = "["c
            Dim close As Integer = text.IndexOf("]"c)
            If close < 0 Then Exit While
            Dim unit As String = text.Substring(1, close - 1)
            If Not unit.StartsWith("SB", StringComparison.Ordinal) OrElse Not unit.Contains(".") OrElse unit.IndexOfAny({" "c, vbTab(0)}) >= 0 Then Exit While
            text = text.Substring(close + 1).TrimStart()
        End While
        Return text
    End Function

    ''' <summary>Removes "user:password@" from the authority of a URL; an '@' in the path or query is left alone.</summary>
    Friend Function StripUserInfo(Url As String) As String
        If String.IsNullOrEmpty(Url) Then Return Url
        Dim schemeEnd As Integer = Url.IndexOf("://", StringComparison.Ordinal)
        If schemeEnd < 0 Then Return Url
        Dim authorityStart As Integer = schemeEnd + 3
        Dim authorityEnd As Integer = Url.IndexOf("/"c, authorityStart)
        Dim at As Integer = Url.IndexOf("@"c, authorityStart)
        If at >= 0 AndAlso (authorityEnd < 0 OrElse at < authorityEnd) Then Return Url.Remove(authorityStart, at - authorityStart + 1)
        Return Url
    End Function
End Module
