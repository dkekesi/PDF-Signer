Imports PDFSigner.My.Resources
Imports System.Text
Imports System.Xml.Linq
Imports PDFSignerCommon

Friend Class MetaData
    Friend Property SignedFileName As String
    Friend Property DocumentName As String
    Friend Property DocumentSize As String
    Friend Property OrganizationName As String
    Friend Property SignerFullName As String
    Friend Property SystemNameAndVersion As String
    Friend Property RegulationName As String
    Friend Property RegulationURL As Uri
    Friend Property RegulationVersion As String
    Friend Property PartialCopyData As String
    Friend Property SignaturePolicyURL As Uri
    Friend Property SignaturePolicyHash As String
    Friend Property SignaturePolicyOID As String
    Friend Property ConversionDate As Date
    Friend Property Format As MetaDataType

    Public Function GetFileName() As String
        If Format = MetaDataType.Text Then Return "Zaradek.txt"
        If Format = MetaDataType.XML OrElse Format = MetaDataType.DublinCoreXML Then Return "Zaradek.xml"

        Throw New NotImplementedException(String.Format(Messages.Exception_Unknown_Metadata_Format, Format.ToString))
    End Function

    Public Function GetDescription() As String
        Return "Hitelesítési záradék"
    End Function

    Public Shadows Function ToString(CryptographicProvider As CryptoProviderBase) As String
        If String.IsNullOrEmpty(SignerFullName) Then Throw New ArgumentException(Messages.Exception_Signer_Name_Not_Provided)

        Dim content As String
        Dim regurl As String = If(RegulationURL Is Nothing, String.Empty, RegulationURL.ToString)
        Dim sigpolurl As String = If(SignaturePolicyURL Is Nothing, String.Empty, SignaturePolicyURL.ToString)

        If Format = MetaDataType.Text Then
            Dim sb As New StringBuilder
            sb.AppendLine("Hitelesítési záradék: Az eredeti papíralapú dokumentummal egyező")
            sb.AppendLine()
            sb.AppendLine($"A papíralapú dokumentum megnevezése: {DocumentName}")
            sb.AppendLine($"A papíralapú dokumentum fizikai méretei: {DocumentSize}")

            If Not String.IsNullOrEmpty(OrganizationName) Then
                sb.AppendLine($"A másolatkészítő szervezet megnevezése: {OrganizationName}")
            End If

            sb.AppendLine($"A másolat képi vagy tartalmi egyezéséért felelős személy neve: {SignerFullName}")
            sb.AppendLine($"A másolatkészítő rendszer megnevezése és verziószáma: {SystemNameAndVersion}")
            sb.AppendLine($"A másolatkészítési szabályzat pontos megnevezése: {RegulationName}")
            sb.AppendLine($"A másolatkészítési szabályzat verziószáma: {RegulationVersion}")
            sb.AppendLine($"A másolatkészítési szabályzat elérhetősége: {regurl}")
            sb.AppendLine($"A másolatkészítés ideje: {ConversionDate:yyyy.MM.dd. HH:mm:ss}")
            sb.AppendLine($"A másolat automatikus másolatkészítési eljárással készült: {"nem"}")
            If String.IsNullOrWhiteSpace(PartialCopyData) Then
                sb.AppendLine("A másolatkészítés a teljes dokumentumot érinti")
            Else
                sb.AppendLine($"Részleges másolatkészítés terjedelme, korlátozása: {PartialCopyData}")
            End If

            If Not String.IsNullOrEmpty(sigpolurl) Then
                sb.AppendLine($"A másolatkészítés során alkalmazott aláírás-szabályzat elérhetősége: {sigpolurl}")
            End If
            If Not String.IsNullOrEmpty(SignaturePolicyHash) Then
                sb.AppendLine($"A másolatkészítés során alkalmazott aláírás-szabályzat lenyomata: {SignaturePolicyHash}")
            End If
            If Not String.IsNullOrEmpty(SignaturePolicyOID) Then
                sb.AppendLine($"A másolatkészítés során alkalmazott aláírás-szabályzat egyedi azonosítója (OID): {SignaturePolicyOID}")
            End If

            sb.AppendLine("A hitelesítő személy az aláírás elhelyezése előtt biztonságos megjelenítővel megtekintette a dokumentumot")

            If TypeOf CryptographicProvider Is PDFSignerCryptoProvider Then
                sb.AppendLine("A dokumentum kizárólag a hitelesítést végző személy befolyása alatt állt a hitelesítés során")
            End If

            content = sb.ToString
            Return content
        End If

        If Format = MetaDataType.XML Then
            Dim xml As New XDocument With {
                .Declaration = New XDeclaration("1.0", "UTF-8", "yes")
            }

            Dim dom As New XElement("CertifiedConversionMetadata")
            dom.Add(New XElement("Statement", "Electronic copy matches the original paper document"))
            dom.Add(New XElement("OriginalDocumentName", ToXmlValidString(DocumentName)))
            dom.Add(New XElement("OriginalDocumentPhysicalSize", ToXmlValidString(DocumentSize)))

            If String.IsNullOrWhiteSpace(OrganizationName) Then
                dom.Add(New XElement("ConvertingOrganizationName", ToXmlValidString(OrganizationName)))
            End If

            dom.Add(New XElement("ConvertingPersonnelName", ToXmlValidString(SignerFullName)))
            dom.Add(New XElement("ConverterSystemNameAndVersion", ToXmlValidString(SystemNameAndVersion)))
            dom.Add(New XElement("ConversionRegulationDocumentName", ToXmlValidString(RegulationName)))
            dom.Add(New XElement("ConversionRegulationDocumentVersion", ToXmlValidString(RegulationVersion)))
            dom.Add(New XElement("ConversionRegulationDocumentURL", ToXmlValidString(regurl)))
            dom.Add(New XElement("CreationDateTime", ConversionDate.ToString("o")))
            dom.Add(New XElement("AutomaticConversion", "False"))

            If String.IsNullOrWhiteSpace(PartialCopyData) Then
                dom.Add(New XElement("PartialConversionDescription", "Conversion covers the entire original document"))
            Else
                dom.Add(New XElement("PartialConversionDescription", ToXmlValidString(PartialCopyData)))
            End If

            If Not String.IsNullOrEmpty(sigpolurl) Then
                dom.Add(New XElement("SignaturePolicyURL", ToXmlValidString(sigpolurl)))
            End If

            If Not String.IsNullOrEmpty(SignaturePolicyHash) Then
                dom.Add(New XElement("SignaturePolicyHash", SignaturePolicyHash))
            End If

            If Not String.IsNullOrEmpty(SignaturePolicyOID) Then
                dom.Add(New XElement("SignaturePolicyOID", SignaturePolicyOID))
            End If

            dom.Add(New XElement("SecureViewBeforeSigning", "Document was viewed with secure viewer prior to signing"))

            If TypeOf CryptographicProvider Is PDFSignerCryptoProvider Then
                dom.Add(New XElement("SecureDocumentControl", "Document was under the sole control of the signer throughout the entire conversion process"))
            End If

            xml.Add(dom)

            Return xml.ToString
        End If

        If Format = MetaDataType.DublinCoreXML Then
            Dim sb As New StringBuilder
            sb.AppendLine("<?xml version=""1.0"" encoding=""UTF-8""?>")
            sb.AppendLine("<mireg:metadata")
            sb.AppendLine(" xmlns:dc = ""http://purl.org/dc/elements/1.1/""")
            sb.AppendLine(" xmlns:dcterms = ""http://purl.org/dc/terms/""")
            sb.AppendLine(" xmlns:mireg = ""http://mireg.org/schema/1.0/""")
            sb.AppendLine(" xmlns:xsi = ""http://www.w3.org/2001/XMLSchema-instance""")
            sb.AppendLine(" xsi:schemaLocation = ""http://mireg.org/schema/1.0/ http://mireg.org/model/XSD/metadata.xsd""")
            sb.AppendLine($" about=""{ToXmlValidString(SignedFileName)}"">")
            sb.AppendLine(" <mireg:mireg>")
            sb.AppendLine("  <dc:description>Az eredeti papíralapú dokumentummal egyező elektronikus irat</dc:description>")
            sb.AppendLine($"  <dc:title>{ToXmlValidString(DocumentName)}</dc:title>")
            sb.AppendLine("  <dc:description>A dokumentumot az aláíró az aláírás elhelyezése előtt biztonságos megjelenítővel megtekintette</dc:description>")
            sb.AppendLine("  <dc:description>A másolatkészítés manuális konverzióval történt (nem automatikus hitelesítéssel)</dc:description>")
            If String.IsNullOrWhiteSpace(PartialCopyData) Then
                sb.AppendLine("  <dc:description>A másolatkészítés a teljes dokumentumot érinti</dc:description>")
            Else
                sb.AppendLine($"  <dc:description>Részleges másolatkészítés terjedelme, kolátozása: {ToXmlValidString(PartialCopyData)}</dc:description>")
            End If
            sb.AppendLine($"  <dc:publisher>{ToXmlValidString(OrganizationName)}</dc:publisher>")
            sb.AppendLine($"  <dc:publisher>{ToXmlValidString(SignerFullName)}</dc:publisher>")
            sb.AppendLine($"  <dc:relation>{ToXmlValidString(SystemNameAndVersion)}</dc:relation>")
            sb.AppendLine($"  <dc:relation>{ToXmlValidString(RegulationName)}, {ToXmlValidString(RegulationVersion)}</dc:relation>")
            sb.AppendLine($"  <dc:relation>{ToXmlValidString(RegulationURL.ToString)}</dc:relation>")
            sb.AppendLine($"  <dcterms:available>{ConversionDate:o}</dcterms:available>")
            sb.AppendLine($"  <dcterms:extent>{ToXmlValidString(DocumentSize)}</dcterms:extent>")
            If Not String.IsNullOrEmpty(sigpolurl) Then
                sb.AppendLine($"  <dcterms:policy>{ToXmlValidString(SignaturePolicyURL.ToString)}</dcterms:policy>")
            End If
            If Not String.IsNullOrEmpty(SignaturePolicyHash) Then
                sb.AppendLine($"  <dcterms:policy>{ToXmlValidString(SignaturePolicyHash)}</dcterms:policy>")
            End If
            If Not String.IsNullOrEmpty(SignaturePolicyOID) Then
                sb.AppendLine($"  <dcterms:policy>{ToXmlValidString(SignaturePolicyOID)}</dcterms:policy>")
            End If
            sb.AppendLine(" </mireg:mireg>")
            sb.AppendLine("</mireg:metadata>")

            content = sb.ToString
            Return content
        End If

        Throw New NotImplementedException($"Unknown metadata format: {Format}")
    End Function

    Private Function ToXmlValidString(input As String) As String
        Return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("""", "&quot;").Replace("'", "&apos;")
    End Function
End Class
