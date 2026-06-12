Public Module Converter
    Public Function StringToBoolean(Input As String) As Boolean
        If Not String.IsNullOrEmpty(Input) Then
            If Input = "1" OrElse Input.ToLower = "true" Then
                Return True
            End If
        End If

        Return False
    End Function

    Public Function BooleanToNumericString(Input As Boolean) As String
        If Input Then
            Return "1"
        Else
            Return "0"
        End If
    End Function

    Public Function BooleanToLiteralString(Input As Boolean, UppercaseFirstLetter As Boolean) As String
        If Input Then
            If UppercaseFirstLetter Then
                Return "True"
            Else
                Return "true"
            End If
        Else
            If UppercaseFirstLetter Then
                Return "False"
            Else
                Return "false"
            End If
        End If
    End Function

    Public Function StringToInteger(Input As String) As Integer
        Dim i As Integer = 0
        Integer.TryParse(Input, i)
        Return i
    End Function

    Public Function StringToUri(Input As String) As Uri
        Dim newUri As Uri = Nothing

        If Not String.IsNullOrEmpty(Input) Then
            Uri.TryCreate(Input, UriKind.RelativeOrAbsolute, newUri)
        End If

        Return newUri
    End Function
End Module
