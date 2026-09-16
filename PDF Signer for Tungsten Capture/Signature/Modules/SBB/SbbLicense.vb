Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Creates SecureBlackbox components with the product runtime license applied; each component validates its own RuntimeLicense.</summary>
Friend Module SbbLicense
    ''' <summary>nsoftware runtime key, expires 2026-11-15; the legacy API receives the same key through SBUtils.Unit.SetLicenseKey.</summary>
    Friend Const Key As String = "53424E4A41444E58524632303236313131353159554631393534004955475658465444494C465800303030303030303000004A574A30585037483239325A0000"

    ''' <summary>A licensed PDFSigner.</summary>
    Friend Function CreateSigner() As SbbPdfSigner
        Return New SbbPdfSigner With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed PDFVerifier.</summary>
    Friend Function CreateVerifier() As PDFVerifier
        Return New PDFVerifier With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed CertificateManager.</summary>
    Friend Function CreateCertificateManager() As CertificateManager
        Return New CertificateManager With {.RuntimeLicense = Key}
    End Function

    ''' <summary>A licensed CertificateValidator.</summary>
    Friend Function CreateCertificateValidator() As CertificateValidator
        Return New CertificateValidator With {.RuntimeLicense = Key}
    End Function
End Module
