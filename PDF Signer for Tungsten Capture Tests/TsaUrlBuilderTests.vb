Imports PDFSigner

''' <summary>Verifies TsaUrlBuilder embeds and percent-encodes credentials into a TSA URL's user-info part.</summary>
<TestClass>
Public Class TsaUrlBuilderTests
    <TestMethod>
    Public Sub Missing_user_returns_the_url_unchanged()
        Assert.AreEqual("https://btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", Nothing, Nothing))
        Assert.AreEqual("https://btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "", "x"))
    End Sub

    <TestMethod>
    Public Sub Credentials_are_embedded_after_the_scheme_and_the_query_survives()
        Assert.AreEqual("https://2.1.19581:secret@btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "2.1.19581", "secret"))
        Assert.AreEqual("https://user:pw@www.netlock.hu/timestampm.cgi?promoid=mNbR649Uqh", TsaUrlBuilder.WithCredentials("https://www.netlock.hu/timestampm.cgi?promoid=mNbR649Uqh", "user", "pw"))
        Assert.AreEqual("http://user:pw@tsa.example.com/ts", TsaUrlBuilder.WithCredentials("http://tsa.example.com/ts", "user", "pw"))
    End Sub

    <DataTestMethod>
    <DataRow("a:b", "p@ss/wd", "https://a%3Ab:p%40ss%2Fwd@btsa.e-szigno.hu/tsa")>
    <DataRow("user", "jelszó", "https://user:jelsz%C3%B3@btsa.e-szigno.hu/tsa")>
    Public Sub Reserved_and_non_ascii_characters_are_percent_encoded(user As String, password As String, expected As String)
        Assert.AreEqual(expected, TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", user, password))
    End Sub

    <TestMethod>
    Public Sub Null_password_is_empty_and_an_invalid_url_throws()
        Assert.AreEqual("https://user:@btsa.e-szigno.hu/tsa", TsaUrlBuilder.WithCredentials("https://btsa.e-szigno.hu/tsa", "user", Nothing))
        Assert.ThrowsException(Of ArgumentException)(Sub() TsaUrlBuilder.WithCredentials("btsa.e-szigno.hu", "user", "pw"))
    End Sub
End Class
