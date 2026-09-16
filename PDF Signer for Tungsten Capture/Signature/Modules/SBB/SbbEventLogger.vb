Imports System.Globalization
Imports System.Text
Imports NLog
Imports nsoftware.SecureBlackbox
Imports SbbPdfSigner = nsoftware.SecureBlackbox.PDFSigner

''' <summary>Writes PDFSigner validation events into the Hungarian signature log; chain events during a TLS handshake describe the TLS server certificate and go to trace only.</summary>
Friend NotInheritable Class SbbEventLogger
    Private ReadOnly _log As StringBuilder
    Private ReadOnly _logger As Logger
    ''' <summary>Certificate currently processed, so OnError (which names none) can be attributed.</summary>
    Private ReadOnly _context As New ChainContextTracker
    Private _contextBeforeTls As ChainContext
    ''' <summary>TLS handshakes in progress; while positive, chain events belong to the TLS server certificate.</summary>
    Private _tlsHandshakeDepth As Integer

    ''' <summary>Display ids (S0 / T0 / S0T0) for known entity labels; unmapped labels pass through.</summary>
    Friend Property Naming As EntityNaming = EntityNaming.Empty

    ''' <summary>Creates a logger that appends to Log and mirrors every line to Logger.</summary>
    Friend Sub New(Log As StringBuilder, Logger As Logger)
        _log = Log
        _logger = Logger
    End Sub

    ''' <summary>Resets the tracked chain/TLS state and subscribes to S's validation events.</summary>
    Friend Sub Attach(S As SbbPdfSigner)
        _tlsHandshakeDepth = 0
        _context.Reset()
        _contextBeforeTls = Nothing
        AddHandler S.OnError, AddressOf Signer_OnError
        AddHandler S.OnChainElementDownload, AddressOf Signer_OnChainElementDownload
        AddHandler S.OnChainValidated, AddressOf Signer_OnChainValidated
        AddHandler S.OnChainValidationProgress, AddressOf Signer_OnChainValidationProgress
        AddHandler S.OnTimestampRequest, AddressOf Signer_OnTimestampRequest
        AddHandler S.OnTimestampValidated, AddressOf Signer_OnTimestampValidated
        AddHandler S.OnSignatureValidated, AddressOf Signer_OnSignatureValidated
        AddHandler S.OnSignatureFound, AddressOf Signer_OnSignatureFound
        AddHandler S.OnTimestampFound, AddressOf Signer_OnTimestampFound
        AddHandler S.OnChainElementNeeded, AddressOf Signer_OnChainElementNeeded
        AddHandler S.OnEncrypted, AddressOf Signer_OnEncrypted
        AddHandler S.OnTLSHandshake, AddressOf Signer_OnTLSHandshake
        AddHandler S.OnTLSCertValidate, AddressOf Signer_OnTLSCertValidate
        AddHandler S.OnTLSEstablished, AddressOf Signer_OnTLSEstablished
        AddHandler S.OnTLSShutdown, AddressOf Signer_OnTLSShutdown
    End Sub

    ''' <summary>Unsubscribes from S's validation events.</summary>
    Friend Sub Detach(S As SbbPdfSigner)
        RemoveHandler S.OnError, AddressOf Signer_OnError
        RemoveHandler S.OnChainElementDownload, AddressOf Signer_OnChainElementDownload
        RemoveHandler S.OnChainValidated, AddressOf Signer_OnChainValidated
        RemoveHandler S.OnChainValidationProgress, AddressOf Signer_OnChainValidationProgress
        RemoveHandler S.OnTimestampRequest, AddressOf Signer_OnTimestampRequest
        RemoveHandler S.OnTimestampValidated, AddressOf Signer_OnTimestampValidated
        RemoveHandler S.OnSignatureValidated, AddressOf Signer_OnSignatureValidated
        RemoveHandler S.OnSignatureFound, AddressOf Signer_OnSignatureFound
        RemoveHandler S.OnTimestampFound, AddressOf Signer_OnTimestampFound
        RemoveHandler S.OnChainElementNeeded, AddressOf Signer_OnChainElementNeeded
        RemoveHandler S.OnEncrypted, AddressOf Signer_OnEncrypted
        RemoveHandler S.OnTLSHandshake, AddressOf Signer_OnTLSHandshake
        RemoveHandler S.OnTLSCertValidate, AddressOf Signer_OnTLSCertValidate
        RemoveHandler S.OnTLSEstablished, AddressOf Signer_OnTLSEstablished
        RemoveHandler S.OnTLSShutdown, AddressOf Signer_OnTLSShutdown
    End Sub

    Private Sub Signer_OnError(sender As Object, e As PDFSignerErrorEventArgs)
        Dim msg As String = SbbErrorTranslator.Describe(e.ErrorCode, e.Description, _context.Current)
        Line(msg)
        _logger.Error("{0}", msg)
    End Sub

    Private Sub Signer_OnChainElementDownload(sender As Object, e As PDFSignerChainElementDownloadEventArgs)
        _context.Update(e.CertRDN, e.CACertRDN)
        Dim cn As String = ChainContextTracker.CommonName(e.CertRDN)
        Dim label As String = If(String.IsNullOrEmpty(cn), String.Empty, $" ('{cn}' tanúsítványhoz)")
        ChainLine($"  {SbbTranslator.ChainKind(e.Kind)} letöltése{label}, URL: {SbbErrorTranslator.StripUserInfo(e.Location)}")
    End Sub

    Private Sub Signer_OnChainValidated(sender As Object, e As PDFSignerChainValidatedEventArgs)
        Dim who As String = If(String.IsNullOrEmpty(e.SubjectRDN), Naming.Display(e.EntityLabel), ChainContextTracker.CommonName(e.SubjectRDN))
        ChainLine($"'{who}' tanúsítványlánc ellenőrizve, érvényesség: {SbbTranslator.ChainValidity(CType(e.ValidationResult, ChainValidities))}")
    End Sub

    Private Sub Signer_OnChainValidationProgress(sender As Object, e As PDFSignerChainValidationProgressEventArgs)
        _context.Update(e.CertRDN, e.CACertRDN)
        Dim caCn As String = ChainContextTracker.CommonName(e.CACertRDN)
        Dim issuer As String = If(String.IsNullOrEmpty(caCn), String.Empty, $" (kiadó: '{caCn}')")
        ChainLine($"  {SbbTranslator.EventKind(e.EventKind)}, tanúsítvány: '{ChainContextTracker.CommonName(e.CertRDN)}'{issuer}")
    End Sub

    ''' <summary>The TSA value mirrors TimestampServer, which may carry credentials; they are stripped before logging.</summary>
    Private Sub Signer_OnTimestampRequest(sender As Object, e As PDFSignerTimestampRequestEventArgs)
        Line($"Időbélyeg kérése, TSA URL: {SbbErrorTranslator.StripUserInfo(e.TSA)}")
    End Sub

    Private Sub Signer_OnTimestampValidated(sender As Object, e As PDFSignerTimestampValidatedEventArgs)
        Line(TimestampValidatedLine(Naming.Display(e.EntityLabel), e.IssuerRDN, e.Time, e.ValidationResult, e.ChainValidationResult))
    End Sub

    ''' <summary>The timestamp-validated line; SecureBlackbox's UTC genTime is re-rendered in the local (UTC) form, or echoed in quotes when it does not parse.</summary>
    Friend Shared Function TimestampValidatedLine(EntityId As String, IssuerRDN As String, Time As String, ValidationResult As Integer, ChainValidationResult As Integer) As String
        Dim validity As String = SbbTranslator.SignatureValidity(CType(ValidationResult, SignatureValidities))
        Dim chainValidity As String = SbbTranslator.ChainValidity(CType(ChainValidationResult, ChainValidities))
        Dim parsed As Date
        Dim moment As String
        If Date.TryParseExact(Time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal Or DateTimeStyles.AdjustToUniversal, parsed) Then
            moment = MomentFormat.Log(MomentFormat.FromUtc(parsed))
        Else
            moment = $"'{Time}'"
        End If
        Return $"{EntityId} (kiadó: '{ChainContextTracker.CommonName(IssuerRDN)}') ellenőrizve {moment} időpontban, érvényesség: {validity}, lánc érvényesség: {chainValidity}"
    End Function

    Private Sub Signer_OnSignatureValidated(sender As Object, e As PDFSignerSignatureValidatedEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}') ellenőrizve, érvényesség: {SbbTranslator.SignatureValidity(CType(e.ValidationResult, SignatureValidities))}")
    End Sub

    Private Sub Signer_OnSignatureFound(sender As Object, e As PDFSignerSignatureFoundEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} megtalálva (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}'), tanúsítvány: {If(e.CertFound, "megtalálva", "nem található")}")
    End Sub

    Private Sub Signer_OnTimestampFound(sender As Object, e As PDFSignerTimestampFoundEventArgs)
        Line($"{Naming.Display(e.EntityLabel)} megtalálva (kiadó: '{ChainContextTracker.CommonName(e.IssuerRDN)}'), tanúsítvány: {If(e.CertFound, "megtalálva", "nem található")}")
    End Sub

    Private Sub Signer_OnChainElementNeeded(sender As Object, e As PDFSignerChainElementNeededEventArgs)
        Dim caCn As String = ChainContextTracker.CommonName(e.CACertRDN)
        Dim issuer As String = If(String.IsNullOrEmpty(caCn), String.Empty, $" (kiadó: '{caCn}')")
        ChainLine($"  {SbbTranslator.ChainKind(e.Kind)} szükséges, tanúsítvány: '{ChainContextTracker.CommonName(e.CertRDN)}'{issuer}")
    End Sub

    Private Sub Signer_OnEncrypted(sender As Object, e As PDFSignerEncryptedEventArgs)
        Line($"A PDF dokumentum titkosított (tanúsítvány alapú: {e.CertUsed})")
    End Sub

    Private Sub Signer_OnTLSHandshake(sender As Object, e As PDFSignerTLSHandshakeEventArgs)
        If _tlsHandshakeDepth = 0 Then _contextBeforeTls = _context.Current
        _tlsHandshakeDepth += 1
        TraceLine($"  TLS kézfogás indul: {e.Host}")
    End Sub

    Private Sub Signer_OnTLSCertValidate(sender As Object, e As PDFSignerTLSCertValidateEventArgs)
        TraceLine($"  TLS szerver tanúsítvány ellenőrzése: {e.ServerHost} ({e.ServerIP}), elfogadva: {e.Accept}")
    End Sub

    Private Sub Signer_OnTLSEstablished(sender As Object, e As PDFSignerTLSEstablishedEventArgs)
        If _tlsHandshakeDepth > 0 Then _tlsHandshakeDepth -= 1
        If _tlsHandshakeDepth = 0 AndAlso _contextBeforeTls IsNot Nothing Then
            _context.Restore(_contextBeforeTls)
            _contextBeforeTls = Nothing
        End If
        TraceLine($"  TLS kapcsolat létrejött: {e.Host}, verzió: {e.Version}, titkosítás: {e.Ciphersuite}")
    End Sub

    Private Sub Signer_OnTLSShutdown(sender As Object, e As PDFSignerTLSShutdownEventArgs)
        TraceLine($"  TLS kapcsolat lezárva: {e.Host}")
    End Sub

    Private Sub TraceLine(M As String)
        If _logger.IsTraceEnabled Then Line(M)
    End Sub

    ''' <summary>Chain lines during a TLS handshake describe the TLS server certificate and are demoted to trace.</summary>
    Private Sub ChainLine(M As String)
        If _tlsHandshakeDepth > 0 Then
            TraceLine(M)
        Else
            Line(M)
        End If
    End Sub

    Private Sub Line(M As String)
        _log.AppendLine(M)
    End Sub
End Class
