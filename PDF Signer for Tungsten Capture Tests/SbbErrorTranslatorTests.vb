Imports PDFSigner

''' <summary>Verifies SbbErrorTranslator's Hungarian rendering of SecureBlackbox revocation/wrapper errors and exception text cleanup.</summary>
<TestClass>
Public Class SbbErrorTranslatorTests
    Private Shared ReadOnly Tsa As New ChainContext("e-Szigno TSA 2020 01", "e-Szigno Qualified TSA CA 2020")

    <TestMethod>
    Public Sub Renders_the_reference_ocsp_line()
        Dim line As String = SbbErrorTranslator.Describe(28311557, "OCSP error 2002 (location: http://etsaca2020-ocsp1.e-szigno.hu)", Tsa, "'S0' aláírás-időbélyege")
        Assert.AreEqual(
            "'S0' aláírás-időbélyege: tanúsítványlánc visszavonás-ellenőrzési hiba: " &
            "nincs használható OCSP válasz ehhez a válaszadóhoz (offline módban: nincs megfelelő beágyazott válasz) " &
            "a(z) 'e-Szigno TSA 2020 01' tanúsítványhoz (kiadó: 'e-Szigno Qualified TSA CA 2020'), " &
            "válaszadó: http://etsaca2020-ocsp1.e-szigno.hu [SBB 28311557 / OCSP 2002]", line)
    End Sub

    <DataTestMethod>
    <DataRow(2001, "a kapott OCSP válasz elutasítva")>
    <DataRow(2003, "az OCSP válaszadó nem érhető el")>
    <DataRow(2005, "az OCSP válaszadó tanúsítványát visszavonták")>
    Public Sub Maps_ocsp_sub_codes(subCode As Integer, fragment As String)
        Dim line As String = SbbErrorTranslator.Describe(28311557, $"OCSP error {subCode} (location: http://ocsp.example)", Tsa)
        StringAssert.Contains(line, fragment)
        StringAssert.EndsWith(line, $"[SBB 28311557 / OCSP {subCode}]")
    End Sub

    <DataTestMethod>
    <DataRow(1001, "a CRL elutasítva")>
    <DataRow(1004, "egyetlen CRL elosztási pont sem")>
    Public Sub Maps_crl_sub_codes(subCode As Integer, fragment As String)
        Dim line As String = SbbErrorTranslator.Describe(28311557, $"CRL error {subCode} (location: http://crl.example/a.crl)", Tsa)
        StringAssert.Contains(line, fragment)
        StringAssert.Contains(line, "elosztási pont: http://crl.example/a.crl")
    End Sub

    <TestMethod>
    Public Sub Empty_location_means_an_already_held_response()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2001 (location: )", Tsa), "válaszadó: nincs megadva (már meglévő válasz)")
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "CRL error 1001 (location: )", Tsa), "elosztási pont: nincs megadva (már meglévő CRL)")
    End Sub

    <TestMethod>
    Public Sub Unknown_codes_are_decoded_not_dropped()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2099 (location: http://x)", Tsa), "ismeretlen OCSP hiba")
        StringAssert.Contains(SbbErrorTranslator.Describe(&H1B00007, "OCSP error 2003 (location: http://x)", Nothing), "ismeretlen hiba (modul 0x1B0, hiba 7)")
        Assert.AreEqual("ismeretlen hiba (modul 0xFFFF, hiba 65535)", SbbErrorTranslator.WrapperText(-1))
    End Sub

    <TestMethod>
    Public Sub Non_template_descriptions_are_kept()
        Assert.AreEqual("általános hiba: Something odd happened [SBB 1048585]", SbbErrorTranslator.Describe(1048585, "Something odd happened", Nothing))
        Assert.AreEqual("általános hiba: Something odd happened – a(z) 'e-Szigno TSA 2020 01' tanúsítványhoz (kiadó: 'e-Szigno Qualified TSA CA 2020') [SBB 1048585]",
                        SbbErrorTranslator.Describe(1048585, "Something odd happened", Tsa))
        StringAssert.Contains(SbbErrorTranslator.Describe(1048585, "", Nothing), "(nincs leírás)")
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2003 (location: http://x)", ChainContext.Empty), "ismeretlen tanúsítványhoz")
    End Sub

    <TestMethod>
    Public Sub User_info_is_stripped_from_urls()
        StringAssert.Contains(SbbErrorTranslator.Describe(28311557, "OCSP error 2003 (location: http://user:pw@ocsp.example/x)", Tsa), "válaszadó: http://ocsp.example/x")
        Assert.AreEqual("http://tsa.example/ts", SbbErrorTranslator.StripUserInfo("http://user:pw@tsa.example/ts"))
        Assert.AreEqual("https://tsa.example/ts?a=b@c", SbbErrorTranslator.StripUserInfo("https://tsa.example/ts?a=b@c"))
        Assert.AreEqual("tsa.example", SbbErrorTranslator.StripUserInfo("tsa.example"))
        Assert.IsNull(SbbErrorTranslator.StripUserInfo(Nothing))
    End Sub

    <TestMethod>
    Public Sub Component_prefixes_are_stripped_from_exception_text()
        Assert.AreEqual("Chain validation failed (721417/0xB0209)", SbbErrorTranslator.StripComponentPrefix("[SBPAdES.EElPDFAdvancedPublicKeySecurityHandlerError] Chain validation failed (721417/0xB0209)"))
        Assert.AreEqual("failure info is 25", SbbErrorTranslator.StripComponentPrefix("[SBCMS.EElCMSError] [SBTSPClient.EElTSPError] failure info is 25"))
        Assert.AreEqual("[tx=1] a hívás megszakadt", SbbErrorTranslator.StripComponentPrefix("[tx=1] a hívás megszakadt"))
        Assert.AreEqual("[SBPAdES hiba] szöveg", SbbErrorTranslator.StripComponentPrefix("[SBPAdES hiba] szöveg"))
        Assert.AreEqual("Chain validation failed", SbbErrorTranslator.ExceptionText(New Exception("[SBPAdES.EElPDFError] Chain validation failed")))
    End Sub

    <TestMethod>
    Public Sub Url_credentials_are_scrubbed_from_a_free_text_description()
        Assert.AreEqual("általános hiba: Connection to https://tsa.example/ts failed [SBB 1048585]",
                        SbbErrorTranslator.Describe(1048585, "Connection to https://u:p@tsa.example/ts failed", Nothing))
        Assert.AreEqual("Connection to https://tsa.example/ts failed",
                        SbbErrorTranslator.ExceptionText(New Exception("Connection to https://u:p@tsa.example/ts failed")))
    End Sub

    <TestMethod>
    Public Sub Url_credentials_are_scrubbed_from_every_url_in_the_text()
        Assert.AreEqual("Primary https://a.example/x failed, falling back to https://b.example/y",
                        SbbErrorTranslator.ScrubUrlCredentials("Primary https://u1:p1@a.example/x failed, falling back to https://u2:p2@b.example/y"))
    End Sub

    <TestMethod>
    Public Sub A_plain_at_sign_without_a_url_is_left_alone()
        Assert.AreEqual("contact admin@example.com for help", SbbErrorTranslator.ScrubUrlCredentials("contact admin@example.com for help"))
        Assert.IsNull(SbbErrorTranslator.ScrubUrlCredentials(Nothing))
    End Sub
End Class
