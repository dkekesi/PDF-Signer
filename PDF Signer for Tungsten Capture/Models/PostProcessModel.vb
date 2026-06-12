Imports System.IO

Friend Class PostProcessRequest
    Friend Property FileToPostProcess As Stream
    Friend Property DocItem As DocumentItem
End Class

Friend Class PostProcessResult
    Friend Property ErrorMessage As String
    Friend Property PostProcessLog As String

    Friend Function Validate() As String
        If Not String.IsNullOrEmpty(ErrorMessage) Then Return ErrorMessage

        Return Nothing
    End Function
End Class
