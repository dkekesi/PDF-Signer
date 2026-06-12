Imports System.Xml.Schema
Imports System.Xml
Imports System.IO
Imports PDFSignerCommon.My.Resources

Public Class ValidationMethods
    Private XSDValidationErrors As String

    Public Function ValidateXMLWithXSD(dom As XDocument, xsdMarkup As String) As String
        XSDValidationErrors = String.Empty

        Dim schemas = New XmlSchemaSet()
        schemas.Add("", XmlReader.Create(New StringReader(xsdMarkup)))

        Dim settings = New XmlReaderSettings With {
            .CloseInput = True,
            .ValidationType = ValidationType.Schema,
            .ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings Or XmlSchemaValidationFlags.ProcessIdentityConstraints Or XmlSchemaValidationFlags.ProcessInlineSchema Or XmlSchemaValidationFlags.ProcessSchemaLocation
        }
        settings.Schemas.Add(schemas)

        Dim r = New StringReader(dom.ToString())
        AddHandler settings.ValidationEventHandler, AddressOf Settings_ValidationEventHandler
        Using validatingReader As XmlReader = XmlReader.Create(r, settings)
            While validatingReader.Read()
            End While
        End Using

        RemoveHandler settings.ValidationEventHandler, AddressOf Settings_ValidationEventHandler
        Return XSDValidationErrors
    End Function

    Private Sub Settings_ValidationEventHandler(sender As Object, e As ValidationEventArgs)
        If e.Severity = XmlSeverityType.Error OrElse e.Severity = XmlSeverityType.Warning Then
            XSDValidationErrors += String.Format(vbCrLf & CommonRes.XMLSchemaValidationError, e.Exception.LineNumber, e.Exception.LinePosition, e.Exception.Message)
        End If
    End Sub
End Class
