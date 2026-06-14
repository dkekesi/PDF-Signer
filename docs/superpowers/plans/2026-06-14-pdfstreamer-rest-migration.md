# PDF Streamer REST Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace PDF Signer's WCF call to PDF Streamer with the PDF Streamer REST streaming endpoint (`POST /api/v1/documents/stream`), since `PDFStreamer.WCFCommon.dll` no longer exists.

**Architecture:** Keep `PDFStreamer.vb` as the mapping/orchestration layer and replace `PDFStreamerProxy.vb` with a thin `PDFStreamerRestClient` that owns an `HttpClient`, streams the document up, writes the signed bytes back into the source stream, reads metadata from response headers, and parses RFC-7807 errors with the inbox `DataContractJsonSerializer`. Vendor `MemoryTributary` locally (it was provided by the deleted WCFCommon DLL).

**Tech Stack:** VB.NET, .NET Framework 4.8, `System.Net.Http.HttpClient`, `System.Runtime.Serialization.Json`, Costura/Fody single-exe packaging, MSBuild (VS 18). No automated test framework — **verification is build success** (` 0 Error(s)` in the build log), per the repo convention.

**Build/verify command (used as the "test" in every task):**

```
"c:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "PDF Signer for Tungsten Capture.slnx" /restore /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false
```

Expected on success: the log ends with ` 0 Error(s)`. Run all commands from the repo root `C:\Projects\PDF Signer`. Work happens on the existing branch `pdfstreamer-rest-migration`.

---

## File Structure

