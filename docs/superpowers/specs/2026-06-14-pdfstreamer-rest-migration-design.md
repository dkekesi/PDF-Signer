# PDF Streamer REST migration — design

**Date:** 2026-06-14
**Project:** PDF Signer for Tungsten Capture
**Status:** Approved (design)

## Problem

The PDF Signer runtime signs documents through PDF Streamer using a WCF service
reference (`Service References\PDFStreamerServiceReference`) and a proxy
(`Signature\Modules\PDFStreamer\PDFStreamerProxy.vb`) that inherits
`WCFProxy(Of IPDFStreamer)` from `PDFStreamer.WCFCommon.dll`. PDF Streamer was
migrated to pure .NET 10 / CoreWCF and **removed** the standalone
`PDFStreamer.WCFCommon` and `PDFStreamer.Common` net48 assemblies (2026-06-14),
so PDF Signer's reference to `lib\PDF Streamer\PDFStreamer.WCFCommon.dll` no
longer resolves and the project cannot build the signing path.

PDF Streamer now exposes a transport-neutral **REST API** (co-hosted on the
service's HTTP/HTTPS Kestrel ports) with a streaming endpoint suitable for large
documents. PDF Signer must call PDF Streamer over this REST streaming endpoint
instead of WCF.

## Scope

- Replace the WCF call to `CreatePDFStream` with an HTTP call to the REST
  streaming endpoint.
- Remove all WCF artifacts specific to PDF Streamer (proxy, service reference,
  `PDFStreamer.WCFCommon` reference, vendored DLL).
- Keep the module's public surface (`PDFStreamer.Initialize`,
  `PDFStreamer.SignDocumentStream`) unchanged so `SignatureOperation.vb` is not
  touched.

**Out of scope:** the MNB signer's WCF service reference (`MNBSignerServiceReference`)
and the `System.ServiceModel` framework reference both stay — MNBSigner still
uses WCF. The buffered REST endpoint (`POST /api/v1/documents`) is not used; PDF
Signer only ever called the streamed operation.

## PDF Streamer REST contract (reference)

Confirmed in the PDF Streamer repo
(`PDFStreamer.Service\RestApi\PdfStreamerApi.cs`, `RestMapping.cs`,
`PDFStreamer.Contracts\RestDtos.cs`).

**Request** — `POST {base}/api/v1/documents/stream`

| Channel | Name | Value |
| --- | --- | --- |
| Header | `X-Api-Key` | authorization code (required; missing → 401) |
| Header | `X-Document-Config` | document configuration filename |
| Header | `X-Transaction-Id` | transaction id |
| Header | `X-Document-Filename` | document file name with extension |
| Header | `X-Attachments` | base64 of a JSON `List<FileDto>` (optional; **not sent** by PDF Signer) |
| Body | — | raw unsigned PDF bytes, `Content-Type: application/octet-stream` |

**Response (success, 200)**

- Body: signed PDF bytes (`application/pdf`).
- Header `X-Valid-Until`: ISO-8601 round-trip (`"o"`) timestamp, when present.
- Header `X-Signer-Name`: signer name from the certificate, when present.
- Header `Content-Disposition`: signed file name.
- `X-Document-Log`: base64 of the UTF-8 processing log. Emitted as an **HTTP/2
  trailer** when the connection supports trailers, otherwise as a **normal
  HTTP/1.1 response header**. The .NET Framework 4.8 `HttpClient` uses HTTP/1.1,
  so PDF Signer reads it as a regular response header.

**Response (error, non-2xx)** — RFC 7807 ProblemDetails JSON with flat
extension members:

```json
{ "title": "...", "status": 400, "detail": "...",
  "errorMessage": "...", "documentLog": "...", "transactionId": "..." }
```

Status mapping: 400 validation, 401 missing/empty key, 403 unauthorized, 404 not
found, 500 other.

## Decisions

- **URL semantics:** `PDFStreamerCryptoProvider.PDFStreamerURL` now holds the
  **full REST endpoint URL** (e.g. `https://host:port/api/v1/documents/stream`)
  and is POSTed to verbatim. No path is appended in code.
