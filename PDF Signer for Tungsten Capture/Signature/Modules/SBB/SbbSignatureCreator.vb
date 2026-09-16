Imports System.IO
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports NLog
Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Built-in signing module: runs the planned SecureBlackbox passes over an unsigned PDF and returns the signed stream, its validity end and the log.</summary>
Friend Class SbbSignatureCreator
    Private Shared ReadOnly _logger As Logger = LogManager.GetCurrentClassLogger()

    Private ReadOnly _log As New StringBuilder
    Private _settings As PDFSignerCryptoProvider
    Private _stores As WindowsCertificateStores
    Private _harvest As HarvestedMaterial = HarvestedMaterial.Empty()
    Private _eventLogger As SbbEventLogger
    Private _largeDocument As Boolean

    ''' <summary>Stores the provider settings every later call reads.</summary>
    Friend Sub Initialize(ProviderSettings As PDFSignerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    ''' <summary>Applies the license of the TEl* API used by the PDF pre/post processors; the components carry their own RuntimeLicense.</summary>
    Friend Shared Sub ActivateLicense()
        SBUtils.Unit.SetLicenseKey(SbbLicense.Key)
        SBPDF.Unit.Initialize()
        SBPDFSecurity.Unit.Initialize()
    End Sub

    ''' <summary>Currently valid signing-capable certificates of the user's personal store, optionally qualified ones only.</summary>
    Friend Function GetCertificatesFromStore(QualifiedCertificatesOnly As Boolean) As List(Of SigningCertificate)
        Dim res As New List(Of SigningCertificate)
        Using store As New X509Store(StoreName.My, StoreLocation.CurrentUser)
            store.Open(OpenFlags.ReadOnly Or OpenFlags.OpenExistingOnly)
            For Each cert As X509Certificate2 In store.Certificates
                If cert.NotBefore >= Date.Now OrElse cert.NotAfter <= Date.Now Then Continue For
                If Not SigningCertificatePrecheck.HasSigningKeyUsage(cert) Then Continue For
                Dim qualified As Boolean = QualifiedCertificateDetector.IsQualified(cert)
                If QualifiedCertificatesOnly AndAlso Not qualified Then Continue For
                res.Add(New SigningCertificate With {.Certificate = cert, .IsQualified = qualified})
            Next
        End Using
        Return res
    End Function

    ''' <summary>Signs Request.FileToSign into a new stream; never throws, a failure sets ErrorMessage and the log.</summary>
    ''' <remarks>The caller owns SignedFile (a self-deleting temp file for large documents); FileToSign is only read.</remarks>
    Friend Function SignDocument(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult
        Dim working As Stream = Nothing
        _eventLogger = New SbbEventLogger(_log, _logger)
        Try
            Dim plan As SigningPlan = SigningPlanBuilder.Build(_settings)

            Append("Aláíró tanúsítvány off-line ellenőrzése")
            Dim cert As X509Certificate2 = Request.SigningCertificate?.Certificate
            If cert Is Nothing Then Return Fail(res, "Nincs kiválasztva aláíró tanúsítvány!")
            Dim requireOcsp As Boolean = _settings.EnableRevocationChecking AndAlso _settings.RevocationCheckProtocol = RevocationType.OCSP
            Dim precheckError As String = SigningCertificatePrecheck.Check(cert, requireOcsp)
            If precheckError IsNot Nothing Then Return Fail(res, precheckError)

            Dim policy As MetaData = Request.MetaDataSettings
            Dim policyHash As String = Nothing
            Dim policyError As String = CheckSignaturePolicy(policy, policyHash)
            If policyError IsNot Nothing Then Return Fail(res, policyError)
            If policyHash Is Nothing Then policy = Nothing

            _stores = WindowsCertificateStores.Load()
            _largeDocument = SigningBufferFactory.IsLargeDocument(Request.FileToSign)
            working = SigningBufferFactory.Create(_largeDocument)
            Request.FileToSign.Seek(0, SeekOrigin.Begin)
            Request.FileToSign.CopyTo(working)

            Dim encrypted As Boolean
            Dim entityCount As Integer
            DocumentCertificateHarvester.Probe(working, encrypted, entityCount)
            If encrypted Then Return Fail(res, "A PDF dokumentum titkosított, titkosított dokumentum nem írható alá!")
            If entityCount > 0 Then Return Fail(res, "A PDF dokumentum már tartalmaz aláírást vagy időbélyeget; a beépített szolgáltató csak aláíratlan dokumentumot ír alá!")

            If plan.CheckSigningCertRevocation Then
                Dim leaf As New Certificate(cert.RawData, 0, cert.RawData.Length)
                If Not RunValidatorCheck(leaf, "aláíró tanúsítvány", res) Then Return res
            End If

            working = Advance(working, RunSignPass(plan, cert, policy, policyHash, working, res))
            If working Is Nothing Then Return res
            _harvest = DocumentCertificateHarvester.Harvest(working)
            _eventLogger.Naming = _harvest.Naming

            If plan.CheckTimestampCertRevocation Then
                Dim tsaBytes As Byte() = _harvest.SignatureTimestampCertificateBytes
                If tsaBytes Is Nothing Then Return Fail(res, "Az aláírás-időbélyeg tanúsítványa nem található a dokumentumban!")
                If Not RunValidatorCheck(New Certificate(tsaBytes, 0, tsaBytes.Length), "időbélyeg-szolgáltató tanúsítvány", res) Then Return res
            End If

            If plan.UpdateToEmbed Then
                working = Advance(working, RunUpdatePass(plan, working, res))
                If working Is Nothing Then Return res
                _harvest = DocumentCertificateHarvester.Harvest(working)
            End If

            If plan.AddDocumentTimestamp Then
                working = Advance(working, RunDocumentTimestampPass(working, res))
                If working Is Nothing Then Return res
                _harvest = DocumentCertificateHarvester.Harvest(working)
                _eventLogger.Naming = _harvest.Naming
                If plan.EmbedDocumentTimestampRevocation Then
                    working = Advance(working, RunUpdatePass(plan, working, res))
                    If working Is Nothing Then Return res
                    _harvest = DocumentCertificateHarvester.Harvest(working)
                End If
            End If

            Dim validity As Date
            If Not TryComputeValidity(working, res, validity) Then Return res

            working.Seek(0, SeekOrigin.Begin)
            res.SignedFile = working
            working = Nothing
            res.SignatureExpiration = validity
            res.SignatureLog = _log.ToString()
            Return res
        Catch ex As Exception
            ' The exception text can quote a URL, so credentials are scrubbed before it reaches any log.
            Dim details As String = SbbErrorTranslator.ScrubUrlCredentials(ex.ToString())
            _logger.Error("Váratlan hiba az aláírás közben: {0}", details)
            res.ExceptionText = details
            Return Fail(res, $"Váratlan hiba az aláírás közben: {SbbErrorTranslator.ExceptionText(ex)}")
        Finally
            ' Nothing once handed to the result or released by Advance; otherwise the buffer is freed here.
            working?.Dispose()
        End Try
    End Function

#Region "Passes"

    ''' <summary>Sign pass at the plan's level; B-LT/B-LTA sign lean (crcNone) and are lifted by the Update pass.</summary>
    ''' <remarks>Policy (with its hex hash PolicyHash) is the document class's signature policy, or Nothing for none.</remarks>
    Private Function RunSignPass(Plan As SigningPlan, SigningCert As X509Certificate2, Policy As MetaData, PolicyHash As String, Source As Stream, Res As SignatureResult) As Stream
        _log.AppendLine()
        Append("PDF dokumentum aláírása")
        If Policy IsNot Nothing Then Append($"Aláírás-szabályzat: OID '{Policy.SignaturePolicyOID}', URL: {SbbErrorTranslator.StripUserInfo(Policy.SignaturePolicyURL.ToString())}")
        If Plan.EmbedSignatureTimestamp Then LogTsa()
        LogProxy()

        Using certManager As CertificateManager = SbbLicense.CreateCertificateManager()
            certManager.ImportFromObject(SigningCert)
            Dim failure As String = Nothing
            Dim output As Stream = RunSignerPass(Source,
                Sub(signer As SbbPdfSigner)
                    signer.SigningCertificate = certManager.Certificate
                    signer.NewSignature.Level = Plan.SignatureLevel
                    signer.NewSignature.HashAlgorithm = SbbTranslator.HashAlgorithmName(_settings.SignatureHashMethod)
                    signer.NewSignature.AuthorName = _settings.SigningOrganization
                    signer.NewSignature.Reason = _settings.SigningReason
                    If Policy IsNot Nothing Then ApplySignaturePolicy(signer.NewSignature, Policy, PolicyHash, _settings.SignatureHashMethod)
                    signer.Widget.Invisible = True
                    signer.RevocationCheck = Plan.SignPassRevocationCheck
                    signer.OfflineMode = False
                    signer.IgnoreChainValidationErrors = False
                    If Plan.EmbedSignatureTimestamp Then signer.TimestampServer = TsaUrl()
                    ConfigureProxy(signer.Proxy)
                End Sub,
                Sub(signer As SbbPdfSigner) signer.Sign(),
                failure)
            If output Is Nothing Then
                Fail(Res, $"Hiba a hitelesítési művelet közben: {failure}")
                Return Nothing
            End If
            Append("Hitelesítési művelet befejezve")
            Return output
        End Using
    End Function

    ''' <summary>Update pass over the last entity: checks the chains with the configured protocol and embeds the collected revocation data; no TSA.</summary>
    Private Function RunUpdatePass(Plan As SigningPlan, Source As Stream, Res As SignatureResult) As Stream
        _log.AppendLine()
        Dim entityLabel As String = _harvest.LastSignatureEntityLabel
        If String.IsNullOrEmpty(entityLabel) Then
            Fail(Res, "Nem található aláírás a visszavonási adatok beágyazásához!")
            Return Nothing
        End If
        Append($"Visszavonási adatok beágyazása (Update, {_harvest.Naming.Display(entityLabel)})")
        LogProxy()

        Dim failure As String = Nothing
        Dim output As Stream = RunSignerPass(Source,
            Sub(signer As SbbPdfSigner)
                signer.RevocationCheck = Plan.Revocation
                signer.OfflineMode = False
                signer.IgnoreChainValidationErrors = False
                signer.Config("AutoCollectRevocationInfo=true")
                signer.Config("IncludeRevocationInfoToAdbeAttribute=true")
                signer.Config("CollectRevInfoForTimestamps=true")
                ConfigureProxy(signer.Proxy)
            End Sub,
            Sub(signer As SbbPdfSigner) signer.Update(entityLabel),
            failure)
        If output Is Nothing Then
            Fail(Res, $"Hiba a visszavonási adatok beágyazása közben: {failure}")
            Return Nothing
        End If
        Append("Visszavonási adatok beágyazva")
        Return output
    End Function

    ''' <summary>Lean archive document timestamp: no revocation checking or collection in the creating pass (SecureBlackbox never embeds the new timestamp's own revocation there).</summary>
    Private Function RunDocumentTimestampPass(Source As Stream, Res As SignatureResult) As Stream
        _log.AppendLine()
        Append("Dokumentumszintű időbélyeg készítése")
        LogTsa()

        Dim failure As String = Nothing
        Dim output As Stream = RunSignerPass(Source,
            Sub(signer As SbbPdfSigner)
                signer.NewSignature.SignatureType = PDFSignatureTypes.pstDocumentTimestamp
                signer.NewSignature.HashAlgorithm = SbbTranslator.HashAlgorithmName(_settings.TimeStampHashMethod)
                signer.Widget.Invisible = True
                signer.TimestampServer = TsaUrl()
                signer.RevocationCheck = PDFSignerRevocationChecks.crcNone
                signer.OfflineMode = False
                ConfigureProxy(signer.Proxy)
            End Sub,
            Sub(signer As SbbPdfSigner) signer.Sign(),
            failure)
        If output Is Nothing Then
            Fail(Res, $"Hiba a dokumentumszintű időbélyeg készítése közben: {failure}")
            Return Nothing
        End If
        Append("Dokumentumszintű időbélyeg elkészítve")
        Return output
    End Function

    ''' <summary>One PDFSigner pass: fresh component and output buffer, stores, harvested material, then configure and execute. Never throws; a failure comes back as text.</summary>
    Private Function RunSignerPass(Source As Stream, Configure As Action(Of SbbPdfSigner), Execute As Action(Of SbbPdfSigner), ByRef Failure As String) As Stream
        Dim output As Stream = Nothing
        Try
            Using signer As SbbPdfSigner = SbbLicense.CreateSigner()
                output = SigningBufferFactory.Create(_largeDocument)
                Source.Seek(0, SeekOrigin.Begin)
                signer.InputStream = Source
                signer.OutputStream = output
                signer.TrustedCertificates = WindowsCertificateStores.CopyOf(_stores.TrustedRoots)
                signer.KnownCertificates = _harvest.BuildKnownCertificates(_stores.IntermediateCertificates)
                _harvest.ApplyKnownRevocation(signer)
                Configure(signer)
                _eventLogger.Attach(signer)
                Try
                    Execute(signer)
                Finally
                    _eventLogger.Detach(signer)
                End Try
                output.Seek(0, SeekOrigin.Begin)
                Failure = Nothing
                Return output
            End Using
        Catch ex As Exception
            Failure = SbbErrorTranslator.ExceptionText(ex)
            output?.Dispose()
            Return Nothing
        End Try
    End Function

    ''' <summary>Returns the operator error of an unusable signature policy, else Nothing; PolicyHashHex is Nothing when no policy applies.</summary>
    ''' <remarks>A blank OID must be refused: SecureBlackbox then signs silently without the policy.</remarks>
    Friend Shared Function CheckSignaturePolicy(Policy As MetaData, ByRef PolicyHashHex As String) As String
        PolicyHashHex = Nothing
        If Policy?.SignaturePolicyURL Is Nothing Then Return Nothing
        If String.IsNullOrWhiteSpace(Policy.SignaturePolicyOID) Then Return "Az aláírás-szabályzat azonosítója (OID) nincs megadva, pedig a szabályzat URL be van állítva!"
        PolicyHashHex = SbbTranslator.PolicyHashHex(Policy.SignaturePolicyHash)
        If PolicyHashHex Is Nothing Then Return "Az aláírás-szabályzat lenyomata nem érvényes base64 érték!"
        Return Nothing
    End Function

    ''' <summary>Makes Signature an EPES signature under Policy: OID, hex hash (PolicyHashHex) with the signature's hash algorithm, and URI.</summary>
    Friend Shared Sub ApplySignaturePolicy(Signature As PDFSignature, Policy As MetaData, PolicyHashHex As String, HashMethod As Integer)
        Signature.PolicyID = Policy.SignaturePolicyOID.Trim()
        Signature.PolicyHash = PolicyHashHex
        Signature.PolicyHashAlgorithm = SbbTranslator.HashAlgorithmName(HashMethod)
        Signature.PolicyURI = Policy.SignaturePolicyURL.ToString()
    End Sub

    ''' <summary>Fail-closed online CertificateValidator pass; False (with the error recorded in Res) unless the chain is cvtValid.</summary>
    Private Function RunValidatorCheck(Leaf As Certificate, Label As String, Res As SignatureResult) As Boolean
        _log.AppendLine()
        Append($"Visszavonás-ellenőrzés: {Label}")
        Dim context As New ChainContextTracker
        Using validator As CertificateValidator = SbbLicense.CreateCertificateValidator()
            Try
                validator.Certificate = Leaf
                validator.TrustedCertificates = WindowsCertificateStores.CopyOf(_stores.TrustedRoots)
                validator.KnownCertificates = _harvest.BuildKnownCertificates(_stores.IntermediateCertificates)
                _harvest.ApplyKnownRevocation(validator)
                validator.RevocationCheck = SbbTranslator.RevocationCheckForValidator(CType(_settings.RevocationCheckProtocol, RevocationType))
                validator.OfflineMode = False
                ConfigureProxy(validator.Proxy)

                AddHandler validator.OnBeforeCertificateValidation, Sub(s, e) context.Update(e.Cert, e.CACert)
                AddHandler validator.OnError, Sub(s, e) Append("  " & SbbErrorTranslator.Describe(e.ErrorCode, e.Description, context.Current, Label))
                AddHandler validator.OnBeforeOCSPDownload, Sub(s, e)
                                                              context.Update(e.Cert, e.CACert)
                                                              Append($"  OCSP letöltése, URL: {SbbErrorTranslator.StripUserInfo(e.Location)} ('{ChainContextTracker.CommonName(e.Cert)}' tanúsítványhoz)")
                                                          End Sub
                AddHandler validator.OnBeforeCRLDownload, Sub(s, e)
                                                             context.Update(e.Cert, e.CACert)
                                                             Append($"  CRL letöltése, URL: {SbbErrorTranslator.StripUserInfo(e.Location)} ('{ChainContextTracker.CommonName(e.Cert)}' tanúsítványhoz)")
                                                         End Sub
                AddHandler validator.OnAfterCertificateValidation, Sub(s, e) Append($"  tanúsítványlánc elem ellenőrizve, érvényesség: {e.Validity}, részletek: 0x{e.ValidationDetails:X} ('{ChainContextTracker.CommonName(e.Cert)}')")

                validator.Validate()
                If validator.ChainValidationResult = CertificateValidatorChainValidationResults.cvtValid Then
                    Append($"A(z) {Label} tanúsítványlánca érvényes")
                    Return True
                End If
                Fail(Res, $"A(z) {Label} tanúsítványlánca nem érvényes: {SbbTranslator.ValidatorChainValidity(validator.ChainValidationResult)} (részletek: 0x{validator.ChainValidationDetails:X})")
                Return False
            Catch ex As Exception
                Fail(Res, $"Hiba a(z) {Label} visszavonás-ellenőrzése közben: {SbbErrorTranslator.ExceptionText(ex)}")
                Return False
            End Try
        End Using
    End Function

    ''' <summary>Offline parse of the finished document and the ETSI validity calculation; the archival expiry becomes the result's validity end.</summary>
    Private Function TryComputeValidity(Final As Stream, Res As SignatureResult, ByRef Validity As Date) As Boolean
        _log.AppendLine()
        Append("Hitelesség lejárati idejének meghatározása")
        Using verifier As PDFVerifier = SbbLicense.CreateVerifier()
            Try
                Final.Seek(0, SeekOrigin.Begin)
                verifier.InputStream = Final
                verifier.OfflineMode = True
                verifier.AutoValidateSignatures = False
                verifier.Verify()

                Dim atTime As DateTimeOffset = DateTimeOffset.UtcNow
                Dim traceSink As Action(Of String) = Nothing
                If _logger.IsTraceEnabled Then traceSink = Sub(m As String) _logger.Trace("{0}", m)
                Dim input As DocumentValidationInput = EtsiValidityInputBuilder.From(verifier, atTime, AddressOf Append, traceSink)
                Dim result As DocumentValidityResult = EtsiValidityCalculator.Compute(input, atTime)
                LogValidity(result)
                ' An unbounded expiry leaves the default Date.MinValue, as the other providers do.
                Validity = If(result.Expiry = DateTimeOffset.MaxValue, Date.MinValue, result.Expiry.UtcDateTime)
                Return True
            Catch ex As Exception
                Fail(Res, $"Hiba a PDF dokumentum hitelességi lejárati idejének meghatározása közben: {SbbErrorTranslator.ExceptionText(ex)}")
                Return False
            Finally
                Final.Seek(0, SeekOrigin.Begin)
            End Try
        End Using
    End Function

    ''' <summary>Writes the validity outcome and the evidence expiry to the signature log, and the per-signature traces to NLog.</summary>
    Private Sub LogValidity(Result As DocumentValidityResult)
        Dim whenText As String = EtsiValidityCalculator.OutcomeMoment(Result.Expiry)
        Dim outcome As String
        Select Case Result.Status
            Case ValidityStatus.ValidUntil : outcome = $"Hitelesség lejárati ideje meghatározva: {whenText}"
            Case ValidityStatus.Expired : outcome = $"Hitelesség lejárati ideje múltbeli ({whenText})"
            Case ValidityStatus.BrokenChain : outcome = $"Hitelességi lánc megszakadt; lejárat: {whenText}"
            Case ValidityStatus.MissingRevocationData : outcome = $"Hiányzó visszavonási adat, lejárat: {whenText}"
            Case ValidityStatus.Invalid : outcome = $"Az aláírás létrejöttétől érvénytelen; lejárat: {whenText}"
            Case ValidityStatus.SuspectTimeline : outcome = $"Gyanús időrend; lejárat: {whenText}"
            Case Else : outcome = $"Hitelesség lejárati ideje: {whenText}"
        End Select
        Append(outcome)
        If Result.Status = ValidityStatus.ValidUntil Then
            _logger.Info("{0}", outcome)
        Else
            _logger.Warn("{0}", outcome)
        End If

        Dim evidenceText As String = EtsiValidityCalculator.OutcomeMoment(Result.EvidenceExpiry)
        If Result.EvidenceExpiry < Result.Expiry Then
            Append($"Bizonyítható hitelességi lejárat (hiányzó tanúsítvány/visszavonási adat miatt): {evidenceText}")
        Else
            Append($"Bizonyítható hitelességi lejárat: {evidenceText}")
        End If

        For Each per As PerSignatureResult In Result.PerSignature
            _logger.Debug("Hitelesség [{0}]: {1}, lejárat={2}", If(per.DisplayId, per.SignatureId), per.Status, EtsiValidityCalculator.TraceMoment(per.Expiry))
            For Each line As String In per.Trace
                _logger.Debug("  {0}", line)
            Next
        Next
    End Sub