| File | Responsibility | Action |
| --- | --- | --- |
| `PDF Signer for Tungsten Capture\Helper\MemoryTributary.vb` | Local multi-block memory stream (replaces WCFCommon's) | Create |
| `PDF Signer for Tungsten Capture\FrmPdfSigner.vb` | Remove WCFCommon import (uses local MemoryTributary) | Modify |
| `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb` | Remove WCFCommon import | Modify |
| `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb` | REST transport: HttpClient, headers, streaming, error parsing | Create |
| `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamer.vb` | Map settings/request → REST call → SignatureResult | Rewrite |
| `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamerProxy.vb` | Old WCF proxy | Delete |
| `PDF Signer for Tungsten Capture\Service References\PDFStreamerServiceReference\` | Generated WCF reference | Delete |
| `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj` | References + WCF metadata | Modify |
| `PDF Signer for Tungsten Capture\FodyWeavers.xml` | Costura embed allowlist | Modify |
| `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFStreamerCryptoProvider.vb` | URL scheme validation | Modify |
| `PDF Signer for Tungsten Capture Common\Language\Messages.resx` / `Messages.hu.resx` / `Messages.Designer.vb` | New validation string | Modify |
| `Help\Content\Modules\Administration\SetupDocumentClass.htm` and `Help\Output\Online Help\Modules\Administration\SetupDocumentClass.htm` | Admin help text | Modify |
| `CLAUDE.md` | Repo guidance | Modify |
| `lib\PDF Streamer\PDFStreamer.WCFCommon.dll` | Vendored dead DLL | Delete (if present) |

---

## Task 1: Vendor `MemoryTributary` and drop the WCFCommon import

`MemoryTributary` came from `PDFStreamer.WCFCommon.Helper`. Port it to a local VB class in the runtime project's root namespace (`PDFSigner`), then remove the `Imports PDFStreamer.WCFCommon.Helper` from the two files that use it so the local type is unambiguous. The WCFCommon `<Reference>` still exists at this point, so the build stays green.

**Files:**
- Create: `PDF Signer for Tungsten Capture\Helper\MemoryTributary.vb`
- Modify: `PDF Signer for Tungsten Capture\FrmPdfSigner.vb:17`
- Modify: `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb:3`

- [ ] **Step 1: Create the local `MemoryTributary`**

Create `PDF Signer for Tungsten Capture\Helper\MemoryTributary.vb` with this content (no `Namespace` block — VB folders do not affect namespace, so the class lives in the root `PDFSigner` namespace and is usable project-wide without an import):

```vb
Imports System.IO

''' <summary>
''' A memory-backed stream that stores data in multiple fixed-size blocks instead of one
''' contiguous array, so it scales to large documents without a single large-object-heap
''' allocation. Ported from the former PDFStreamer.WCFCommon.Helper.MemoryTributary, which
''' was removed when PDF Streamer dropped its net48 WCFCommon assembly.
''' </summary>
Public Class MemoryTributary
    Inherits Stream

    Public Sub New()
        Position = 0
    End Sub

    Public Sub New(source As Byte())
        Me.Write(source, 0, source.Length)
        Position = 0
    End Sub

    ' length is ignored because capacity has no meaning unless we impose an artificial limit
    Public Sub New(length As Integer)
        SetLength(length)
        Position = length
        Dim d As Byte() = blck ' access block to prompt allocation
        Position = 0
    End Sub

    Public Overrides ReadOnly Property CanRead As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanSeek As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property CanWrite As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property Length As Long
        Get
            Return lngt
        End Get
    End Property

    Public Overrides Property Position As Long

    Protected lngt As Long = 0
    Protected blockSize As Long = 1048576
    Protected blcks As New List(Of Byte())

    ''' <summary>The block of memory currently addressed by Position.</summary>
    Protected ReadOnly Property blck As Byte()
        Get
            While blcks.Count <= blockId
                blcks.Add(New Byte(CInt(blockSize) - 1) {})
            End While
            Return blcks(CInt(blockId))
        End Get
    End Property

    ''' <summary>The id of the block currently addressed by Position.</summary>
    Protected ReadOnly Property blockId As Long
        Get
            Return Position \ blockSize
        End Get
    End Property

    ''' <summary>The offset of the byte currently addressed by Position, into its block.</summary>
    Protected ReadOnly Property blockOffset As Long
        Get
            Return Position Mod blockSize
        End Get
    End Property

    Public Overrides Sub Flush()
    End Sub

    Public Overrides Function Read(buff As Byte(), offset As Integer, count As Integer) As Integer
        Dim lcount As Long = CLng(count)

        If lcount < 0 Then
            Throw New ArgumentOutOfRangeException("count", lcount, "Number of bytes to copy cannot be negative.")
        End If

        Dim remaining As Long = (lngt - Position)
        If lcount > remaining Then lcount = remaining

        If buff Is Nothing Then
            Throw New ArgumentNullException("buffer", "Buffer cannot be null.")
        End If
        If offset < 0 Then
            Throw New ArgumentOutOfRangeException("offset", offset, "Destination offset cannot be negative.")
        End If

        ' guard against EOF so CopyTo terminates and no empty trailing block is allocated
        If lcount <= 0 Then Return 0

        Dim rd As Integer = 0
        Do
            Dim copysize As Long = Math.Min(lcount, (blockSize - blockOffset))
            Buffer.BlockCopy(blck, CInt(blockOffset), buff, offset, CInt(copysize))
            lcount -= copysize
            offset += CInt(copysize)
            rd += CInt(copysize)
            Position += copysize
        Loop While lcount > 0

        Return rd
    End Function

    Public Overrides Function Seek(offset As Long, origin As SeekOrigin) As Long
        Select Case origin
            Case SeekOrigin.Begin
                Position = offset
            Case SeekOrigin.Current
                Position += offset
            Case SeekOrigin.End
                Position = Length - offset
        End Select
        Return Position
    End Function

    Public Overrides Sub SetLength(value As Long)
        lngt = value
    End Sub

    Public Overrides Sub Write(buff As Byte(), offset As Integer, count As Integer)
        Dim initialPosition As Long = Position
        Dim copysize As Integer
        Try
            Do
                copysize = Math.Min(count, CInt(blockSize - blockOffset))
                EnsureCapacity(Position + copysize)
                Buffer.BlockCopy(buff, offset, blck, CInt(blockOffset), copysize)
                count -= copysize
                offset += copysize
                Position += copysize
            Loop While count > 0
        Catch e As Exception
            Position = initialPosition
            Throw
        End Try
    End Sub

    Public Overrides Function ReadByte() As Integer
        If Position >= lngt Then Return -1
        Dim b As Byte = blck(CInt(blockOffset))
        Position += 1
        Return b
    End Function

    Public Overrides Sub WriteByte(value As Byte)
        EnsureCapacity(Position + 1)
        blck(CInt(blockOffset)) = value
        Position += 1
    End Sub

    Protected Sub EnsureCapacity(intended_length As Long)
        If intended_length > lngt Then lngt = intended_length
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        ' no unmanaged resources
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary>Returns the entire content as a byte array (may fail for very large streams).</summary>
    Public Function ToArray() As Byte()
        Dim firstposition As Long = Position
        Position = 0
        Dim destination As Byte() = New Byte(CInt(Length) - 1) {}
        Read(destination, 0, CInt(Length))
        Position = firstposition
        Return destination
    End Function

    ''' <summary>Reads length bytes from source into this instance at the current position.</summary>
    Public Sub ReadFrom(source As Stream, length As Long)
        Dim buffer As Byte() = New Byte(4095) {}
        Dim read As Integer
        Do
            read = source.Read(buffer, 0, CInt(Math.Min(4096, length)))
            length -= read
            Me.Write(buffer, 0, read)
        Loop While length > 0
    End Sub

    ''' <summary>Writes the entire stream into destination; Position is preserved.</summary>
    Public Sub WriteTo(destination As Stream)
        Dim initialpos As Long = Position
        Position = 0
        Me.CopyTo(destination)
        Position = initialpos
    End Sub
End Class
```

- [ ] **Step 2: Remove the WCFCommon import from `FrmPdfSigner.vb`**

In `PDF Signer for Tungsten Capture\FrmPdfSigner.vb`, delete line 17:

```vb
Imports PDFStreamer.WCFCommon.Helper
```

(The `New MemoryTributary()` usage at ~line 464 now resolves to the local `PDFSigner.MemoryTributary`.)

- [ ] **Step 3: Remove the WCFCommon import from `MNBSigner.vb`**

In `PDF Signer for Tungsten Capture\Signature\Modules\MNBSigner\MNBSigner.vb`, delete line 3:

```vb
Imports PDFStreamer.WCFCommon.Helper
```

(The `New MemoryTributary(...)` usages at ~lines 87 and 96 now resolve to the local type.)

- [ ] **Step 4: Build to verify**

Run the build/verify command from the plan header.
Expected: ` 0 Error(s)`. (`PDFStreamer.WCFCommon` is still referenced; both the old and new `MemoryTributary` no longer collide because the imports were removed.)

- [ ] **Step 5: Commit**

```bash
git add "PDF Signer for Tungsten Capture/Helper/MemoryTributary.vb" "PDF Signer for Tungsten Capture/FrmPdfSigner.vb" "PDF Signer for Tungsten Capture/Signature/Modules/MNBSigner/MNBSigner.vb"
git commit -m "Vendor MemoryTributary locally, drop WCFCommon helper import

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 2: Add the REST client

Create the transport class that owns the `HttpClient`, performs the streamed POST, writes the signed bytes back into the source stream, reads metadata headers, and parses RFC-7807 errors. Also defines the small result and ProblemDetails types it returns/consumes.

**Files:**
- Create: `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb`

> Note: this task adds the file but nothing references it yet, and the project does not yet reference `System.Net.Http` / `System.Runtime.Serialization` — so the build verification for this client happens in Task 4 after the project references are added. This task ends with a commit only (no standalone build).

- [ ] **Step 1: Create `PDFStreamerRestClient.vb`**

Create `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb`:

```vb
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

            Using content As New StreamContent(documentStream)
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
```

- [ ] **Step 2: Commit**

```bash
git add "PDF Signer for Tungsten Capture/Signature/Modules/PDFStreamer/PDFStreamerRestClient.vb"
git commit -m "Add PDF Streamer REST streaming client

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 3: Rewrite the PDFStreamer module and delete the WCF proxy

Rewrite `PDFStreamer.vb` to call the REST client and map the result into `SignatureResult`, preserving the existing public surface (`SignatureOperation.vb` is untouched). Delete `PDFStreamerProxy.vb`.

**Files:**
- Modify (full rewrite): `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamer.vb`
- Delete: `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamerProxy.vb`

- [ ] **Step 1: Rewrite `PDFStreamer.vb`**

Replace the entire contents of `PDF Signer for Tungsten Capture\Signature\Modules\PDFStreamer\PDFStreamer.vb` with:

```vb
Imports PDFSignerCommon

Friend Class PDFStreamer
    Private _settings As PDFStreamerCryptoProvider

    Friend Sub Initialize(ProviderSettings As PDFStreamerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Function SignDocumentStream(Request As SignatureRequest) As SignatureResult
        Dim res As New SignatureResult

        Dim client As New PDFStreamerRestClient
        Dim callResult As PDFStreamerRestResult = client.Sign(
            _settings.PDFStreamerURL,
            _settings.PDFStreamerAuthorizationCode,
            _settings.PDFStreamerConfigFile,
            "1",
            "PDFSigner.pdf",
            Request.FileToSign)

        res.SignatureLog = callResult.DocumentLog
        res.ErrorMessage = callResult.ErrorMessage
        res.ExceptionText = callResult.ExceptionText

        If callResult.ValidUntil.HasValue Then
            res.SignatureExpiration = callResult.ValidUntil.Value
        End If

        If callResult.Success Then
            res.SignedFile = callResult.SignedStream
        End If

        Return res
    End Function
End Class
```

Notes on parity with the old code: transaction id `"1"` and filename `"PDFSigner.pdf"` are preserved; `SignerNameFromCert` is still ignored (no `SignatureResult` field for it); on error `SignedFile` is left `Nothing` so `SignatureResult.Validate` reports the failure.

- [ ] **Step 2: Delete the old WCF proxy**

```bash
git rm "PDF Signer for Tungsten Capture/Signature/Modules/PDFStreamer/PDFStreamerProxy.vb"
```

- [ ] **Step 3: Commit**

(No build yet — the project still references `PDFStreamer.WCFCommon` and not `System.Net.Http`; the build runs in Task 4 after project edits.)

```bash
git add "PDF Signer for Tungsten Capture/Signature/Modules/PDFStreamer/PDFStreamer.vb"
git commit -m "Rewrite PDFStreamer module to use the REST client

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 4: Update project references, Costura allowlist, and delete the WCF service reference

Swap the WCF references for the framework references the REST client needs, drop the dead Costura entries, and delete the generated WCF service reference. This is the task that makes the solution compile again.

**Files:**
- Modify: `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj`
- Modify: `PDF Signer for Tungsten Capture\FodyWeavers.xml`
- Delete: `PDF Signer for Tungsten Capture\Service References\PDFStreamerServiceReference\` (whole folder)
- Delete (if present): `lib\PDF Streamer\PDFStreamer.WCFCommon.dll`

- [ ] **Step 1: Remove the `PDFStreamer.WCFCommon` reference and add framework references**

In `PDF Signer for Tungsten Capture\PDF Signer for Tungsten Capture.vbproj`, delete these lines:

```xml
    <Reference Include="PDFStreamer.WCFCommon">
      <HintPath>..\lib\PDF Streamer\PDFStreamer.WCFCommon.dll</HintPath>
    </Reference>
```

In the same `<ItemGroup>` (the one containing `<Reference Include="System.ServiceModel" />`), add two framework references — place them next to the existing `System.*` references:

```xml
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Runtime.Serialization" />
```

Keep `<Reference Include="System.ServiceModel" />` (MNBSigner still uses WCF).

- [ ] **Step 2: Remove the PDFStreamer WCF metadata items**

In the same `.vbproj`, delete the PDFStreamer service-reference `None Update` item:

```xml
    <None Update="Service References\PDFStreamerServiceReference\Reference.svcmap">
      <Generator>WCF Proxy Generator</Generator>
      <LastGenOutput>Reference.vb</LastGenOutput>
    </None>
```

and delete the PDFStreamer `WCFMetadataStorage` item:

```xml
    <WCFMetadataStorage Include="Service References\PDFStreamerServiceReference\" />
```

Keep `<WCFMetadata Include="Service References\" />` and the MNB `<WCFMetadataStorage Include="Service References\MNBSignerServiceReference\" />` and its `<None Update>` item.

- [ ] **Step 3: Remove dead Costura entries**

In `PDF Signer for Tungsten Capture\FodyWeavers.xml`, remove these two lines from `<IncludeAssemblies>`:

```
				PDFStreamer.Common
				PDFStreamer.WCFCommon
```

(Neither assembly is referenced any longer; leaving them is harmless but stale.)

- [ ] **Step 4: Delete the generated WCF service reference folder**

```bash
git rm -r "PDF Signer for Tungsten Capture/Service References/PDFStreamerServiceReference"
```

- [ ] **Step 5: Delete the vendored dead DLL (if present)**

```bash
git rm --ignore-unmatch "lib/PDF Streamer/PDFStreamer.WCFCommon.dll"
```

If the file is not tracked by git, instead remove it from disk if it exists:

```bash
rm -f "lib/PDF Streamer/PDFStreamer.WCFCommon.dll"
```

- [ ] **Step 6: Build to verify**

Run the build/verify command from the plan header.
Expected: ` 0 Error(s)`. The REST client, the rewritten module, and the two earlier-edited files all compile; no references to `PDFStreamer.WCFCommon` or `PDFStreamerServiceReference` remain.

- [ ] **Step 7: Commit**

```bash
git add "PDF Signer for Tungsten Capture/PDF Signer for Tungsten Capture.vbproj" "PDF Signer for Tungsten Capture/FodyWeavers.xml"
git commit -m "Drop PDF Streamer WCF reference; wire REST framework references

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 5: Require an http/https scheme in setup validation

`PDFStreamerURL` now holds the full REST endpoint URL. Tighten `Validate()` to reject non-http(s) schemes (e.g. legacy `net.tcp://`) with a clear message, and add the resource string it needs.

**Files:**
- Modify: `PDF Signer for Tungsten Capture Common\Language\Messages.resx`
- Modify: `PDF Signer for Tungsten Capture Common\Language\Messages.hu.resx`
- Modify: `PDF Signer for Tungsten Capture Common\Language\Messages.Designer.vb`
- Modify: `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFStreamerCryptoProvider.vb`

- [ ] **Step 1: Add the English resource string**

In `PDF Signer for Tungsten Capture Common\Language\Messages.resx`, immediately after the `PDFStreamer_URL_Missing` `<data>` block (the one whose value is `PDF Streamer URL is not provided.`), add:

```xml
  <data name="PDFStreamer_URL_Not_Http" xml:space="preserve">
    <value>PDF Streamer URL must use http or https.</value>
  </data>
```

- [ ] **Step 2: Add the Hungarian resource string**

In `PDF Signer for Tungsten Capture Common\Language\Messages.hu.resx`, immediately after the `PDFStreamer_URL_Missing` `<data>` block (value `PDF Streamer URL nincs megadva!`), add:

```xml
  <data name="PDFStreamer_URL_Not_Http" xml:space="preserve">
    <value>A PDF Streamer URL csak http vagy https lehet!</value>
  </data>
```

- [ ] **Step 3: Add the Designer property**

In `PDF Signer for Tungsten Capture Common\Language\Messages.Designer.vb`, immediately after the `PDFStreamer_URL_Missing` property block (which ends at its `End Property` near line 428), add:

```vb
        '''<summary>
        '''  Looks up a localized string similar to PDF Streamer URL must use http or https..
        '''</summary>
        Friend Shared ReadOnly Property PDFStreamer_URL_Not_Http() As String
            Get
                Return ResourceManager.GetString("PDFStreamer_URL_Not_Http", resourceCulture)
            End Get
        End Property
```

- [ ] **Step 4: Enforce the scheme in `Validate()`**

In `PDF Signer for Tungsten Capture Common\Models\Setup\Providers\PDFStreamerCryptoProvider.vb`, find this block inside `Validate()`:

```vb
            If Not PDFStreamerURL.IsAbsoluteUri Then
                Return Messages.URL_Not_Complete
            End If
```

and insert the scheme check directly after it:

```vb
            If Not PDFStreamerURL.IsAbsoluteUri Then
                Return Messages.URL_Not_Complete
            End If
            If Not PDFStreamerURL.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) AndAlso
               Not PDFStreamerURL.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) Then
                Return Messages.PDFStreamer_URL_Not_Http
            End If
