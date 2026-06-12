Public Enum IconType As Integer
    Batch = 0
    Document = 1
    Page = 2
    RejectedDocument = 3
    RejectedPage = 4
    SignedDocument = 5
    DocumentToBeSigned = 6
    RejectedDocumentToBeSigned = 7
    NotSignableDocument = 8
    NotSignableRejectedDocument = 9
    SkippedDocument = 10
End Enum

Public Enum MetaDataType As Integer
    Text = 0
    XML = 1
    DublinCoreXML = 2
End Enum

Public Enum CloseSuspendResult As Integer
    Cancel = 0
    Close = 1
    Suspend = 2
End Enum

Public Enum RotateDirection As Integer
    Left = 0
    Right = 1
End Enum

Public Enum HashType As Integer
    None = 0
    SHA1 = &H7101
    SHA224 = &H7107
    SHA256 = &H7104
    SHA384 = &H7105
    SHA512 = &H7106
End Enum

Public Enum RevocationType As Integer
    None = 0
    CRL = 1
    OCSP = 2
    OCSPWithCRLFallback = 3
End Enum

Public Enum ProxyAuthenticationMethod As Integer
    NoAuthentication = 0
    UserPassword = 1
    Digest = 2
    NTLM = 3
End Enum

Public Enum DocumentType As Integer
    None = 0
    UnsignedDocument = 1
    SignedDocument = 2
End Enum

Public Enum SetupExtractMode As Integer
    FullExtract = 0
    WFADataExtract = 1
End Enum

Public Enum DocumentViewerType As Integer
    SimpleViewer = 0
    AdvancedViewer = 1
End Enum

Public Enum CryptoProviderType
    PDFSigner = 0
    PDFStreamer = 1
    MQFTP = 2
    MNBSigner = 3
End Enum
