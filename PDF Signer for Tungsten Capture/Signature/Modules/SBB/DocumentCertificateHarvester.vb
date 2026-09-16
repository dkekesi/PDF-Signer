Imports System.IO
Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Certificates, CRLs and OCSP responses the document currently carries, kept as DER bytes so they outlive the verifier that read them.</summary>
Friend NotInheritable Class HarvestedMaterial
    ''' <summary>DER of every certificate the document's signatures embed.</summary>
    Friend ReadOnly Property CertificateBytes As New List(Of Byte())
    ''' <summary>DER of every CRL the document's signatures embed.</summary>
    Friend ReadOnly Property CrlBytes As New List(Of Byte())
    ''' <summary>DER of every OCSP response the document's signatures embed.</summary>
    Friend ReadOnly Property OcspBytes As New List(Of Byte())
    ''' <summary>EntityLabel of the last signature entity (the target of the next Update pass), or Nothing.</summary>
    Friend Property LastSignatureEntityLabel As String
    ''' <summary>DER of the TSA certificate of the last signature's first timestamp, or Nothing.</summary>
    Friend Property SignatureTimestampCertificateBytes As Byte()
    ''' <summary>Display ids (S0 / T0 / S0T0) for the document's entity labels.</summary>
    Friend Property Naming As EntityNaming = EntityNaming.Empty

    ''' <summary>Material of an unsigned document: nothing harvested.</summary>
    Friend Shared Function Empty() As HarvestedMaterial
        Return New HarvestedMaterial
    End Function

    ''' <summary>A new list holding the store certificates followed by the harvested ones; the input list is never mutated.</summary>
    Friend Function BuildKnownCertificates(StoreCertificates As CertificateList) As CertificateList
        Dim combined As New CertificateList
        If StoreCertificates IsNot Nothing Then
            For i As Integer = 0 To StoreCertificates.Count - 1
                combined.Add(StoreCertificates(i))
            Next
        End If
        For Each b As Byte() In CertificateBytes
            combined.Add(New Certificate(b, 0, b.Length))
        Next
        Return combined
    End Function

    ''' <summary>Feeds the harvested CRLs/OCSP responses to a signer so an earlier pass's downloads are not repeated.</summary>
    Friend Sub ApplyKnownRevocation(Signer As SbbPdfSigner)
        For Each crl As Byte() In CrlBytes
            Signer.KnownCRLs.Add(New CRL(crl, 0, crl.Length))
        Next
        For Each ocsp As Byte() In OcspBytes
            Signer.KnownOCSPs.Add(New OCSPResponse(ocsp, 0, ocsp.Length))
        Next
    End Sub

    ''' <summary>Feeds the harvested CRLs/OCSP responses to a validator.</summary>
    Friend Sub ApplyKnownRevocation(Validator As CertificateValidator)
        For Each crl As Byte() In CrlBytes
            Validator.KnownCRLs.Add(New CRL(crl, 0, crl.Length))
        Next
        For Each ocsp As Byte() In OcspBytes
            Validator.KnownOCSPs.Add(New OCSPResponse(ocsp, 0, ocsp.Length))
        Next
    End Sub
End Class

''' <summary>Parse-only reads of the working document: encryption/entity probe and harvest of its embedded validation material.</summary>
Friend Module DocumentCertificateHarvester
    ''' <summary>Opens the document without validation and reports whether it is encrypted and how many signature entities it holds.</summary>
    Friend Sub Probe(Document As MemoryTributary, ByRef Encrypted As Boolean, ByRef EntityCount As Integer)
        Document.Seek(0, SeekOrigin.Begin)
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            verifier.InputStream = Document
            verifier.AutoValidateSignatures = False
            Try
                verifier.Open(False)
                Encrypted = verifier.DocumentInfo IsNot Nothing AndAlso verifier.DocumentInfo.EncryptionType <> PDFEncryptionTypes.petNone
                EntityCount = verifier.Signatures.Count
                verifier.Close(False)
            Finally
                Document.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
    End Sub

    ''' <summary>Parse-only Verify() (no network, no chain build) capturing the DSS and the last entity's label; Verify() closes the document itself.</summary>
    Friend Function Harvest(Document As MemoryTributary) As HarvestedMaterial
        Dim material As New HarvestedMaterial
        Document.Seek(0, SeekOrigin.Begin)
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            verifier.InputStream = Document
            verifier.AutoValidateSignatures = False
            Try
                verifier.Verify()

                If verifier.Signatures.Count > 0 Then
                    Dim last As PDFSignature = verifier.Signatures(verifier.Signatures.Count - 1)
                    material.LastSignatureEntityLabel = last.EntityLabel
                    For j As Integer = 0 To verifier.Timestamps.Count - 1
                        Dim ts As TimestampInfo = verifier.Timestamps(j)
                        If Not String.Equals(ts.ParentEntity, last.EntityLabel, StringComparison.Ordinal) Then Continue For
                        If ts.CertificateIndex >= 0 AndAlso ts.CertificateIndex < verifier.Certificates.Count Then
                            material.SignatureTimestampCertificateBytes = verifier.Certificates(ts.CertificateIndex).Bytes
                        End If
                        Exit For
                    Next
                End If

                material.Naming = EntityNaming.FromVerifier(verifier)
                For i As Integer = 0 To verifier.Certificates.Count - 1
                    AddIfNotEmpty(material.CertificateBytes, verifier.Certificates(i).Bytes)
                Next
                For i As Integer = 0 To verifier.CRLs.Count - 1
                    AddIfNotEmpty(material.CrlBytes, verifier.CRLs(i).Bytes)
                Next
                For i As Integer = 0 To verifier.OCSPs.Count - 1
                    AddIfNotEmpty(material.OcspBytes, verifier.OCSPs(i).Bytes)
                Next
            Finally
                Document.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
        Return material
    End Function

    Private Sub AddIfNotEmpty(Target As List(Of Byte()), Bytes As Byte())
        If Bytes IsNot Nothing AndAlso Bytes.Length > 0 Then Target.Add(Bytes)
    End Sub
End Module