```

- [ ] **Step 5: Build to verify**

Run the build/verify command from the plan header.
Expected: ` 0 Error(s)`. (`Messages` resolves the new `PDFStreamer_URL_Not_Http` property; the Common project compiles and the runtime exe links against it.)

- [ ] **Step 6: Commit**

```bash
git add "PDF Signer for Tungsten Capture Common/Language/Messages.resx" "PDF Signer for Tungsten Capture Common/Language/Messages.hu.resx" "PDF Signer for Tungsten Capture Common/Language/Messages.Designer.vb" "PDF Signer for Tungsten Capture Common/Models/Setup/Providers/PDFStreamerCryptoProvider.vb"
git commit -m "Require http/https scheme for the PDF Streamer endpoint URL

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 6: Update documentation

Clarify the admin help text and refresh repo guidance to reflect the REST switch. No code/build impact.

**Files:**
- Modify: `Help\Content\Modules\Administration\SetupDocumentClass.htm`
- Modify: `Help\Output\Online Help\Modules\Administration\SetupDocumentClass.htm`
- Modify: `CLAUDE.md`

- [ ] **Step 1: Update the help source**

In `Help\Content\Modules\Administration\SetupDocumentClass.htm`, replace the PDF Streamer URL bullet:

Find:

```html
            <li><b>Szolgáltatás URL-je (Web service URL):</b> a PDF Streamer szolgáltatás elérhetősége.</li>
```

