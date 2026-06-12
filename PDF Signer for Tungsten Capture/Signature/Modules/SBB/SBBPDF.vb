Imports System.IO
Imports System.Linq
Imports System.Text
Imports PDFSignerCommon
Imports SBCertValidator
Imports SBCMS
Imports SBCustomCertStorage
Imports SBHTTPCRL
Imports SBHTTPOCSPClient
Imports SBHTTPSClient
Imports SBHTTPTSPClient
Imports SBPAdES
Imports SBPDF
Imports SBPDFSecurity
Imports SBWinCertStorage
Imports SBX509
Imports SBX509Ext

Friend Class SBBPDF
    Private ReadOnly HTTPSClient As New TElHTTPSClient
    Private ReadOnly TSPClient As New TElHTTPTSPClient
    Private PADESSignatureHandler As TElPDFAdvancedPublicKeySecurityHandler
    Private PADESDocTimeStampHandler As TElPDFAdvancedPublicKeySecurityHandler

    Private ReadOnly _sbSignLog As New StringBuilder
    Private _TrustedRootCertStorage As TElMemoryCertStorage
    Private ReadOnly _CodeTranslator As New SBBCodeTranslator
    Private ReadOnly _CertTranslator As New CertificateTranslator
    Private _docTempFile As String
    Private _fileTempFile As String
    Private _signRequest As SignatureRequest
    Private _settings As PDFSignerCryptoProvider

    Friend Sub Initialize(ProviderSettings As PDFSignerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Sub ActivateLicense()
        SBUtils.Unit.SetLicenseKey("53424E4A41444E58524632303236313131353159554631393534004955475658465444494C465800303030303030303000004A574A30585037483239325A0000")
        SBPDF.Unit.Initialize()
        SBPDFSecurity.Unit.Initialize()
        SBHTTPCRL.Unit.RegisterHTTPCRLRetrieverFactory()
        SBHTTPOCSPClient.Unit.RegisterHTTPOCSPClientFactory()
    End Sub

    Friend Function GetCertificatesFromStore(QualifiedCertificatesOnly As Boolean) As List(Of SigningCertificate)
        Dim res As New List(Of SigningCertificate)
        Dim SystemStore As New TElWinCertStorage

        SystemStore.SystemStores.BeginUpdate()
        SystemStore.SystemStores.Add("MY")
        SystemStore.SystemStores.EndUpdate()

        For i As Integer = 0 To SystemStore.Count - 1
            Dim cert As TElX509Certificate = SystemStore.Certificates(i)
            Dim sigCert As New SigningCertificate

            If cert.ValidFrom < Now AndAlso cert.ValidTo > Now AndAlso (cert.Extensions.KeyUsage.DigitalSignature OrElse cert.Extensions.KeyUsage.NonRepudiation) Then
                Dim isQualified As Boolean = IsCertificateQualified(cert)
                If QualifiedCertificatesOnly AndAlso Not isQualified Then
                    Continue For
                End If

                sigCert.Certificate = cert.ToX509Certificate2(False)
                sigCert.IsQualified = isQualified
                res.Add(sigCert)
            End If
        Next

        TryCast(SystemStore, IDisposable)?.Dispose()
        Return res
    End Function

    Private Function IsCertificateQualified(Certificate As TElX509Certificate) As Boolean
        If Certificate.Extensions IsNot Nothing Then
            If Certificate.Extensions.QualifiedStatements IsNot Nothing Then
                Return Certificate.Extensions.QualifiedStatements.EuQcCompliance
            End If
        End If

        Return False
    End Function

    Friend Function SignDocument(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult

        _signRequest = Request
        _TrustedRootCertStorage = New TElMemoryCertStorage

        Dim doc As New TElPDFDocument
        PADESSignatureHandler = New TElPDFAdvancedPublicKeySecurityHandler With
            {
                .PAdESSignatureType = TSBPAdESSignatureType.pastEnhanced,
                .CustomName = "Adobe.PPKMS"
            }
        PADESDocTimeStampHandler = New TElPDFAdvancedPublicKeySecurityHandler With
            {
                .PAdESSignatureType = TSBPAdESSignatureType.pastDocumentTimestamp,
                .CustomName = "Adobe.PPKMS"
            }

        Dim CertStorage As New TElMemoryCertStorage
        Dim SystemStore As New TElWinCertStorage
        Dim sig As TElPDFSignature

        '********** Setting up HTTP client **********

        HTTPSClient.DNS.Enabled = False
        HTTPSClient.AutoValidateCertificates = False ' causes skipping TLS certificate validation, which doesn't work in SBB at the moment
        HTTPSClient.SSLEnabled = True
        HTTPSClient.UseHTTPProxy = _settings.IsProxyEnabled
        HTTPSClient.HTTPProxyHost = _settings.ProxyServer.ToString
        HTTPSClient.HTTPProxyPort = _settings.ProxyPort
        HTTPSClient.HTTPProxyAuthentication = _settings.ProxyAuthMethod
        HTTPSClient.HTTPProxyUsername = _settings.ProxyUserName
        HTTPSClient.HTTPProxyPassword = _settings.ProxyPassword

        AddHandler HTTPSClient.OnCertificateValidate, AddressOf HTTPSClient_OnCertificateValidate
        AddHandler HTTPSClient.OnCertValidatorFinished, AddressOf HTTPSClient_OnCertValidatorFinished
        AddHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
        AddHandler PADESSignatureHandler.OnCertValidatorFinished, AddressOf PADESHandler_OnCertValidatorFinished
        AddHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
        AddHandler PADESDocTimeStampHandler.OnCertValidatorFinished, AddressOf PADESHandler_OnCertValidatorFinished
        AddHandler TSPClient.OnBeforeSign, AddressOf TSPClient_OnBeforeSign
        AddHandler TSPClient.OnCertificateValidate, AddressOf TSPClient_OnCertificateValidate
        AddHandler TSPClient.OnHTTPError, AddressOf TSPClient_OnHTTPError
        AddHandler TSPClient.OnTSPError, AddressOf TSPClient_OnTSPError

        '********** "Off-line validating signing certificate" **********

        Try
            _sbSignLog.AppendLine("Aláíró tanúsítvány off-line ellenőrzése")

            ' search for signing certificate
            SystemStore.SystemStores.Add("ROOT")
            For i As Integer = 0 To SystemStore.Count - 1
                _TrustedRootCertStorage.Add(SystemStore.Certificates(i), False)
            Next
            SystemStore.SystemStores.Add("CA")
            SystemStore.SystemStores.Add("MY")

            Dim digest As New SBTypes.TMessageDigest160()
            SBBufferUtils.Unit.StrToDigest160(Request.SigningCertificate.Certificate.Thumbprint, digest)
            Dim certIndex As Integer = SystemStore.FindByHashSHA1(digest)

            If certIndex < 0 Then
                res.ErrorMessage = $"Nem található '{Request.SigningCertificate.Certificate.Thumbprint}' thumbprint-tel rendelkező tanúsítvány a bejelentkezett felhasználó 'MY' tanúsítvány tárában!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            ' signing certificate found
            Dim Cert As TElX509Certificate = SystemStore.Certificates(certIndex)

            ' basic validation of signing certificate
            If Cert.ValidFrom > Date.Now Then
                res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítvány csak {Cert.ValidFrom} után érvényes!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            If Cert.ValidTo < Date.Now Then
                res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítvány már nem érvényes (lejárt: {Cert.ValidTo})!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            If Not Cert.PrivateKeyExists Then
                res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítvány nem tartalmaz privát kulcsot! Privát kulcs nélkül nem lehet aláírást létrehozni!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            If Not (Cert.Extensions.KeyUsage.DigitalSignature OrElse Cert.Extensions.KeyUsage.NonRepudiation) Then
                res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítvány nem használható elektronikus aláírás készítésére (nincs beállítva a 'Digital Signature' vagy 'Non Repudiation' flag)!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            ' building certificate chain
            Dim chain As TElX509CertificateChain = SystemStore.BuildChain(Cert)

            ' if chain is incomplete (last certificate not self-signed) then abort
            If Not chain.Complete Then
                res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítványhoz tartozó tanúsítványlánc nem építhető fel megbízható gyökértanúsítványig!"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End If

            ' checking if certifiacate revocation can be checked via OCSP, if it is a requirement
            If _settings.RevocationCheck = RevocationType.OCSP Then
                Dim HasOCSPResponder As Boolean

                For j As Integer = 0 To Cert.Extensions.AuthorityInformationAccess.Count - 1
                    Dim accDesc As TElAccessDescription = Cert.Extensions.AuthorityInformationAccess.AccessDescriptions(j)
                    If accDesc.AccessMethod.SequenceEqual(SBConstants.__Global.SB_OID_ACCESS_METHOD_OCSP.Data) Then
                        HasOCSPResponder = True
                        Exit For
                    End If
                Next

                If Not HasOCSPResponder Then
                    res.ErrorMessage = $"'{Cert.SubjectName.CommonName}' tanúsítvány visszavonási állapota nem ellenőrizhető OCSP-vel, de a dokumentumtípus beállításai megkövetelik!"
                    _sbSignLog.AppendLine(res.ErrorMessage)
                    res.SignatureLog = _sbSignLog.ToString
                    Return res
                End If
            End If

            ' adding certificate chain members to the MemoryCertStorage that will be later added to the signature
            For i As Integer = 0 To chain.Count - 1
                CertStorage.Add(chain.Certificates(i), False)
            Next


            '********** Signing PDF document **********

            If _settings.IsTimeStampingEnabled Then
                _sbSignLog.AppendLine("PDF dokumentum megnyitása aláíráshoz és időbélyegzéshez")
            Else
                _sbSignLog.AppendLine("PDF dokumentum megnyitása aláíráshoz")
            End If

            ' Override SBB MemoryStream if file size is above threshold
            If _signRequest.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
                Dim fo As New FileOperation
                _docTempFile = fo.GetTempFile("ds-")
                _fileTempFile = fo.GetTempFile("fs-")

                AddHandler doc.OnCreateTemporaryStream, AddressOf DocOnCreateTemporaryStream
                AddHandler doc.PDFFile.OnCreateTemporaryStream, AddressOf FileOnCreateTemporaryStream
            End If

            Try
                Request.FileToSign.Seek(0, SeekOrigin.Begin)
                doc.Open(Request.FileToSign)
            Catch ex As Exception
                res.ErrorMessage = $"Hiba a PDF dokumentum aláíráskori megnyitása közben: {ex.Message}"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End Try


            '********** Adding signature **********

            _sbSignLog.AppendLine("Aláírás paraméterezése")

            Dim index As Integer = doc.AddSignature()
            sig = doc.Signatures(index)
            sig.Handler = PADESSignatureHandler
            sig.AuthorName = _settings.SigningOrganization
            sig.SigningTime = Date.Now.ToUniversalTime()
            sig.Reason = _settings.SigningReason
            sig.Invisible = True

            PADESSignatureHandler.AutoCollectRevocationInfo = False
            PADESSignatureHandler.IgnoreChainValidationErrors = True
            PADESSignatureHandler.ForceCompleteChainValidation = False
            PADESSignatureHandler.DeepValidation = False
            PADESSignatureHandler.HashAlgorithm = _settings.SignatureHashMethod
            PADESSignatureHandler.CertStorage = CertStorage

            '********** Adding EPES information **********

            If Request.MetaDataSettings.SignaturePolicyURL IsNot Nothing Then
                With PADESSignatureHandler.CMS.Signatures(0)
                    .CommitmentTypeIndication.ProofOfApproval = True
                    .SignaturePolicy.Included = True
                    .SignaturePolicy.PolicyIdentifier = Encoding.ASCII.GetBytes(Request.MetaDataSettings.SignaturePolicyOID)
                    .SignaturePolicy.PolicyHash.HashValue = Convert.FromBase64String(Request.MetaDataSettings.SignaturePolicyHash)
                    .SignaturePolicy.PolicyHash.HashAlgorithm = _settings.SignatureHashMethod
                    .SignaturePolicy.AddQualifier()
                    .SignaturePolicy.Qualifiers(0).URI = Request.MetaDataSettings.SignaturePolicyURL.ToString
                    .SignaturePolicy.Qualifiers(0).QualifierType = TSBSigPolicyQualifierType.spqtURI
                End With
            End If


            '********** Adding timestamp **********

            If _settings.IsTimeStampingEnabled Then
                _sbSignLog.AppendLine("Időbélyeg paraméterezése")

                If Not String.IsNullOrEmpty(_settings.TSAUserName) Then
                    HTTPSClient.RequestParameters.Username = _settings.TSAUserName
                    HTTPSClient.RequestParameters.Password = _settings.TSAPassword
                End If

                TSPClient.HTTPClient = HTTPSClient
                TSPClient.Options = SBTSPClient.Unit.tsoIgnoreBadNonce
                TSPClient.IncludeCertificates = True
                TSPClient.URL = _settings.TSAURL.ToString
                TSPClient.HashAlgorithm = _settings.TimeStampHashMethod

                ' Create embedded time stamp when
                ' document time stamp and single pass PAdES B-LTA is disabled or when
                ' both document time stamp and single pass PAdES B-LTA is enabled
                If Not _settings.IsDocumentTimeStamp AndAlso Not _settings.IsSinglePassPadesBLTA OrElse
                    _settings.IsDocumentTimeStamp AndAlso _settings.IsSinglePassPadesBLTA Then

                    PADESSignatureHandler.TSPClient = TSPClient
                End If
            End If

            _sbSignLog.AppendLine()

            If _settings.IsProxyEnabled Then
                _sbSignLog.AppendLine($"HTTP proxy szerver {_settings.ProxyServer}:{_settings.ProxyPort}, felhasználó: '{_settings.ProxyUserName}', autentikáció: {_CodeTranslator.ProxyAuthMethodToString(_settings.ProxyAuthMethod)}")
            End If

            _sbSignLog.AppendLine("Hitelesítési művelet megkezdése")

            If _settings.IsTimeStampingEnabled Then
                _sbSignLog.AppendLine($"Időbélyeg szolgáltató URL: {_settings.TSAURL}, felhasználó: '{_settings.TSAUserName}'")
            End If

            doc.Close(True)

            _sbSignLog.AppendLine("Hitelesítési művelet befejezve")

            '********** Embedding revocation information **********

            ' We only embed revocation information if revocation checking should be performed
            ' either on the signature or on the time stamp
            If _settings.RevocationCheck <> RevocationType.None Then
                _sbSignLog.AppendLine()
                _sbSignLog.AppendLine("PDF dokumentum megnyitása PAdES szabványú aláírások/időbélyegek visszavonási információinak beszerzéséhez")
                Try
                    Request.FileToSign.Seek(0, SeekOrigin.Begin)
                    doc.Open(Request.FileToSign)
                Catch ex As Exception
                    res.ErrorMessage = $"Hiba a PDF dokumentum visszavonási információk beágyazásához történő megnyitása közben: {ex.Message}"
                    _sbSignLog.AppendLine(res.ErrorMessage)
                    res.SignatureLog = _sbSignLog.ToString
                    Return res
                End Try

                For i As Integer = doc.SignatureCount - 1 To 0 Step -1
                    sig = doc.Signatures(i)
                    If (TypeOf sig.Handler Is TElPDFAdvancedPublicKeySecurityHandler) Then
                        PADESSignatureHandler = CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler)
                        RemoveHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                        AddHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                        PADESSignatureHandler.AutoCollectRevocationInfo = True
                        PADESSignatureHandler.IgnoreChainValidationErrors = False
                        PADESSignatureHandler.ForceCompleteChainValidation = True
                        PADESSignatureHandler.DeepValidation = True

                        _sbSignLog.AppendLine($"{i + 1}. aláíráshoz/időbélyeghez kapcsolódó visszavonási információk beszerzése")
                        Try
                            sig.Update()
                        Catch ex As Exception
                            res.ErrorMessage = $"Hiba a(z) {i + 1}. aláíráshoz/időbélyeghez kapcsolódó visszavonási információk beágyazása közben: {ex.Message}"
                            _sbSignLog.AppendLine(res.ErrorMessage)
                            res.SignatureLog = _sbSignLog.ToString
                            Return res
                        End Try

                    End If
                Next

                doc.Close(True)

                _sbSignLog.AppendLine("PAdES szabványú aláírások/időbélyegek visszavonási információi beágyazva")
            End If

            '********** Adding document time stamp **********

            If _settings.IsTimeStampingEnabled AndAlso _settings.IsDocumentTimeStamp Then
                _sbSignLog.AppendLine("PDF dokumentum megnyitása dokumentumszintű időbélyeg készítéséhez")
                Try
                    Request.FileToSign.Seek(0, SeekOrigin.Begin)
                    doc.Open(Request.FileToSign)
                Catch ex As Exception
                    res.ErrorMessage = $"Hiba a PDF dokumentum dokumentumszintű időbélyeg készítéséhez történő megnyitása közben: {ex.Message}"
                    _sbSignLog.AppendLine(res.ErrorMessage)
                    res.SignatureLog = _sbSignLog.ToString
                    Return res
                End Try

                ' create document time stamp
                Dim ind As Integer = doc.AddSignature
                sig = doc.Signatures(ind)
                sig.Handler = PADESDocTimeStampHandler
                If _settings.RevocationCheck = RevocationType.None Then
                    PADESDocTimeStampHandler.AutoCollectRevocationInfo = False
                    PADESDocTimeStampHandler.IgnoreChainValidationErrors = True
                    PADESDocTimeStampHandler.ForceCompleteChainValidation = False
                    PADESDocTimeStampHandler.DeepValidation = False
                Else
                    PADESDocTimeStampHandler.AutoCollectRevocationInfo = True
                    PADESDocTimeStampHandler.IgnoreChainValidationErrors = False
                    PADESDocTimeStampHandler.ForceCompleteChainValidation = True
                    PADESDocTimeStampHandler.DeepValidation = True
                End If

                PADESDocTimeStampHandler.HashAlgorithm = _settings.TimeStampHashMethod
                PADESDocTimeStampHandler.TSPClient = TSPClient
                sig.SigningTime = Date.UtcNow
                sig.Invisible = True

                doc.Close(True)

                _sbSignLog.AppendLine("Dokumentumszintű időbélyeg elkészítve")

                ' We only embed revocation information if revocation checking should be performed on time stamps
                If _settings.RevocationCheck <> RevocationType.None Then
                    _sbSignLog.AppendLine()
                    _sbSignLog.AppendLine("PDF dokumentum megnyitása az utolsó dokumentumszintű időbélyeg visszavonási információinak beszerzéséhez")
                    Try
                        Request.FileToSign.Seek(0, SeekOrigin.Begin)
                        doc.Open(Request.FileToSign)
                    Catch ex As Exception
                        res.ErrorMessage = $"Hiba a PDF dokumentum utolsó dokumentumszintű időbélyegének visszavonási információinak beágyazásához történő megnyitása közben: {ex.Message}"
                        _sbSignLog.AppendLine(res.ErrorMessage)
                        res.SignatureLog = _sbSignLog.ToString
                        Return res
                    End Try

                    sig = doc.Signatures(doc.SignatureCount - 1)
                    If (TypeOf sig.Handler Is TElPDFAdvancedPublicKeySecurityHandler) Then
                        PADESDocTimeStampHandler = CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler)
                        If PADESDocTimeStampHandler.PAdESSignatureType = TSBPAdESSignatureType.pastDocumentTimestamp Then
                            RemoveHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                            AddHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
                            PADESDocTimeStampHandler.AutoCollectRevocationInfo = True
                            PADESDocTimeStampHandler.IgnoreChainValidationErrors = False
                            PADESDocTimeStampHandler.ForceCompleteChainValidation = True
                            PADESDocTimeStampHandler.DeepValidation = True

                            _sbSignLog.AppendLine("Utolsó dokumentumszintű időbélyeghez kapcsolódó visszavonási információk beszerzése")
                            Try
                                sig.Update()
                            Catch ex As Exception
                                res.ErrorMessage = $"Hiba az utolsó dokumentumszintű időbélyeghez kapcsolódó visszavonási információk beszerzése közben: {ex.Message}"
                                _sbSignLog.AppendLine(res.ErrorMessage)
                                res.SignatureLog = _sbSignLog.ToString
                                Return res
                            End Try

                        End If

                    End If

                    doc.Close(True)

                    _sbSignLog.AppendLine("PDF dokumentum utolsó dokumentumszintű időbélyegének visszavonási információi beágyazva")
                End If

            End If

            '********** Determining signature validity **********

            _sbSignLog.AppendLine()
            _sbSignLog.AppendLine("PDF dokumentum megnyitása hitelesség lejárati idejének meghatározásához")
            Try
                Request.FileToSign.Seek(0, SeekOrigin.Begin)
                doc.Open(Request.FileToSign)
            Catch ex As Exception
                res.ErrorMessage = $"Hiba a PDF dokumentum hitelességi lejárati idejének meghatározására történő megnyitása közben: {ex.Message}"
                _sbSignLog.AppendLine(res.ErrorMessage)
                res.SignatureLog = _sbSignLog.ToString
                Return res
            End Try

            _sbSignLog.AppendLine("Hitelesség lejárati idejének meghatározása")
            Dim docValidity As Date = Date.MaxValue
            For i As Integer = doc.SignatureCount - 1 To 0 Step -1
                sig = doc.Signatures(i)
                If TypeOf sig.Handler Is TElPDFAdvancedPublicKeySecurityHandler AndAlso CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler).PAdESSignatureType = TSBPAdESSignatureType.pastDocumentTimestamp Then
                    ' if there's a document timestamp, then we're only interested in the last one
                    If i = (doc.SignatureCount - 1) Then
                        PADESDocTimeStampHandler = CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler)
                        docValidity = GetSigningCertificateValidity(PADESDocTimeStampHandler.CMS.Certificates)
                        Exit For
                    End If

                End If

                Dim sigValidity As Date
                ' cycle through all non-document time stamp advanced signatures
                ' and select the one which expires soonest
                If TypeOf sig.Handler Is TElPDFAdvancedPublicKeySecurityHandler AndAlso CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler).PAdESSignatureType <> TSBPAdESSignatureType.pastDocumentTimestamp Then
                    PADESSignatureHandler = CType(sig.Handler, TElPDFAdvancedPublicKeySecurityHandler)
                    sigValidity = GetSigningCertificateValidity(PADESSignatureHandler.CMS.Certificates)
                    Dim tsValidity As Date = Date.MinValue
                    For j As Integer = 0 To PADESSignatureHandler.CMS.Signatures(0).TimestampCount - 1
                        Dim currTsValidity = GetSigningCertificateValidity(PADESSignatureHandler.CMS.Signatures(0).Timestamps(j).Certificates)
                        If currTsValidity > tsValidity Then
                            tsValidity = currTsValidity
                        End If
                    Next

                    If tsValidity > sigValidity Then
                        sigValidity = tsValidity
                    End If

                    If docValidity > sigValidity Then
                        docValidity = sigValidity
                    End If
                End If

                ' cycle through all PKCS7 or X509RSA signatures
                ' and select the one which expires soonest
                If TypeOf sig.Handler Is TElPDFPublicKeySecurityHandler AndAlso TypeOf sig.Handler IsNot TElPDFAdvancedPublicKeySecurityHandler Then
                    Dim PDFSignatureHandler As TElPDFPublicKeySecurityHandler = CType(sig.Handler, TElPDFPublicKeySecurityHandler)
                    sigValidity = GetSigningCertificateValidity(PDFSignatureHandler.Certificates)
                    Dim tsValidity As Date = Date.MinValue
                    For j As Integer = 0 To PDFSignatureHandler.TimestampCount - 1
                        Dim currTsValidity = GetSigningCertificateValidity(PDFSignatureHandler.Timestamps(j).Certificates)
                        If currTsValidity > tsValidity Then
                            tsValidity = currTsValidity
                        End If
                    Next

                    If tsValidity > sigValidity Then
                        sigValidity = tsValidity
                    End If

                    If docValidity > sigValidity Then
                        docValidity = sigValidity
                    End If

                End If
            Next

            Dim docValidUntil As Date = IIf(docValidity <> Date.MaxValue, docValidity, Date.MinValue)
            _sbSignLog.AppendLine($"Hitelesség lejárati ideje meghatározva: {docValidUntil:yyyy.MM.dd. HH:mm:ss} (UTC)")

            doc.Close(False)

            res.SignedFile = Request.FileToSign
            res.SignatureExpiration = docValidUntil
            res.SignatureLog = _sbSignLog.ToString

            Return res

        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = _sbSignLog.ToString
            Return res

        Finally
            RemoveHandler HTTPSClient.OnCertificateValidate, AddressOf HTTPSClient_OnCertificateValidate
            RemoveHandler HTTPSClient.OnCertValidatorFinished, AddressOf HTTPSClient_OnCertValidatorFinished
            RemoveHandler PADESSignatureHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
            RemoveHandler PADESSignatureHandler.OnCertValidatorFinished, AddressOf PADESHandler_OnCertValidatorFinished
            RemoveHandler PADESDocTimeStampHandler.OnCertValidatorPrepared, AddressOf PADESHandler_OnCertValidatorPrepared
            RemoveHandler PADESDocTimeStampHandler.OnCertValidatorFinished, AddressOf PADESHandler_OnCertValidatorFinished
            RemoveHandler TSPClient.OnBeforeSign, AddressOf TSPClient_OnBeforeSign
            RemoveHandler TSPClient.OnCertificateValidate, AddressOf TSPClient_OnCertificateValidate
            RemoveHandler TSPClient.OnHTTPError, AddressOf TSPClient_OnHTTPError
            RemoveHandler TSPClient.OnTSPError, AddressOf TSPClient_OnTSPError

            If _signRequest.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
                Try
                    If File.Exists(_docTempFile) Then File.Delete(_docTempFile)
                    If File.Exists(_fileTempFile) Then File.Delete(_fileTempFile)

                    RemoveHandler doc.OnCreateTemporaryStream, AddressOf DocOnCreateTemporaryStream
                    RemoveHandler doc.PDFFile.OnCreateTemporaryStream, AddressOf FileOnCreateTemporaryStream
                Catch
                End Try
            End If

            ' release SBB objects deterministically (types that implement IDisposable)
            Try
                TryCast(doc, IDisposable)?.Dispose()
                TryCast(SystemStore, IDisposable)?.Dispose()
                TryCast(CertStorage, IDisposable)?.Dispose()
                TryCast(_TrustedRootCertStorage, IDisposable)?.Dispose()
                TryCast(HTTPSClient, IDisposable)?.Dispose()
                TryCast(TSPClient, IDisposable)?.Dispose()
            Catch
            End Try

        End Try

    End Function

    Private Sub TSPClient_OnBeforeSign(Sender As Object, Signer As Object)
        _sbSignLog.AppendLine($"  Időbélyeg kérése, TSA URL: {_settings.TSAURL}")
    End Sub

    Private Sub TSPClient_OnCertificateValidate(Sender As Object, Certificate As TElX509Certificate, AdditionalCertificates As TElCustomCertStorage, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer, ByRef DoContinue As Boolean)
        _sbSignLog.AppendLine($"  TSP tanúsítvány: '{_CertTranslator.GetIssuedToName(Certificate)}' (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}')")
    End Sub

    Private Sub TSPClient_OnHTTPError(Sender As Object, ResponseCode As Integer)
        _sbSignLog.AppendLine($"  TSP HTTP hiba, HTTP hibakód: {ResponseCode}")
    End Sub

    Private Sub TSPClient_OnTSPError(Sender As Object, ResultCode As Integer, ServerResult As Integer, FailureInfo As Integer, StatusString As String)
        _sbSignLog.AppendLine($"  TSP hiba, hibakód: {_CodeTranslator.GetTSPError(ResultCode)}, szerver oldali eredmény: {_CodeTranslator.GetPKIStatusCode(ServerResult)}, szerver hibakód: {FailureInfo}, státusz: {StatusString}")
    End Sub

    Private Sub DocOnCreateTemporaryStream(Sender As Object, ByRef Stream As Stream, ByRef FreeOnClose As Boolean)
        If _signRequest.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
            Stream = New FileStream(_docTempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
            FreeOnClose = True
        End If
    End Sub

    Private Sub FileOnCreateTemporaryStream(Sender As Object, ByRef Stream As Stream, ByRef FreeOnClose As Boolean)
        If _signRequest.DocItem.FileSize > Constant.MemoryStreamSizeLimit Then
            Stream = New FileStream(_fileTempFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None)
            FreeOnClose = True
        End If
    End Sub

    Private Function GetSigningCertificateValidity(Certificates As TElCustomCertStorage) As Date
        Dim sigChain As TElX509CertificateChain = Certificates.BuildChain(0)
        Dim sigCert As TElX509Certificate = sigChain.Certificates(0)
        Return sigCert.ValidTo
    End Function

    Private Sub HTTPSClient_OnCertificateValidate(Sender As Object, X509Certificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        Validity = TSBCertificateValidity.cvOk
    End Sub

    Private Sub HTTPSClient_OnCertValidatorFinished(Sender As Object, CertValidator As TElX509CertificateValidator, Cert As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer)
        If Validity <> TSBCertificateValidity.cvOk Then
            _sbSignLog.AppendLine(CertValidator.InternalLogger.Log.Text)
        End If
    End Sub

#Region "Certificate Validator events"

    Private Sub PADESHandler_OnCertValidatorPrepared(Sender As Object, ByRef CertValidator As TElX509CertificateValidator, Cert As TElX509Certificate)

        CertValidator.AddTrustedCertificates(_TrustedRootCertStorage)

        If _settings.RevocationCheck = RevocationType.None Then
            CertValidator.MandatoryRevocationCheck = False
            CertValidator.CheckCRL = False
            CertValidator.CheckOCSP = False
            CertValidator.MandatoryCRLCheck = False
            CertValidator.MandatoryOCSPCheck = False
            CertValidator.RevocationCheckPreference = TSBX509RevocationCheckPreference.rcpPreferOCSP
        End If

        If _settings.RevocationCheck = RevocationType.CRL Then
            CertValidator.MandatoryRevocationCheck = True
            CertValidator.MandatoryCRLCheck = True
            CertValidator.MandatoryOCSPCheck = False
            CertValidator.RevocationCheckPreference = TSBX509RevocationCheckPreference.rcpPreferCRL
        End If

        If _settings.RevocationCheck = RevocationType.OCSP Then
            CertValidator.MandatoryRevocationCheck = True
            CertValidator.MandatoryCRLCheck = False
            CertValidator.MandatoryOCSPCheck = True
            CertValidator.RevocationCheckPreference = TSBX509RevocationCheckPreference.rcpPreferOCSP
        End If

        If _settings.RevocationCheck = RevocationType.OCSPWithCRLFallback Then
            CertValidator.MandatoryRevocationCheck = True
            CertValidator.MandatoryCRLCheck = False
            CertValidator.MandatoryOCSPCheck = False
            CertValidator.RevocationCheckPreference = TSBX509RevocationCheckPreference.rcpPreferOCSP
        End If

        '********** Start validating certificates **********

        _sbSignLog.AppendLine($"'{_CertTranslator.GetIssuedToName(Cert)}' tanúsítványláncának ellenőrzése")

        ' the same validator instance can be prepared multiple times per run; avoid double subscription
        RemoveHandler CertValidator.OnBeforeCRLRetrieverUse, AddressOf CertValidator_OnBeforeCRLRetrieverUse
        RemoveHandler CertValidator.OnBeforeOCSPClientUse, AddressOf CertValidator_OnBeforeOCSPClientUse
        RemoveHandler CertValidator.OnCRLError, AddressOf CertValidator_OnCRLError
        RemoveHandler CertValidator.OnCRLNeeded, AddressOf CertValidator_OnCRLNeeded
        RemoveHandler CertValidator.OnCRLRetrieved, AddressOf CertValidator_OnCRLRetrieved
        RemoveHandler CertValidator.OnOCSPError, AddressOf CertValidator_OnOCSPError
        RemoveHandler CertValidator.OnAfterCRLUse, AddressOf CertValidator_OnAfterCRLUse
        RemoveHandler CertValidator.OnAfterOCSPResponseUse, AddressOf CertValidator_OnAfterOCSPResponseUse
        RemoveHandler CertValidator.OnBeforeCertificateValidation, AddressOf CertValidator_OnBeforeCertificateValidation
        RemoveHandler CertValidator.OnAfterCertificateValidation, AddressOf CertValidator_OnAfterCertificateValidation
        AddHandler CertValidator.OnBeforeCRLRetrieverUse, AddressOf CertValidator_OnBeforeCRLRetrieverUse
        AddHandler CertValidator.OnBeforeOCSPClientUse, AddressOf CertValidator_OnBeforeOCSPClientUse
        AddHandler CertValidator.OnCRLError, AddressOf CertValidator_OnCRLError
        AddHandler CertValidator.OnCRLNeeded, AddressOf CertValidator_OnCRLNeeded
        AddHandler CertValidator.OnCRLRetrieved, AddressOf CertValidator_OnCRLRetrieved
        AddHandler CertValidator.OnOCSPError, AddressOf CertValidator_OnOCSPError
        AddHandler CertValidator.OnAfterCRLUse, AddressOf CertValidator_OnAfterCRLUse
        AddHandler CertValidator.OnAfterOCSPResponseUse, AddressOf CertValidator_OnAfterOCSPResponseUse
        AddHandler CertValidator.OnBeforeCertificateValidation, AddressOf CertValidator_OnBeforeCertificateValidation
        AddHandler CertValidator.OnAfterCertificateValidation, AddressOf CertValidator_OnAfterCertificateValidation
    End Sub

    Private Sub PADESHandler_OnCertValidatorFinished(Sender As Object, CertValidator As TElX509CertificateValidator, Cert As TElX509Certificate, Validity As TSBCertificateValidity, Reason As Integer)
        _sbSignLog.AppendLine($"'{_CertTranslator.GetIssuedToName(Cert)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Cert)}') lánca ellenőrizve, érvényesség: {_CodeTranslator.GetCertError(Validity)}{_CodeTranslator.GetCertErrorReasonList(Reason)}")
        _sbSignLog.AppendLine()

        If Validity <> TSBCertificateValidity.cvOk Then
            _sbSignLog.AppendLine(CertValidator.InternalLogger.Log.Text)
        End If

        ' the validator is finished; unhook everything we attached in OnCertValidatorPrepared
        RemoveHandler CertValidator.OnBeforeCRLRetrieverUse, AddressOf CertValidator_OnBeforeCRLRetrieverUse
        RemoveHandler CertValidator.OnBeforeOCSPClientUse, AddressOf CertValidator_OnBeforeOCSPClientUse
        RemoveHandler CertValidator.OnCRLError, AddressOf CertValidator_OnCRLError
        RemoveHandler CertValidator.OnCRLNeeded, AddressOf CertValidator_OnCRLNeeded
        RemoveHandler CertValidator.OnCRLRetrieved, AddressOf CertValidator_OnCRLRetrieved
        RemoveHandler CertValidator.OnOCSPError, AddressOf CertValidator_OnOCSPError
        RemoveHandler CertValidator.OnAfterCRLUse, AddressOf CertValidator_OnAfterCRLUse
        RemoveHandler CertValidator.OnAfterOCSPResponseUse, AddressOf CertValidator_OnAfterOCSPResponseUse
        RemoveHandler CertValidator.OnBeforeCertificateValidation, AddressOf CertValidator_OnBeforeCertificateValidation
        RemoveHandler CertValidator.OnAfterCertificateValidation, AddressOf CertValidator_OnAfterCertificateValidation
    End Sub

    Private Sub CertValidator_OnBeforeCertificateValidation(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate)
        _sbSignLog.AppendLine($" '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány ellenőrzése (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}')")
    End Sub

    Private Sub CertValidator_OnAfterOCSPResponseUse(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, Response As SBOCSPClient.TElOCSPResponse)
        _sbSignLog.AppendLine($"  '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') OCSP válasza kiértékelve")
    End Sub

    Private Sub CertValidator_OnAfterCRLUse(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, CRL As SBCRL.TElAbstractCRL)
        _sbSignLog.AppendLine($"  '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') CRL-je kiértékelve")
    End Sub

    Private Sub CertValidator_OnOCSPError(Sender As Object, Certificate As TElX509Certificate, Location As String, Client As SBOCSPClient.TElOCSPClient, ErrorCode As Integer)
        'If TypeOf (Client) Is TElHTTPOCSPClient Then
        '    Dim clnt As TElHTTPOCSPClient = CType(Client, TElHTTPOCSPClient)
        '    Dim buff As Byte() = New Byte(10000) {}
        '    clnt.Response.SaveFull(buff, 0, 10000)
        '    File.WriteAllBytes("c:\temp\OCSPresp", buff)
        'End If
        _sbSignLog.AppendLine($"  OCSP hiba a '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') ellenőrzésekor. Hely: {Location}, hibakód: {_CodeTranslator.GetOCSPError(ErrorCode)}")
    End Sub

    Private Sub CertValidator_OnCRLRetrieved(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, NameType As SBX509Ext.TSBGeneralName, Location As String, CRL As SBCRL.TElAbstractCRL)
        _sbSignLog.AppendLine($"  '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') CRL-je sikeresen letöltve. Hely: {Location}")
    End Sub

    Private Sub CertValidator_OnCRLNeeded(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, ByRef CRLs As SBCRLStorage.TElCustomCRLStorage)
        _sbSignLog.AppendLine($"  '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') ellenőrzése CRL-lel")
    End Sub

    Private Sub CertValidator_OnCRLError(Sender As Object, Certificate As TElX509Certificate, Location As String, Retriever As SBCRLStorage.TElCustomCRLRetriever, ErrorCode As Integer)
        _sbSignLog.AppendLine($"  CRL hiba a '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') ellenőrzésekor. Hely: {Location}, hibakód: {_CodeTranslator.GetCRLError(ErrorCode)}")
    End Sub

    Private Sub CertValidator_OnBeforeOCSPClientUse(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, OCSPLocation As String, ByRef OCSPClient As SBOCSPClient.TElOCSPClient)
        If TypeOf OCSPClient Is TElHTTPOCSPClient Then
            Dim clnt As TElHTTPOCSPClient = CType(OCSPClient, TElHTTPOCSPClient)

            clnt.HTTPClient.DNS.Enabled = False
            clnt.HTTPClient.AutoValidateCertificates = False ' causes skipping TLS certificate validation, which doesn't work in SBB at the moment
            clnt.HTTPClient.UseHTTPProxy = _settings.IsProxyEnabled
            clnt.HTTPClient.HTTPProxyHost = _settings.ProxyServer.ToString
            clnt.HTTPClient.HTTPProxyPort = _settings.ProxyPort
            clnt.HTTPClient.HTTPProxyAuthentication = _settings.ProxyAuthMethod
            clnt.HTTPClient.HTTPProxyUsername = _settings.ProxyUserName
            clnt.HTTPClient.HTTPProxyPassword = _settings.ProxyPassword

            If _settings.IsProxyEnabled Then
                _sbSignLog.AppendLine($"  HTTP proxy szerver {_settings.ProxyServer}:{_settings.ProxyPort} , felhasználó: '{_settings.ProxyUserName}', autentikáció: {_CodeTranslator.ProxyAuthMethodToString(_settings.ProxyAuthMethod)} beállítva OCSP letöltéshez")
            End If
        End If

        _sbSignLog.AppendLine($"  OCSP válasz letöltése '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítványhoz (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}'). URL: {OCSPLocation}")
    End Sub

    Private Sub CertValidator_OnBeforeCRLRetrieverUse(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, NameType As SBX509Ext.TSBGeneralName, Location As String, ByRef Retriever As SBCRLStorage.TElCustomCRLRetriever)
        If TypeOf Retriever Is TElHTTPCRLRetriever Then
            Dim retr As TElHTTPCRLRetriever = CType(Retriever, TElHTTPCRLRetriever)

            retr.HTTPClient.DNS.Enabled = False
            retr.HTTPClient.AutoValidateCertificates = False ' causes skipping TLS certificate validation, which doesn't work in SBB at the moment
            retr.HTTPClient.UseHTTPProxy = _settings.IsProxyEnabled
            retr.HTTPClient.HTTPProxyHost = _settings.ProxyServer.ToString
            retr.HTTPClient.HTTPProxyPort = _settings.ProxyPort
            retr.HTTPClient.HTTPProxyAuthentication = _settings.ProxyAuthMethod
            retr.HTTPClient.HTTPProxyUsername = _settings.ProxyUserName
            retr.HTTPClient.HTTPProxyPassword = _settings.ProxyPassword

            If _settings.IsProxyEnabled Then
                _sbSignLog.AppendLine($"  HTTP proxy szerver {_settings.ProxyServer}:{_settings.ProxyPort} , felhasználó: '{_settings.ProxyUserName}', autentikáció: {_CodeTranslator.ProxyAuthMethodToString(_settings.ProxyAuthMethod)} beállítva CRL letöltéshez")
            End If
        End If

        _sbSignLog.AppendLine($"  CRL letöltése '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítványhoz (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}'). URL: {Location}")
    End Sub

    Private Sub CertValidator_OnAfterCertificateValidation(Sender As Object, Certificate As TElX509Certificate, CACertificate As TElX509Certificate, ByRef Validity As TSBCertificateValidity, ByRef Reason As Integer, ByRef DoContinue As Boolean)
        _sbSignLog.AppendLine($" '{_CertTranslator.GetIssuedToName(Certificate)}' tanúsítvány (kiadó: '{_CertTranslator.GetIssuerName(Certificate)}') ellenőrizve, érvényesség: {_CodeTranslator.GetCertError(Validity)} {_CodeTranslator.GetCertErrorReasonList(Reason)}")
        _sbSignLog.AppendLine()
    End Sub

#End Region

End Class
