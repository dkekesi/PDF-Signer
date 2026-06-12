Imports Kofax.Capture.AdminModule.InteropServices

Public Class SetupCSSParser
    Private ReadOnly _docClass As DocumentClass

    Public Sub New(DocClass As DocumentClass)
        _docClass = DocClass
    End Sub

    Public Function ReadSetupCSS(CSSName As String) As String
        Try
            Return _docClass.CustomStorageString(CSSName)
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Public Function ReadSetupCSSInteger(CSSName As String, DefaultValue As Integer) As Integer
        Try
            Dim res As Integer
            Dim cssContent As String = ReadSetupCSS(CSSName)

            If Integer.TryParse(cssContent, res) Then
                Return res
            Else
                Return DefaultValue
            End If

        Catch ex As Exception
            Return DefaultValue
        End Try
    End Function

    Public Sub WriteSetupCSS(CSSName As String, CSSValue As String)
        _docClass.CustomStorageString(CSSName) = CSSValue
    End Sub

    Public Function IsIndexNameStoredInCSSValid(CSSName As String) As Boolean
        Try
            Dim IndexName = ReadSetupCSS(CSSName)
            If String.IsNullOrEmpty(IndexName) Then Return True

            For Each ind As IndexField In _docClass.IndexFields
                If ind.Name = IndexName Then Return True
            Next

            Return False

        Catch
            Return False
        End Try
    End Function

End Class