Replace with:

```html
            <li><b>Szolgáltatás URL-je (Web service URL):</b> a PDF Streamer REST végpont teljes URL-je (pl. https://kiszolgalo:port/api/v1/documents/stream).</li>
```

- [ ] **Step 2: Update the generated help copy**

Apply the identical find/replace from Step 1 in `Help\Output\Online Help\Modules\Administration\SetupDocumentClass.htm` (the packaged copy harvested by the installer).

- [ ] **Step 3: Update `CLAUDE.md`**

In `CLAUDE.md`, find the PDF Streamer architecture bullet describing the runtime modules:

```
`MQFTP` (IBM MQ/FTP transfer), and `PDFStreamer` (delegates to the external PDF Streamer Windows service via a WCF service reference).
```

Replace with:

```
`MQFTP` (IBM MQ/FTP transfer), and `PDFStreamer` (delegates to the external PDF Streamer Windows service via its REST streaming endpoint, `POST {configured-url}` → `/api/v1/documents/stream`, using `HttpClient`).
```

Then find the `lib\PDFSigner\` build-gotcha bullet:

```
- `lib\PDFSigner\` holds the vendored **PDF Streamer** client assemblies (`PDFStreamer.WCFCommon.dll`, `PDFStreamer.Common.dll`, `log4net.dll`), refreshed from the PDF Streamer repo's Release build (last refresh 2026-06-11 build, at migration). PDF Streamer lives in its own Git repo at `C:\Projects\PDF Streamer` (github.com/dkekesi/PDF-Streamer); rebuild it there and re-copy these DLLs when the service contract changes. (In the old TFS layout this was a direct cross-solution reference to `PDFStreamer.WCFCommon\bin\Release\net48`.)
```

