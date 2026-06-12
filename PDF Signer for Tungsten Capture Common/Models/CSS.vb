Public Module CSS
    Public Const BaseNamespace As String = "DocSoft.PDFSigner."

    ' Runtime CSSs
    Public Const IsDocumentSigned As String = BaseNamespace + "IsDocumentSigned" ' contains the fact that the document is signed
    Public Const IsDocumentSkipped As String = BaseNamespace + "IsDocumentSkipped" ' contains the fact that the document is skipped
    Public Const SignedFilePath As String = BaseNamespace + "SignedFilePath" ' contains the full path and file name of the signed file
    Public Const UnsignedFilePath As String = BaseNamespace + "UnsignedFilePath" ' contains the full path and file name of the unsigned original file
    Public Const SignedFileDateTime As String = BaseNamespace + "SignedFileDateTime" ' contains the last modification time of the signed file 
    Public Const UnsignedFileDateTime As String = BaseNamespace + "UnsignedFileDateTime" ' contains the last modification time of the unsigned file 
    Public Const DetachedSignatureFilePath As String = BaseNamespace + "DetachedSignatureFilePath" ' contains the full path and file name of the detached signature file (empty if no such file is available)
    Public Const PartialCopyData As String = BaseNamespace + "PartialCopyData" ' contains the reason of partial copy (empty if entire document is covered)
    Public Const ClauseFilePath As String = BaseNamespace + "ClauseFilePath" ' contains the full path and file name of the standalone clause file (empty if no such file is available)
    Public Const ClauseContent As String = BaseNamespace + "ClauseContent" ' contains the text content of the clause (empty if no such file is available)
    Public Const SignatureLogContent As String = BaseNamespace + "SignatureLogContent" ' contains the signature log (empty if no log is available)

    ' Setup CSSs
    Public Const IsEnabled As String = BaseNamespace + "Enabled"

    Public Const IndexIsSigned As String = BaseNamespace + "IndexIsSigned"
    Public Const IndexSignedDateTime As String = BaseNamespace + "IndexSignedDateTime"
    Public Const IndexSignedBy As String = BaseNamespace + "IndexSignedBy"
    Public Const IndexSignatureValidUntil As String = BaseNamespace + "IndexSignatureValidUntil"
    Public Const IndexSkipNote As String = BaseNamespace + "IndexSkipNote"

    Public Const SignAllDocuments As String = BaseNamespace + "AllDocsNeedSignature"
    Public Const IndexBarCode As String = BaseNamespace + "IndexBarCode"
    Public Const SignatureMarker As String = BaseNamespace + "SignatureMarker"
    Public Const SignatureMarkerPosition As String = BaseNamespace + "SignatureMarkerPosition"
    Public Const AllowSkipRequiredDocument As String = BaseNamespace + "AllowSkipRequiredDocument"

    Public Const IndexDocumentName As String = BaseNamespace + "IndexDocumentName"
    Public Const DocumentNameFromFormType As String = BaseNamespace + "DocumentNameFromFormType"
    Public Const DocumentNameDefault As String = BaseNamespace + "DocumentNameDefault"
    Public Const ConvertingOrganization As String = BaseNamespace + "ConvertingOrganization"
    Public Const ConvertingRegulationName As String = BaseNamespace + "RegulationName"
    Public Const ConvertingRegulationURL As String = BaseNamespace + "RegulationURL"
    Public Const ConvertingRegulationVersion As String = BaseNamespace + "RegulationVersion"
    Public Const MetaDataFormat As String = BaseNamespace + "MetaDataFormat"
    Public Const SignaturePolicyURL As String = BaseNamespace + "SigPolicyURL"
    Public Const SignaturePolicyHash As String = BaseNamespace + "SigPolicyHash"
    Public Const SignaturePolicyOID As String = BaseNamespace + "SigPolicyOID"

    Public Const FileOverwriteOriginal As String = BaseNamespace + "FileOverwriteOriginal"
    Public Const FileCreateNew As String = BaseNamespace + "FileCreateNew"
    Public Const FileNameAppend As String = BaseNamespace + "FileNameAppend"
    Public Const FileExtensionReplace As String = BaseNamespace + "FileExtensionReplace"

    Public Const DocumentViewer As String = BaseNamespace + "DocumentViewer"
    Public Const DefaultCryptographicProvider As String = BaseNamespace + "CryptographicProvider"

    ' PDF Signer properties
    Public Const IsPDFSignerEnabled As String = BaseNamespace + "IsPDFSignerEnabled"
    Public Const SigningOrganization As String = BaseNamespace + "SigningOrganization"
    Public Const SigningReason As String = BaseNamespace + "SigningReason"
    Public Const RevocationCheck As String = BaseNamespace + "RevocationCheck"
    Public Const SignatureHashMethod As String = BaseNamespace + "SignatureHashMethod"
    Public Const AllowQualifiedCertificatesOnly As String = BaseNamespace + "AllowQualifiedCertificatesOnly"

    Public Const IsTimeStampingEnabled As String = BaseNamespace + "TimeStampingEnabled"
    Public Const TSAURL As String = BaseNamespace + "TSAURL"
    Public Const TSAUserName As String = BaseNamespace + "TSAUserName"
    Public Const TSAPassword As String = BaseNamespace + "TSAPassword"
    Public Const IsDocumentTimeStamp As String = BaseNamespace + "DocumentTimeStamp"
    Public Const IsSinglePassPadesBLTA As String = BaseNamespace + "SinglePassPadesBLTA"
    Public Const TimeStampHashMethod As String = BaseNamespace + "TimeStampHashMethod"

    Public Const IsProxyEnabled As String = BaseNamespace + "ProxyEnabled"
    Public Const ProxyServer As String = BaseNamespace + "ProxyServer"
    Public Const ProxyPort As String = BaseNamespace + "ProxyPort"
    Public Const ProxyAuthMethod As String = BaseNamespace + "ProxyAuthMethod"
    Public Const ProxyUserName As String = BaseNamespace + "ProxyUserName"
    Public Const ProxyPassword As String = BaseNamespace + "ProxyPassword"

    ' PDF Streamer properties
    Public Const IsPDFStreamerEnabled As String = BaseNamespace + "IsPDFStreamerEnabled"
    Public Const PDFStreamerURL As String = BaseNamespace + "PDFStreamerURL"
    Public Const PDFStreamerConfigFile As String = BaseNamespace + "PDFStreamerConfigFile"
    Public Const PDFStreamerAuthorizationCode As String = BaseNamespace + "PDFStreamerAuthorizationCode"

    ' MQFTP properties
    Public Const IsMQEnabled As String = BaseNamespace + "IsMQEnabled"
    Public Const MQFolderOut As String = BaseNamespace + "MQFolderOut"
    Public Const MQFolderIn As String = BaseNamespace + "MQFolderIn"
    Public Const MQDomain As String = BaseNamespace + "MQDomain"
    Public Const MQUserName As String = BaseNamespace + "MQUserName"
    Public Const MQPassword As String = BaseNamespace + "MQPassword"
    Public Const MQDocUID As String = BaseNamespace + "MQDocUID"

    ' MNB Signer properties
    Public Const IsMNBSignerEnabled As String = BaseNamespace + "IsMNBSignerEnabled"
    Public Const MNBSignerURL As String = BaseNamespace + "MNBSignerURL"
    Public Const MNBSignerWSTimeout As String = BaseNamespace + "MNBSignerWSTimeout"
    Public Const MNBSignerSigningTimeout As String = BaseNamespace + "MNBSignerSigningTimeout"
    Public Const MNBSignerChunkSize As String = BaseNamespace + "MNBSignerChunkSize"
    Public Const IsMNBSignerWindowsAuthentication As String = BaseNamespace + "IsMNBSignerWindowsAuthentication"
    Public Const IsMNBSignerUserPasswordAuthentication As String = BaseNamespace + "IsMNBSignerUserPasswordAuthentication"
    Public Const MNBSignerDomain As String = BaseNamespace + "MNBSignerDomain"
    Public Const MNBSignerUserName As String = BaseNamespace + "MNBSignerUserName"
    Public Const MNBSignerPassword As String = BaseNamespace + "MNBSignerPassword"
End Module