#End Region

#Region "Helpers"

    ''' <summary>Releases the pass's input buffer and returns its output (Nothing when the pass failed).</summary>
    Private Shared Function Advance(Current As Stream, Output As Stream) As Stream
        Current.Dispose()
        Return Output
    End Function

    ''' <summary>TSA URL with the credentials as user-info; the result must never be written to any log.</summary>
    Private Function TsaUrl() As String
        Return TsaUrlBuilder.WithCredentials(_settings.TSAURL.ToString(), _settings.TSAUserName, _settings.TSAPassword)
    End Function

    Private Sub LogTsa()
        Append($"Időbélyeg szolgáltató URL: {SbbErrorTranslator.StripUserInfo(_settings.TSAURL?.ToString())}, felhasználó: '{_settings.TSAUserName}'")
    End Sub

    Private Sub ConfigureProxy(Proxy As ProxySettings)
        If Not _settings.IsProxyEnabled Then Return
        Proxy.ProxyType = ProxyTypes.cptHTTP
        Proxy.Address = _settings.ProxyServer
        Proxy.Port = _settings.ProxyPort
        Proxy.Authentication = SbbTranslator.ProxyAuth(CType(_settings.ProxyAuthMethod, ProxyAuthenticationMethod))
        Proxy.Username = _settings.ProxyUserName
        Proxy.Password = _settings.ProxyPassword
    End Sub

    Private Sub LogProxy()
        If Not _settings.IsProxyEnabled Then Return
        Append($"HTTP proxy szerver {_settings.ProxyServer}:{_settings.ProxyPort}, felhasználó: '{_settings.ProxyUserName}', autentikáció: {SbbTranslator.ProxyAuthName(CType(_settings.ProxyAuthMethod, ProxyAuthenticationMethod))} beállítva")
    End Sub

    Private Sub Append(Message As String)
        _log.AppendLine(Message)
    End Sub

    ''' <summary>Records the operator-facing error and the log on the result and returns it, so callers can `Return Fail(...)`.</summary>
    Private Function Fail(Res As SignatureResult, Message As String) As SignatureResult
        Res.ErrorMessage = Message
        _log.AppendLine(Message)
        Res.SignatureLog = _log.ToString()
        _logger.Error("{0}", Message)
        Return Res
    End Function

#End Region
End Class