Replace with:

```
- PDF Signer no longer links any PDF Streamer assemblies: it calls PDF Streamer's REST streaming endpoint (`POST /api/v1/documents/stream`) over `HttpClient` (see `Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb`). PDF Streamer dropped its net48 `PDFStreamer.WCFCommon`/`PDFStreamer.Common` assemblies in its .NET 10 / CoreWCF migration. `MemoryTributary` (previously from WCFCommon) is now a local class at `PDF Signer for Tungsten Capture\Helper\MemoryTributary.vb`. PDF Streamer lives in its own Git repo at `C:\Projects\PDF Streamer` (github.com/dkekesi/PDF-Streamer).
```

- [ ] **Step 4: Commit**

```bash
git add "Help/Content/Modules/Administration/SetupDocumentClass.htm" "Help/Output/Online Help/Modules/Administration/SetupDocumentClass.htm" "CLAUDE.md"
git commit -m "Document the PDF Streamer REST migration

Co-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>"
```

---

## Task 7: Final full-solution verification

Confirm the whole solution still builds clean after all changes.

- [ ] **Step 1: Clean build**

Run the build/verify command from the plan header once more.
Expected: ` 0 Error(s)`.

- [ ] **Step 2: Confirm no lingering WCF references**

Run:

```bash
git grep -n "PDFStreamerServiceReference\|PDFStreamer.WCFCommon\|PDFStreamerProxy\|PDFStreamer.Common" -- "PDF Signer for Tungsten Capture" "PDF Signer for Tungsten Capture Common"
```