- **Error parsing:** use the inbox `System.Runtime.Serialization.Json.DataContractJsonSerializer`
  (a `<Reference Include="System.Runtime.Serialization" />`) to deserialize the
  flat ProblemDetails body. No NuGet package and no new Costura-embedded
  assemblies — `FodyWeavers.xml` uses an explicit `<IncludeAssemblies>` allowlist,
  and System.Text.Json would have dragged ~7 transitive assemblies onto it. The
  success path needs no JSON — all metadata is in headers.
- **`MemoryTributary` vendoring:** `MemoryTributary` (the multi-block memory
  stream) came from the deleted `PDFStreamer.WCFCommon.dll`
  (`PDFStreamer.WCFCommon.Helper`) and is used by `FrmPdfSigner.vb` and
  `MNBSigner.vb` as well as indirectly by the signing flow. It is ported to a
  local VB class `Helper\MemoryTributary.vb` in the runtime project (root
  namespace), and the two `Imports PDFStreamer.WCFCommon.Helper` lines are
  removed. Discovered during planning; not in the original problem statement.
- **Client structure (Approach B):** keep `PDFStreamer.vb` as the
  mapping/orchestration layer and replace `PDFStreamerProxy.vb` with a thin
  `PDFStreamerRestClient` that owns the transport. This mirrors the existing
  mapping/proxy split.
- **Config migration:** existing batch-class configuration is **not**
  auto-migrated. Legacy `net.tcp://`/`.svc` URLs will fail validation; admins
  reconfigure the endpoint. Documented as a deployment step.

## Architecture

```
SignatureOperation.vb
   └─ PDFStreamer.vb            (unchanged public surface; mapping/orchestration)
        └─ PDFStreamerRestClient.vb   (transport: HttpClient, headers, streaming, error parsing)
```

### Component: `PDFStreamerRestClient` (new)

`Signature\Modules\PDFStreamer\PDFStreamerRestClient.vb`

- **Owns** one `Shared ReadOnly` `HttpClient` with `Timeout = InfiniteTimeSpan`;
  per-call deadline supplied via a `CancellationTokenSource` with a generous
  default. TLS 1.2/1.3 is ensured once via `ServicePointManager.SecurityProtocol`.
- **Public method** (synchronous, to match the caller): accepts the endpoint
  URL, authorization code, config filename, transaction id, document filename,
  and the source `Stream`; returns a small transport result carrying the
  outcome — success flag, error message, document log, valid-until, and the
  signed stream (written back into the source stream).
- **Streaming upload:** wraps the source stream (seeked to 0) in `StreamContent`
  with `application/octet-stream`. Because the stream is seekable, `Content-Length`
  is set and the body streams without being fully buffered in memory.
- **Success handling:** copies the response body stream back into the source
  stream — seek to 0, copy in a fixed buffer, then `SetLength(Position)` so a
  signed file shorter than the original leaves no stale trailing bytes (matching
  current behavior). Reads `X-Document-Log` (base64 → UTF-8) and `X-Valid-Until`
  (parsed with round-trip kind) from the response headers.
- **Error handling:** for non-success status, reads the body and deserializes
  the ProblemDetails DTO with `DataContractJsonSerializer`; surfaces
  `errorMessage` (falling back to `detail`, then `title`) and `documentLog`.
  Network/transport exceptions are caught and surfaced as the error message plus
  full exception text.
- **Sync-over-async:** the HTTP work is an async core; the public method blocks
  via `Task.Run(Function() CoreAsync(...)).GetAwaiter().GetResult()` to avoid any
  UI-thread synchronization-context deadlock. `ConfigureAwait(False)` is used in
  the core.
- A private `ProblemDetails` DTO mirrors the relevant fields
  (`title`, `detail`, `errorMessage`, `documentLog`, `transactionId`).

### Component: `PDFStreamer.vb` (rewritten)

`Signature\Modules\PDFStreamer\PDFStreamer.vb`

