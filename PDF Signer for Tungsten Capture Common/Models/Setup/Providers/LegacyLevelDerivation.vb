''' <summary>Provider values derived from a backup file written before the PAdESLevel element existed (timestamp switches + one revocation setting).</summary>
Public Class LegacyLevelDerivation
    ''' <summary>Derived PAdES baseline level.</summary>
    Public Property Level As PAdESLevelType
    ''' <summary>Whether revocation checking should be enabled for the derived level.</summary>
    Public Property EnableRevocationChecking As Boolean
    ''' <summary>Derived revocation check protocol (a <see cref="RevocationType"/> value); OCSP when checking is disabled.</summary>
    Public Property RevocationCheckProtocol As Integer
    ''' <summary>Whether revocation data should be embedded; always true for a legacy backup.</summary>
    Public Property EmbedRevocationInformation As Boolean

    ''' <summary>Maps the timestamp switches and the single revocation setting onto a level and the three revocation switches.</summary>
    Public Shared Function FromLegacy(TimeStampingEnabled As Boolean, DocumentTimeStamp As Boolean, RevocationCheck As Integer) As LegacyLevelDerivation
        Dim checking As Boolean = RevocationCheck <> RevocationType.None
        Dim level As PAdESLevelType
        If Not TimeStampingEnabled Then
            level = PAdESLevelType.BaselineB
        ElseIf Not DocumentTimeStamp Then
            ' A timestamped signature whose revocation data was collected is a B-LT.
            level = If(checking, PAdESLevelType.BaselineLT, PAdESLevelType.BaselineT)
        Else
            level = PAdESLevelType.BaselineLTA
        End If

        Return New LegacyLevelDerivation With {
            .Level = level,
            .EnableRevocationChecking = checking,
            .RevocationCheckProtocol = If(checking, RevocationCheck, CInt(RevocationType.OCSP)),
            .EmbedRevocationInformation = True}
    End Function
End Class
