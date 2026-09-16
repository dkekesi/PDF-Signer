''' <summary>Outcome codes of the end-of-validity calculation; the reported time is always a concrete instant.</summary>
Friend Enum ValidityStatus
    ValidUntil
    Expired
    BrokenChain
    MissingRevocationData
    Invalid
    SuspectTimeline
End Enum

''' <summary>What a covering layer is; the signature itself is implicit in <see cref="SignatureInput"/>.</summary>
Friend Enum LayerKind
    SignatureTimestamp
    DocumentTimestamp
End Enum

''' <summary>Per-document aggregation: All = valid only while every signature is; Any = while at least one is.</summary>
Friend Enum AggregationPolicy
    All
    Any
End Enum

''' <summary>Revocation facts for one certificate; Nothing on a certificate means no revocation cap.</summary>
Friend NotInheritable Class RevocationInfo
    ''' <summary>Whether the certificate has been revoked.</summary>
    Friend Property Revoked As Boolean
    ''' <summary>The revocation instant reported by the CRL/OCSP responder.</summary>
    Friend Property RevocationTime As DateTimeOffset
    ''' <summary>CRL reason name, e.g. "keyCompromise".</summary>
    Friend Property Reason As String
    ''' <summary>The reported invalidity date, when the responder supplied one.</summary>
    Friend Property InvalidityDate As DateTimeOffset?

    ''' <summary>Effective revocation instant: the invalidity date for compromise reasons, else the revocation time.</summary>
    Friend Function EffectiveR() As DateTimeOffset
        If InvalidityDate.HasValue AndAlso (Reason = "keyCompromise" OrElse Reason = "cACompromise") Then Return InvalidityDate.Value
        Return RevocationTime
    End Function
End Class

''' <summary>One certificate of a path.</summary>
Friend NotInheritable Class CertInfo
    ''' <summary>Start of the certificate's validity period.</summary>
    Friend Property NotBefore As DateTimeOffset
    ''' <summary>End of the certificate's validity period.</summary>
    Friend Property NotAfter As DateTimeOffset
    ''' <summary>Revocation facts for this certificate, or Nothing when it carries no revocation cap.</summary>
    Friend Property Revocation As RevocationInfo
    ''' <summary>Subject common name, for the signature log.</summary>
    Friend Property CommonName As String
End Class

''' <summary>Algorithm identifiers for the sunset lookup.</summary>
Friend NotInheritable Class AlgorithmId
    ''' <summary>OID of the signature algorithm.</summary>
    Friend Property SignatureAlgOid As String
    ''' <summary>OID of the hash algorithm.</summary>
    Friend Property HashAlgOid As String
    ''' <summary>Key size in bits.</summary>
    Friend Property KeyBits As Integer
End Class

