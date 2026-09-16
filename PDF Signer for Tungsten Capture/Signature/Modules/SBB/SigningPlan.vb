Imports nsoftware.SecureBlackbox

''' <summary>The passes one signing call runs, decided once from the provider settings.</summary>
Friend NotInheritable Class SigningPlan
    ''' <summary>Level of the Sign pass: paslBaselineB, or paslBaselineT (B-LT/B-LTA are lifted later by Update).</summary>
    Friend Property SignatureLevel As PAdESSignatureLevels
    ''' <summary>Revocation policy of the checking passes: the configured protocol, or crcNone when checking is off.</summary>
    Friend Property Revocation As PDFSignerRevocationChecks
    ''' <summary>RevocationCheck applied by the Sign passes: equals <see cref="Revocation"/> for B-B/B-T, crcNone for the two-pass levels.</summary>
    Friend Property SignPassRevocationCheck As PDFSignerRevocationChecks
    ''' <summary>True when the Sign pass requests a signature timestamp from the TSA.</summary>
    Friend Property EmbedSignatureTimestamp As Boolean
    ''' <summary>True when a fail-closed CertificateValidator pass checks the signing chain before signing (B-B/B-T with checking on).</summary>
    Friend Property CheckSigningCertRevocation As Boolean
    ''' <summary>True when a fail-closed CertificateValidator pass checks the TSA chain after signing (B-T with checking on).</summary>
    Friend Property CheckTimestampCertRevocation As Boolean
    ''' <summary>True when an Update pass collects and embeds the new signature's revocation data (B-LT/B-LTA).</summary>
    Friend Property UpdateToEmbed As Boolean
    ''' <summary>True when a lean document-timestamp pass appends the archive timestamp (B-LTA).</summary>
    Friend Property AddDocumentTimestamp As Boolean
    ''' <summary>True when a further Update pass embeds the archive timestamp's own TSA-chain revocation (B-LTA with checking on).</summary>
    Friend Property EmbedDocumentTimestampRevocation As Boolean
End Class