- `Initialize(ProviderSettings As PDFStreamerCryptoProvider)` unchanged.
- `SignDocumentStream(Request As SignatureRequest) As SignatureResult` unchanged
  signature. Body now:
  - Builds the call from `_settings.PDFStreamerURL`,
    `_settings.PDFStreamerAuthorizationCode`, `_settings.PDFStreamerConfigFile`,
    transaction id `"1"` and filename `"PDFSigner.pdf"` (preserving current
    constants), passing `Request.FileToSign`.
  - Invokes `PDFStreamerRestClient`.
  - Maps the transport result into `SignatureResult`: `documentLog → SignatureLog`,
    `validUntil → SignatureExpiration` (only when present), error →
    `ErrorMessage` / `ExceptionText`. `SignedFile = Request.FileToSign`.
  - `SignerNameFromCert` remains unused (no `SignatureResult` field exists for
    it, matching today's behavior).
- Drops `Imports PDFSigner.PDFStreamerServiceReference` and
  `Imports PDFStreamer.WCFCommon.Helper`.

### Removals

- Delete `Signature\Modules\PDFStreamer\PDFStreamerProxy.vb`.
- Delete `Service References\PDFStreamerServiceReference\` (Reference.vb,
  Reference.svcmap, configuration.svcinfo, configuration91.svcinfo,
  PDFStreamer.wsdl, the `.datasource`).
- `.vbproj`: remove `<Reference Include="PDFStreamer.WCFCommon">`, the PDFStreamer
  `Reference.svcmap` `<None Update>` item, and the PDFStreamer
  `<WCFMetadataStorage>` item. Add `<Reference Include="System.Net.Http" />` and
  `<Reference Include="System.Runtime.Serialization" />` framework references.
  Keep `System.ServiceModel` and the MNB WCF metadata items.
- `FodyWeavers.xml`: remove the `PDFStreamer.Common` and `PDFStreamer.WCFCommon`
  entries from `<IncludeAssemblies>` (neither assembly is referenced any longer).
- Remove the unused vendored `lib\PDF Streamer\PDFStreamer.WCFCommon.dll`.

### Config and documentation

- `PDFStreamerCryptoProvider`: fields and CSS keys are unchanged. Tighten
  `Validate()` so `PDFStreamerURL` must be an absolute URI **and** use an
  `http`/`https` scheme. Update the Hungarian Setup field label / admin help text
  and the validation message to describe a REST endpoint URL.
- Update PDF Signer's `CLAUDE.md`: the `lib\PDFSigner` / `PDFStreamer.WCFCommon`
  note and the PDFStreamer module description now reflect the REST call.

## Error handling summary

| Situation | Result |
| --- | --- |
| 2xx | signed bytes written to `FileToSign`; `SignatureLog`, `SignatureExpiration` set |
| non-2xx with ProblemDetails | `ErrorMessage` = `errorMessage`/`detail`/`title`; `SignatureLog` = `documentLog` |
| transport/network exception | `ErrorMessage` = `ex.Message`; `ExceptionText` = `ex.ToString()` |

`SignatureResult.Validate` already rejects an empty `ErrorMessage` + missing
signed file and a signed file no larger than the original, so no extra
validation is needed in the module.

## Testing / verification

- Build verification: `msbuild "PDF Signer for Tungsten Capture.slnx" /restore
  /p:Configuration=Debug /p:Platform="Any CPU" /p:CopyToCaptureBin=false` and
  confirm `0 Error(s)`.
- No automated tests exist in the repo. Manual smoke test: configure a batch
  class with a live PDF Streamer REST endpoint and sign a document, confirming
  the signed file, the stored signature log, and the valid-until index value.

## Risks / notes

- Large-document streaming relies on the seekable source stream giving a known
  `Content-Length`; HttpClient then streams the body rather than buffering it
  whole.
- The processing log arrives as a normal header on HTTP/1.1 (net48), so no
  trailer support is required on the client.
- Adding `System.Text.Json` increases the Costura-embedded payload slightly;
  acceptable for the cleaner parsing.
