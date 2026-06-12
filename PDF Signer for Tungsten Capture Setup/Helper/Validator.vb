Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions

Public Class Validator
    Public Function IsBase64String(s As String) As Boolean
        Return s.Length Mod 4 = 0 AndAlso Regex.IsMatch(s, "^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None)
    End Function

    Public Function IsValidFileString(FileString As String) As Boolean
        For i = 0 To FileString.Length - 1
            If Path.GetInvalidFileNameChars().Contains(FileString.Chars(i)) Then Return False
        Next

        Return True
    End Function
End Class
