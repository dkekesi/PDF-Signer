''' <summary>Builds the PDFSigner.TimestampServer URL; credentials go into the URL because OnTimestampRequest is not raised for http(s) in this SecureBlackbox build.</summary>
Friend Module TsaUrlBuilder
    ''' <summary>The URL with user and password percent-encoded as its user-info part, or unchanged when no user is configured. The result holds the plaintext password; never log it.</summary>
    Friend Function WithCredentials(TsaUrl As String, User As String, Password As String) As String
        If String.IsNullOrEmpty(User) Then Return TsaUrl

        Dim schemeEnd As Integer = TsaUrl.IndexOf("://", StringComparison.Ordinal)
        If schemeEnd < 0 Then Throw New ArgumentException($"Érvénytelen időbélyeg szolgáltató URL: '{TsaUrl}'", NameOf(TsaUrl))

        Dim userInfo As String = Uri.EscapeDataString(User) & ":" & Uri.EscapeDataString(If(Password, String.Empty))
        Return TsaUrl.Insert(schemeEnd + 3, userInfo & "@")
    End Function
End Module
