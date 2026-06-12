Imports NLog
Imports Kofax.Capture.SDK.Data
Imports System.Globalization
Imports System.Threading

Friend Module CSSParser
    Private ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()
    Private Const _maxRetryCount As Integer = 5

    Friend Function GetCSSValue(Document As IACDataElement, CssName As String) As String
        Dim DocCSSsElement As IACDataElement = Document.FindChildElementByName("DocumentCustomStorageStrings")
        Dim DocCSS As IACDataElement = DocCSSsElement.FindChildElementByAttribute("DocumentCustomStorageString", "Name", CssName)

        Dim res As String = String.Empty
        If DocCSS IsNot Nothing Then res = DocCSS("Value")

        Return res
    End Function

    Friend Function GetCSSDateValue(Document As IACDataElement, CssName As String) As Date?
        Dim DocCSSsElement As IACDataElement = Document.FindChildElementByName("DocumentCustomStorageStrings")
        Dim DocCSS As IACDataElement = DocCSSsElement.FindChildElementByAttribute("DocumentCustomStorageString", "Name", CssName)

        If DocCSS IsNot Nothing Then
            If String.IsNullOrWhiteSpace(DocCSS("Value")) Then Return Nothing

            Dim res As Date
            Date.TryParseExact(DocCSS("Value"), "o", CultureInfo.InvariantCulture, DateTimeStyles.None, res)
            Return res
        End If

        Return Nothing
    End Function

    Friend Sub SetCSSValue(Document As IACDataElement, CssName As String, CssValue As String)
        Dim cssWritten As Boolean = False
        Dim retryCounter As Integer = 1

        While Not cssWritten
            Try
                SetCSSTextValue(Document, CssName, CssValue)
                cssWritten = True

            Catch ex As Exception
                If retryCounter = _maxRetryCount Then
                    Throw ex
                End If

                _logger.Warn("'{0}' CSS írási hiba ({1}. próbálkozás sikertelen)", CssName, retryCounter)
                retryCounter += 1
                Thread.Sleep(400 * retryCounter)
            End Try
        End While
    End Sub

    Private Sub SetCSSTextValue(Document As IACDataElement, CssName As String, CssValue As String)
        Dim DocCSSsElement As IACDataElement = Document.FindChildElementByName("DocumentCustomStorageStrings")
        Dim DocCSS As IACDataElement = DocCSSsElement.FindChildElementByAttribute("DocumentCustomStorageString", "Name", CssName)

        ' ha nem létezik a CSS akkor létrehozzuk
        If DocCSS Is Nothing Then
            '_logger.Debug("'{0}' CSS nem létezik, létre kell hozni", CssName)
            DocCSS = DocCSSsElement.CreateChildElement("DocumentCustomStorageString")
            DocCSS("Name") = CssName
        End If

        DocCSS("Value") = CssValue
        '_logger.Debug("'{0}' CSS értéke '{1}'", CssName, CssValue)
    End Sub

    Friend Sub SetCSSValue(Document As IACDataElement, CssName As String, CssValue As Date?)
        Dim cssWritten As Boolean = False
        Dim retryCounter As Integer = 1

        While Not cssWritten
            Try
                SetCSSDateValue(Document, CssName, CssValue)
                cssWritten = True

            Catch ex As Exception
                If retryCounter = _maxRetryCount Then
                    Throw ex
                End If

                _logger.Warn("'{0}' CSS írási hiba ({1}. próbálkozás sikertelen)", CssName, retryCounter)
                retryCounter += 1
                Thread.Sleep(400 * retryCounter)
            End Try
        End While
    End Sub

    Private Sub SetCSSDateValue(Document As IACDataElement, CssName As String, CssValue As Date?)
        Dim DocCSSsElement As IACDataElement = Document.FindChildElementByName("DocumentCustomStorageStrings")
        Dim DocCSS As IACDataElement = DocCSSsElement.FindChildElementByAttribute("DocumentCustomStorageString", "Name", CssName)

        ' ha nem létezik a CSS akkor létrehozzuk
        If DocCSS Is Nothing Then
            '_logger.Debug("'{0}' CSS nem létezik, létre kell hozni", CssName)
            DocCSS = DocCSSsElement.CreateChildElement("DocumentCustomStorageString")
            DocCSS("Name") = CssName
        End If

        Dim CSSExportValue As String = String.Empty
        If CssValue.HasValue Then CSSExportValue = CssValue.Value.ToString("o")

        DocCSS("Value") = CSSExportValue
        '_logger.Debug("'{0}' CSS értéke '{1}'", CssName, CSSExportValue)
    End Sub

End Module
