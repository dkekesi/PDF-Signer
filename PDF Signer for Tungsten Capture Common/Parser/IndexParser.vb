Imports Kofax.Capture.SDK.Data
Imports PDFSignerCommon.My.Resources
Imports System.Threading

Public Module IndexParser
    Private Const _maxRetryCount As Integer = 5

    Public Function GetIndexValue(Document As IACDataElement, IndexFieldName As String) As String
        Dim indexRead As Boolean = False
        Dim retryCounter As Integer = 1

        If String.IsNullOrWhiteSpace(IndexFieldName) Then Return String.Empty

        While Not indexRead
            Try
                Dim IndexFieldsElement As IACDataElement = Document.FindChildElementByName("IndexFields")
                Dim IndexField As IACDataElement = IndexFieldsElement.FindChildElementByAttribute("IndexField", "Name", IndexFieldName)

                If IndexField IsNot Nothing Then
                    Dim res As String = IndexField("Value").ToString
                    Return res
                Else
                    Return String.Empty
                End If

            Catch ex As Exception
                If retryCounter = _maxRetryCount Then
                    Throw ex
                End If

                retryCounter += 1
                Thread.Sleep(400 * retryCounter)
            End Try
        End While

        Return String.Empty ' We never get here, just to keep the compiler from complaining
    End Function

    Public Sub SetIndexValue(Document As IACDataElement, IndexFieldName As String, IndexFieldValue As String)
        If String.IsNullOrWhiteSpace(IndexFieldName) Then Return

        Dim IndexFieldsElement As IACDataElement = Document.FindChildElementByName("IndexFields")

        Try
            Dim IndexFieldElement As IACDataElement = IndexFieldsElement.FindChildElementByAttribute("IndexField", "Name", IndexFieldName)

            If IndexFieldElement IsNot Nothing Then
                IndexFieldElement("Value") = IndexFieldValue
            End If

        Catch ex As Exception
            Dim msg As String = String.Format(Messages.Error_Writing_Index_Value, IndexFieldName, ex.Message)
            Throw New System.InvalidOperationException(msg)
        End Try
    End Sub

    Public Sub SetIndexValue(Document As IACDataElement, IndexFieldName As String, IndexFieldValue As Date)
        If String.IsNullOrWhiteSpace(IndexFieldName) Then Return

        Dim IndexFieldsElement As IACDataElement = Document.FindChildElementByName("IndexFields")

        Try
            Dim IndexFieldElement As IACDataElement = IndexFieldsElement.FindChildElementByAttribute("IndexField", "Name", IndexFieldName)

            If IndexFieldElement IsNot Nothing Then
                IndexFieldElement("Value") = IndexFieldValue.ToString("yyyy-MM-ddTHH:mm:ss")
            End If

        Catch ex As Exception
            Dim msg As String = String.Format(Messages.Error_Writing_Index_Value, IndexFieldName, ex.Message)
            Throw New System.InvalidOperationException(msg)
        End Try
    End Sub

End Module