Expected: no matches (empty output). If anything prints, it is a missed reference — fix before finishing.

- [ ] **Step 3: Manual smoke test (operator, out of band)**

Not automatable here. With a live PDF Streamer instance that has `RestApiEnabled` true: configure a batch class whose PDF Streamer URL is the full streaming endpoint (`https://host:port/api/v1/documents/stream`), the config filename, and the authorization code; sign a document; confirm the signed PDF is produced, the signature log is stored, and the valid-until index value is populated. Note this as a hand-off item for the user.

---

## Self-Review

**Spec coverage:**
- Replace WCF call with REST streaming → Tasks 2, 3. ✅
- Remove WCF artifacts (proxy, service reference, WCFCommon reference, dead DLL, Costura entries) → Tasks 3, 4. ✅
- Keep module public surface / `SignatureOperation.vb` untouched → Task 3 (identical signatures). ✅
- Keep MNB WCF + `System.ServiceModel` → Task 4 (explicitly retained). ✅
- Full endpoint URL used verbatim → Task 2 (`HttpRequestMessage(..., endpointUrl)`), Task 3 (passes `_settings.PDFStreamerURL`). ✅
- DataContractJsonSerializer error parsing → Task 2. ✅
- `X-Document-Log` (base64) and `X-Valid-Until` from response headers; HTTP/1.1 header fallback → Task 2 (`GetHeader` checks response and content headers). ✅
- Validate() requires http/https + message → Task 5. ✅
- Admin help text + CLAUDE.md → Task 6. ✅
- MemoryTributary vendoring (planning addition) → Task 1. ✅
- Build verification (no test framework) → every task + Task 7. ✅

**Placeholder scan:** No TBD/TODO; every code step has complete code and every command has expected output.

**Type consistency:** `PDFStreamerRestClient.Sign(...)` signature in Task 2 matches the call in Task 3. `PDFStreamerRestResult` members (`Success`, `ErrorMessage`, `ExceptionText`, `DocumentLog`, `ValidUntil`, `SignedStream`) used in Task 3 match the class defined in Task 2. `PDFStreamer_URL_Not_Http` resource name is identical across resx, Designer, and `Validate()` (Task 5). `SignatureResult` members written (`SignatureLog`, `ErrorMessage`, `ExceptionText`, `SignatureExpiration`, `SignedFile`) match `Models\SignatureModel.vb`.
