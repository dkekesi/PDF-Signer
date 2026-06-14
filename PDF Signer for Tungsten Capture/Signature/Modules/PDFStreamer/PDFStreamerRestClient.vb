Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks

''' <summary>Outcome of a PDF Streamer REST streaming call.</summary>
Friend Class PDFStreamerRestResult
    Friend Property Success As Boolean
    Friend Property ErrorMessage As String
    Friend Property ExceptionText As String
    Friend Property DocumentLog As String
    Friend Property ValidUntil As Date?
    Friend Property SignedStream As Stream
End Class

''' <summary>Subset of the RFC 7807 ProblemDetails body returned by PDF Streamer on error.</summary>
<DataContract>
Friend Class PDFStreamerProblemDetails
    <DataMember(Name:="title")> Public Property Title As String
    <DataMember(Name:="detail")> Public Property Detail As String
    <DataMember(Name:="errorMessage")> Public Property ErrorMessage As String
    <DataMember(Name:="documentLog")> Public Property DocumentLog As String
    <DataMember(Name:="transactionId")> Public Property TransactionId As String
End Class

''' <summary>
''' Calls the PDF Streamer REST streaming endpoint (POST .../api/v1/documents/stream):
''' streams the unsigned document up and writes the signed bytes back into the same stream.
''' </summary>
Friend Class PDFStreamerRestClient

    ' One shared client; signing large documents can be slow, so the client itself has no
    ' timeout and each call carries its own deadline via a CancellationToken.
    Private Shared ReadOnly _http As HttpClient = BuildClient()

    ' Generous per-call deadline (large documents + signing/TSA round-trips).
    Private Shared ReadOnly _callTimeout As TimeSpan = TimeSpan.FromMinutes(30)

    Private Const StreamCopyBufferSize As Integer = 65000

    Private Shared Function BuildClient() As HttpClient
        ' net48 may not negotiate modern TLS by default
        ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol Or
                                               SecurityProtocolType.Tls12 Or SecurityProtocolType.Tls13
        Dim c As New HttpClient()
        c.Timeout = Timeout.InfiniteTimeSpan
        Return c
    End Function

    ''' <summary>
    ''' Posts <paramref name="documentStream"/> to the endpoint and overwrites it in place with the
    ''' signed response. Synchronous wrapper around the async core; all transport exceptions are
    ''' captured into the result.
    ''' </summary>
    Friend Function Sign(endpointUrl As Uri,
                         authorizationCode As String,
                         documentConfigFile As String,
                         transactionId As String,
                         documentFileName As String,
                         documentStream As Stream) As PDFStreamerRestResult
        Try
            ' Offload to the thread pool and block, so a UI-thread caller cannot deadlock on the
            ' awaited continuations.
            Return Task.Run(Function() SignAsync(endpointUrl, authorizationCode, documentConfigFile,
                                                 transactionId, documentFileName, documentStream)).
                                                 GetAwaiter().GetResult()
        Catch ex As Exception
            Return New PDFStreamerRestResult With {
                .ErrorMessage = ex.Message,
                .ExceptionText = ex.ToString()
            }
        End Try
    End Function

    Private Async Function SignAsync(endpointUrl As Uri,
                                     authorizationCode As String,
                                     documentConfigFile As String,
                                     transactionId As String,
                                     documentFileName As String,
                                     documentStream As Stream) As Task(Of PDFStreamerRestResult)
        Dim result As New PDFStreamerRestResult

        Using cts As New CancellationTokenSource(_callTimeout)
            documentStream.Seek(0, SeekOrigin.Begin)

            Using content As New StreamContent(New NonDisposingStream(documentStream))
                content.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")

                Using request As New HttpRequestMessage(HttpMethod.Post, endpointUrl)
                    request.Content = content
                    request.Headers.TryAddWithoutValidation("X-Api-Key", authorizationCode)
                    request.Headers.TryAddWithoutValidation("X-Document-Config", documentConfigFile)
                    request.Headers.TryAddWithoutValidation("X-Transaction-Id", transactionId)
                    request.Headers.TryAddWithoutValidation("X-Document-Filename", documentFileName)

                    Using response As HttpResponseMessage = Await _http.SendAsync(
                            request, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(False)

                        If Not response.IsSuccessStatusCode Then
                            Await ReadProblemAsync(response, result).ConfigureAwait(False)
                            Return result
                        End If

                        ' Overwrite the unsigned bytes in place with the signed response.
                        documentStream.Seek(0, SeekOrigin.Begin)
                        Using responseStream As Stream = Await response.Content.ReadAsStreamAsync().ConfigureAwait(False)
                            Dim buffer(StreamCopyBufferSize - 1) As Byte
                            Dim count As Integer
                            Do
                                count = Await responseStream.ReadAsync(buffer, 0, buffer.Length, cts.Token).ConfigureAwait(False)
                                If count > 0 Then documentStream.Write(buffer, 0, count)
                            Loop While count > 0
                        End Using
                        ' A signed file shorter than the original must not leave stale trailing bytes.
                        documentStream.SetLength(documentStream.Position)

                        result.Success = True
                        result.SignedStream = documentStream
                        result.DocumentLog = DecodeLog(response)
                        result.ValidUntil = ParseValidUntil(response)
                        Return result
                    End Using
                End Using
            End Using
        End Using
    End Function

    Private Async Function ReadProblemAsync(response As HttpResponseMessage, result As PDFStreamerRestResult) As Task
        Dim body As String = Await response.Content.ReadAsStringAsync().ConfigureAwait(False)

        Dim problem As PDFStreamerProblemDetails = Nothing
        Try
            If Not String.IsNullOrWhiteSpace(body) Then
                Dim ser As New DataContractJsonSerializer(GetType(PDFStreamerProblemDetails))
                Using ms As New MemoryStream(Encoding.UTF8.GetBytes(body))
                    problem = CType(ser.ReadObject(ms), PDFStreamerProblemDetails)
                End Using
            End If
        Catch
            ' Non-JSON error body; fall through to the status line below.
        End Try

        If problem IsNot Nothing Then
            result.ErrorMessage = If(Not String.IsNullOrEmpty(problem.ErrorMessage), problem.ErrorMessage,
                                  If(Not String.IsNullOrEmpty(problem.Detail), problem.Detail, problem.Title))
            result.DocumentLog = problem.DocumentLog
        End If

        If String.IsNullOrEmpty(result.ErrorMessage) Then
            result.ErrorMessage = $"HTTP {CInt(response.StatusCode)} {response.ReasonPhrase}"
        End If
        result.ExceptionText = body
    End Function

    Private Function DecodeLog(response As HttpResponseMessage) As String
        Dim encoded As String = GetHeader(response, "X-Document-Log")
        If String.IsNullOrEmpty(encoded) Then Return Nothing
        Try
            Return Encoding.UTF8.GetString(Convert.FromBase64String(encoded))
        Catch
            Return encoded
        End Try
    End Function

    Private Function ParseValidUntil(response As HttpResponseMessage) As Date?
        Dim raw As String = GetHeader(response, "X-Valid-Until")
        If String.IsNullOrEmpty(raw) Then Return Nothing
        Dim parsed As Date
        If Date.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, parsed) Then
            Return parsed
        End If
        Return Nothing
    End Function

    Private Function GetHeader(response As HttpResponseMessage, name As String) As String
        Dim values As IEnumerable(Of String) = Nothing
        If response.Headers.TryGetValues(name, values) Then
            Return values.FirstOrDefault()
        End If
        If response.Content IsNot Nothing AndAlso response.Content.Headers.TryGetValues(name, values) Then
            Return values.FirstOrDefault()
        End If
        Return Nothing
    End Function

End Class

''' <summary>
''' Wraps a stream so that disposing the wrapper does NOT dispose the inner stream.
''' HttpClient's StreamContent disposes its content stream on .NET Framework (no leaveOpen
''' overload), but the caller reuses FileToSign after signing, so the source must stay open.
''' </summary>
Friend NotInheritable Class NonDisposingStream
    Inherits Stream

    Private ReadOnly _inner As Stream

    Friend Sub New(inner As Stream)
        _inner = inner
    End Sub

    Public Overrides ReadOnly Property CanRead As Boolean
        Get
            Return _inner.CanRead
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek As Boolean
        Get
            Return _inner.CanSeek
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite As Boolean
        Get
            Return _inner.CanWrite
        End Get
    End Property

    Public Overrides ReadOnly Property Length As Long
        Get
            Return _inner.Length
        End Get
    End Property

    Public Overrides Property Position As Long
        Get
            Return _inner.Position
        End Get
        Set(value As Long)
            _inner.Position = value
        End Set
    End Property

    Public Overrides Sub Flush()
        _inner.Flush()
    End Sub

    Public Overrides Function Read(buffer As Byte(), offset As Integer, count As Integer) As Integer
        Return _inner.Read(buffer, offset, count)
    End Function

    Public Overrides Function ReadAsync(buffer As Byte(), offset As Integer, count As Integer, cancellationToken As CancellationToken) As Task(Of Integer)
        Return _inner.ReadAsync(buffer, offset, count, cancellationToken)
    End Function

    Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
        Return _inner.Seek(offset, origin)
    End Function

    Public Overrides Sub SetLength(value As Long)
        _inner.SetLength(value)
    End Sub

    Public Overrides Sub Write(buffer As Byte(), offset As Integer, count As Integer)
        _inner.Write(buffer, offset, count)
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        ' Intentionally does NOT dispose _inner — the caller owns and reuses it.
        MyBase.Dispose(disposing)
    End Sub
End Class
