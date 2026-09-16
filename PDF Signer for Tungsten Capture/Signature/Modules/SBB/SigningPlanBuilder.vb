Imports nsoftware.SecureBlackbox
Imports PDFSignerCommon

''' <summary>Pure mapping from the provider settings to a <see cref="SigningPlan"/>; no I/O.</summary>
Friend Module SigningPlanBuilder
    ''' <summary>Builds the signing plan for one document from the provider's PAdES level and revocation settings.</summary>
    Friend Function Build(Settings As PDFSignerCryptoProvider) As SigningPlan
        Dim protocol As PDFSignerRevocationChecks = SbbTranslator.RevocationCheck(CType(Settings.RevocationCheckProtocol, RevocationType))
        Dim revocation As PDFSignerRevocationChecks = If(Settings.EnableRevocationChecking, protocol, PDFSignerRevocationChecks.crcNone)
        Dim level As Integer = Settings.PAdESLevel

        Select Case level
            Case PAdESLevelType.BaselineB
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineB,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = revocation,
                    .CheckSigningCertRevocation = Settings.EnableRevocationChecking}
            Case PAdESLevelType.BaselineT
                ' A non-LTV Sign() does not check revocation itself, so the two chains get explicit validator checks.
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineT,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = revocation,
                    .EmbedSignatureTimestamp = True,
                    .CheckSigningCertRevocation = Settings.EnableRevocationChecking,
                    .CheckTimestampCertRevocation = Settings.EnableRevocationChecking}
            Case PAdESLevelType.BaselineLT, PAdESLevelType.BaselineLTA
                ' Sign lean at B-T, then Update() checks and embeds once; B-LTA appends the archive timestamp afterwards.
                Return New SigningPlan With {
                    .SignatureLevel = PAdESSignatureLevels.paslBaselineT,
                    .Revocation = revocation,
                    .SignPassRevocationCheck = PDFSignerRevocationChecks.crcNone,
                    .EmbedSignatureTimestamp = True,
                    .UpdateToEmbed = True,
                    .AddDocumentTimestamp = (level = PAdESLevelType.BaselineLTA),
                    .EmbedDocumentTimestampRevocation = (level = PAdESLevelType.BaselineLTA) AndAlso Settings.EnableRevocationChecking}
            Case Else
                Throw New NotSupportedException($"Nem támogatott PAdES szint: {level}")
        End Select
    End Function
End Module