''' <summary>A covering timestamp layer; CertPath(0) is the TSA certificate.</summary>
Friend NotInheritable Class Layer
    ''' <summary>Whether this layer is a signature timestamp or a document (archive) timestamp.</summary>
    Friend Property Kind As LayerKind
    ''' <summary>Log id of the timestamp (S0T0 / T1); the calculator falls back to "L{n}" when Nothing.</summary>
    Friend Property DisplayId As String
    ''' <summary>Position of this layer among the document's timestamps, in application order.</summary>
    Friend Property DocumentOrderIndex As Integer
    ''' <summary>The validated genTime.</summary>
    Friend Property TrustedTime As DateTimeOffset
    ''' <summary>The TSA certificate chain; element 0 is the TSA leaf certificate.</summary>
    Friend Property CertPath As New List(Of CertInfo)
    ''' <summary>Signature algorithm of the timestamp token, for the sunset lookup.</summary>
    Friend Property Algorithm As AlgorithmId
    ''' <summary>Instant from which the DSS lacks revocation data this layer would need, else Nothing.</summary>
    Friend Property MissingRevocationFrom As DateTimeOffset?
    ''' <summary>True when the TSA path lacks a CA certificate / CRL / OCSP; makes the layer's validity end "soft".</summary>
    Friend Property MissingData As Boolean
End Class

''' <summary>A signature and its covering timestamps; SignerPath(0) is the signer certificate.</summary>
Friend NotInheritable Class SignatureInput
    ''' <summary>Stable identifier of the signature within the document.</summary>
    Friend Property Id As String
    ''' <summary>Log-friendly identifier; the calculator falls back to <see cref="Id"/> when Nothing.</summary>
    Friend Property DisplayId As String
    ''' <summary>Position of this signature among the document's signatures, in application order.</summary>
    Friend Property DocumentOrderIndex As Integer
    ''' <summary>The signer's certificate chain; element 0 is the signer leaf certificate.</summary>
    Friend Property SignerPath As New List(Of CertInfo)
    ''' <summary>Signature algorithm of the signature itself, for the sunset lookup.</summary>
    Friend Property Algorithm As AlgorithmId
    ''' <summary>Untrusted claimed signing time.</summary>
    Friend Property ClaimedSigningTime As DateTimeOffset?
    ''' <summary>The signature's covering timestamp layers, signature timestamps and archive timestamps alike.</summary>
    Friend Property CoveringTimestamps As New List(Of Layer)
    ''' <summary>True when the signer path lacks a CA certificate / CRL / OCSP; caps the evidence expiry at the signer's NotAfter.</summary>
    Friend Property MissingData As Boolean

    ''' <summary>The signer's own certificate, i.e. the first entry of <see cref="SignerPath"/>.</summary>
    Friend ReadOnly Property SignerLeaf As CertInfo
        Get
            Return SignerPath(0)
        End Get
    End Property
End Class

''' <summary>Whole-document input.</summary>
Friend NotInheritable Class DocumentValidationInput
    ''' <summary>How the per-signature results combine into the document result.</summary>
    Friend Property Aggregation As AggregationPolicy = AggregationPolicy.All
    ''' <summary>The document's signatures to evaluate.</summary>
    Friend Property Signatures As New List(Of SignatureInput)
    ''' <summary>Algorithm-sunset caps applied while computing each entity's own validity end.</summary>
    Friend Property Sunset As AlgorithmSunsetTable = AlgorithmSunsetTable.Empty
End Class

''' <summary>Per-signature result; both expiries are concrete instants.</summary>
Friend NotInheritable Class PerSignatureResult
    ''' <summary>Identifier copied from the evaluated <see cref="SignatureInput.Id"/>.</summary>
    Friend Property SignatureId As String
    ''' <summary>Log-friendly identifier copied from the evaluated <see cref="SignatureInput.DisplayId"/>.</summary>
    Friend Property DisplayId As String
    ''' <summary>Archival expiry: the latest instant the signature's validity chain reaches, extending across missing data.</summary>
    Friend Property Expiry As DateTimeOffset
    ''' <summary>Expiry provable from embedded evidence; equals <see cref="Expiry"/> when no entity lacks data.</summary>
    Friend Property EvidenceExpiry As DateTimeOffset
    ''' <summary>Outcome code for this signature.</summary>
    Friend Property Status As ValidityStatus
    ''' <summary>When the chain is broken, the genTime of the first layer that could not be reached.</summary>
    Friend Property GapBefore As DateTimeOffset?
    ''' <summary>Human-readable trace of the walk, in Hungarian, for the signature log.</summary>
    Friend Property Trace As New List(Of String)
End Class

''' <summary>Whole-document result.</summary>
Friend NotInheritable Class DocumentValidityResult
    ''' <summary>Document-level archival expiry, aggregated from <see cref="PerSignature"/> per <see cref="DocumentValidationInput.Aggregation"/>.</summary>
    Friend Property Expiry As DateTimeOffset
    ''' <summary>Document-level evidence expiry, aggregated the same way as <see cref="Expiry"/>.</summary>
    Friend Property EvidenceExpiry As DateTimeOffset
    ''' <summary>Outcome code for the document, aggregated the same way as <see cref="Expiry"/>.</summary>
    Friend Property Status As ValidityStatus
    ''' <summary>The per-signature results this document result was aggregated from.</summary>
    Friend Property PerSignature As New List(Of PerSignatureResult)
End Class
