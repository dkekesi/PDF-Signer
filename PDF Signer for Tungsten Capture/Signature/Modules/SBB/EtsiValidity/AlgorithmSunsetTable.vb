''' <summary>Algorithm-sunset lookup keyed by OID; ships empty (no cap), the shape allows a policy table later without touching the calculator.</summary>
Friend NotInheritable Class AlgorithmSunsetTable
    ''' <summary>Shared instance with no sunset dates configured.</summary>
    Friend Shared ReadOnly Empty As New AlgorithmSunsetTable(New Dictionary(Of String, DateTimeOffset))

    Private ReadOnly _byOid As IDictionary(Of String, DateTimeOffset)

    ''' <summary>Builds the table from a sunset-date-by-OID map.</summary>
    Friend Sub New(ByOid As IDictionary(Of String, DateTimeOffset))
        _byOid = If(ByOid, New Dictionary(Of String, DateTimeOffset))
    End Sub

    ''' <summary>Earliest sunset among the algorithm's signature and hash OIDs; MaxValue means no cap.</summary>
    Friend Function EarliestSunset(Alg As AlgorithmId) As DateTimeOffset
        If Alg Is Nothing Then Return DateTimeOffset.MaxValue
        Dim earliest As DateTimeOffset = DateTimeOffset.MaxValue
        For Each oid As String In {Alg.SignatureAlgOid, Alg.HashAlgOid}
            Dim d As DateTimeOffset
            If Not String.IsNullOrEmpty(oid) AndAlso _byOid.TryGetValue(oid, d) AndAlso d < earliest Then earliest = d
        Next
        Return earliest
    End Function
End Class
