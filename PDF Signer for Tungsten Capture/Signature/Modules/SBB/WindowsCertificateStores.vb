Imports System.Security.Cryptography.X509Certificates
Imports NLog
Imports nsoftware.SecureBlackbox

''' <summary>Trust anchors and intermediate certificates of the Windows stores as SecureBlackbox lists, loaded once per signing call.</summary>
Friend NotInheritable Class WindowsCertificateStores
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    ''' <summary>Root store of the current user and the machine.</summary>
    Friend ReadOnly Property TrustedRoots As New CertificateList
    ''' <summary>Intermediate CA store of the current user and the machine.</summary>
    Friend ReadOnly Property IntermediateCertificates As New CertificateList

    ''' <summary>Loads TrustedRoots and IntermediateCertificates from the current user's and the machine's Root and CA stores.</summary>
    Friend Shared Function Load() As WindowsCertificateStores
        Dim res As New WindowsCertificateStores
        For Each location As StoreLocation In {StoreLocation.CurrentUser, StoreLocation.LocalMachine}
            AddStore(StoreName.Root, location, res.TrustedRoots)
            AddStore(StoreName.CertificateAuthority, location, res.IntermediateCertificates)
        Next
        Return res
    End Function

    ''' <summary>A new list of fresh Certificate copies built from List's DER bytes; List and its certificates are left untouched, so each signing pass can dispose its own copy independently.</summary>
    Friend Shared Function CopyOf(List As CertificateList) As CertificateList
        Dim copy As New CertificateList
        If List IsNot Nothing Then
            For i As Integer = 0 To List.Count - 1
                Dim raw As Byte() = List(i).Bytes
                copy.Add(New Certificate(raw, 0, raw.Length))
            Next
        End If
        Return copy
    End Function

    ''' <summary>Adds every certificate of one store; an unreadable store is logged and skipped so signing can still proceed on the other stores.</summary>
    Private Shared Sub AddStore(Name As StoreName, Location As StoreLocation, Target As CertificateList)
        Try
            Using store As New X509Store(Name, Location)
                store.Open(OpenFlags.ReadOnly Or OpenFlags.OpenExistingOnly)
                For Each cert As X509Certificate2 In store.Certificates
                    Dim raw As Byte() = cert.RawData
                    Target.Add(New Certificate(raw, 0, raw.Length))
                Next
            End Using
        Catch ex As Exception
            _logger.Warn(ex, "A(z) {0}\{1} tanúsítványtár nem olvasható", Location, Name)
        End Try
    End Sub
End Class
