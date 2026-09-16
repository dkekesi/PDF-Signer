Imports System.Globalization

''' <summary>Renders moments for the signature log: local time first, the UTC instant in parentheses.</summary>
Friend Module MomentFormat
    ''' <summary>Dotted date and time to the second.</summary>
    Friend Const DateTimePattern As String = "yyyy.MM.dd. HH:mm:ss"

    ''' <summary>Wraps a UTC wall-clock value regardless of its Kind.</summary>
    Friend Function FromUtc(Utc As Date) As DateTimeOffset
        Return New DateTimeOffset(Date.SpecifyKind(Utc, DateTimeKind.Utc))
    End Function

    ''' <summary>"2026.09.08. 19:18:20 (2026.09.08. 17:18:20 UTC)" in the process time zone.</summary>
    Friend Function Log(Moment As DateTimeOffset) As String
        Dim local As String = TimeZoneInfo.ConvertTime(Moment, TimeZoneInfo.Local).ToString(DateTimePattern, CultureInfo.InvariantCulture)
        Dim utc As String = Moment.UtcDateTime.ToString(DateTimePattern, CultureInfo.InvariantCulture)
        Return $"{local} ({utc} UTC)"
    End Function

    ''' <summary>One half only: the UTC instant suffixed with " UTC", or the local time without a marker.</summary>
    Friend Function Display(Moment As DateTimeOffset, Utc As Boolean) As String
        If Utc Then Return Moment.UtcDateTime.ToString(DateTimePattern, CultureInfo.InvariantCulture) & " UTC"
        Return TimeZoneInfo.ConvertTime(Moment, TimeZoneInfo.Local).ToString(DateTimePattern, CultureInfo.InvariantCulture)
    End Function
End Module
