Imports PDFSigner.MNBSignerServiceReference
Imports PDFSignerCommon
Imports PDFStreamer.WCFCommon.Helper
Imports System.IO
Imports System.Security.Cryptography
Imports System.ServiceModel
Imports System.Text

Friend Class MNBSigner
    Private ReadOnly _sbSignLog As New StringBuilder
    Private _settings As MNBSignerCryptoProvider

    Friend Sub Initialize(ProviderSettings As MNBSignerCryptoProvider)
        _settings = ProviderSettings
    End Sub

    Friend Function SignDocument(Request As SignatureRequest) As SignatureResult
        Dim res As SignatureResult
        Dim bind As BasicHttpBinding = GetHttpBinding()

        Using clt As New MNBSignerServiceClient(bind, New EndpointAddress(_settings.MNBSignerURL.ToString))
            If _settings.IsMNBSignerUserPasswordAuthentication Then
                bind.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic
                clt.ClientCredentials.UserName.UserName = Path.Combine(_settings.MNBSignerDomain, _settings.MNBSignerUserName)
                clt.ClientCredentials.UserName.Password = _settings.MNBSignerPassword
            End If

            If _settings.IsMNBSignerWindowsAuthentication Then
                bind.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm
            End If

            If Request.FileToSign.Length <= _settings.MNBSignerChunkSize Then
                _sbSignLog.AppendLine("Dokumentum aláírása MNBSigner-rel")
                res = SignSinglePass(Request, clt)
                _sbSignLog.AppendLine("Dokumentum aláírása MNBSigner-rel befejeződött")
            Else
                _sbSignLog.AppendLine("Dokumentum darabolt aláírása MNBSigner-rel")
                res = SignMultiPass(Request, clt)
                _sbSignLog.AppendLine("Dokumentum darabolt aláírása MNBSigner-rel befejeződött")
            End If
        End Using

        Return res
    End Function

    Private Function SignSinglePass(Request As SignatureRequest, Client As MNBSignerServiceClient) As SignatureResult
        Dim res As New SignatureResult

        ' read the whole document into the request buffer without an intermediate copy
        Dim PdfBin(CInt(Request.FileToSign.Length) - 1) As Byte
        Request.FileToSign.Seek(0, SeekOrigin.Begin)
        Dim offset As Integer = 0
        While offset < PdfBin.Length
            Dim bytesRead As Integer = Request.FileToSign.Read(PdfBin, offset, PdfBin.Length - offset)
            If bytesRead = 0 Then Exit While
            offset += bytesRead
        End While

        Dim response As ReturnModelOfFileContentModeliOGFhNrL

        Try
            _sbSignLog.AppendLine("Szolgáltatás hívása")
            response = Client.ServerSignPDF(PdfBin)
        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = ex.ToString
            Return res
        End Try

        res.ErrorMessage = GetErrors(response)
        res.SignatureLog = _sbSignLog.ToString

        If response.Value IsNot Nothing AndAlso response.Value.Content IsNot Nothing Then
            Dim mt As New MemoryTributary(response.Value.Content)
            res.SignedFile = mt
        End If

        Return res
    End Function

    Private Function SignMultiPass(Request As SignatureRequest, Client As MNBSignerServiceClient) As SignatureResult
        Dim res As New SignatureResult
        Dim mtSigned As New MemoryTributary

        Dim NumberOfChunks As Integer = Math.Ceiling(Request.FileToSign.Length / _settings.MNBSignerChunkSize)
        If NumberOfChunks = 0 Then
            NumberOfChunks = 1
        End If

        Try
            ' ************ initialzing chunked signing ************
            _sbSignLog.AppendLine($"Darabolt aláírás inicializálása (fájlméret: {Request.FileToSign.Length}, {_settings.MNBSignerChunkSize} byte-onként darabolva, összesen {NumberOfChunks} darab)")

            ' create file hash
            Dim HashText As String
            Using sha = SHA256.Create()
                Request.FileToSign.Seek(0, SeekOrigin.Begin)
                HashText = BitConverter.ToString(sha.ComputeHash(Request.FileToSign)).Replace("-", String.Empty).ToLower
            End Using

            ' initializing multi-part sending
            Dim InitRes As ReturnModelOfInitServerSignChunkResultModeliOGFhNrL =
                Client.InitServerSignChunk(New FileInfoModel With {
                                            .Name = "1.pdf",
                                            .Size = Request.FileToSign.Length,
                                            .ChunkSize = _settings.MNBSignerChunkSize,
                                            .Hash = HashText,
                                            .NumberOfChunk = NumberOfChunks
                                        },
                                        SignatureTypeEnum.Pdf,
                                        True)

            If Not InitRes.Success OrElse InitRes.Value Is Nothing OrElse String.IsNullOrWhiteSpace(InitRes.Value.Guid) Then
                res.ErrorMessage = GetErrors(InitRes)
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If

            ' ************ sending chunks ************
            Request.FileToSign.Seek(0, SeekOrigin.Begin)
            For i As Integer = 1 To NumberOfChunks
                _sbSignLog.AppendLine($"{i}/{NumberOfChunks}. darab küldése")

                Dim CurrentChunkSize As Integer
                If Request.FileToSign.Length - i * _settings.MNBSignerChunkSize > 0 Then
                    CurrentChunkSize = _settings.MNBSignerChunkSize
                Else
                    CurrentChunkSize = Request.FileToSign.Length - (i - 1) * _settings.MNBSignerChunkSize
                End If

                Dim PdfChunk = New Byte(CurrentChunkSize - 1) {}
                Request.FileToSign.Read(PdfChunk, 0, CurrentChunkSize)

                Dim ChunkSendRes As ReturnModelOfSendServerSignChunkResultModeliOGFhNrL =
                    Client.SendServerSignChunk(InitRes.Value.Guid,
                                            PdfChunk,
                                            i)

                If Not ChunkSendRes.Success Then
                    res.ErrorMessage = GetErrors(ChunkSendRes)
                    res.SignatureLog = _sbSignLog.ToString
                    mtSigned.Dispose()
                    Return res
                End If
            Next

            ' ************ waiting for server to finish ************
            Dim StartTime As Date = Now
            Dim SigningStatus As Integer = 1 ' initialize with 1 as that means signing on the server is still in progress
            Dim WaitSeconds As Integer = 5 ' number of seconds to wait between tries
            Dim ServerRes As ReturnModelOfWaitForSignServerSignChunkResultModeliOGFhNrL = Nothing

            _sbSignLog.AppendLine("Várakozás az aláírás elkészültére")

            While (Now - StartTime).TotalSeconds < _settings.MNBSignerSigningTimeout AndAlso SigningStatus = 1
                Threading.Thread.Sleep(WaitSeconds * 1000)
                ServerRes = Client.WaitForSignServerSignChunk(InitRes.Value.Guid)
                SigningStatus = ServerRes.Value?.Result
                _sbSignLog.AppendLine($"{(Now - StartTime).TotalSeconds} mp várakozás eltelt...")
            End While

            If SigningStatus = 1 Then ' timeout
                res.ErrorMessage = $"Időtúllépés: nem érkezett aláírt fájl a szerverről {(Now - StartTime).TotalSeconds} mp alatt."
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If

            If SigningStatus = -1 OrElse Not ServerRes.Success Then ' error while signing
                res.ErrorMessage = GetErrors(ServerRes)
                res.SignatureLog = _sbSignLog.ToString
                mtSigned.Dispose()
                Return res
            End If

            ' ************ get signed file chunks from server ************
            If SigningStatus = 0 Then

                _sbSignLog.AppendLine($"Aláírás elkészült, fájl mérete: {ServerRes.Value.FileInfo.Size} byte")

                For i As Integer = 1 To ServerRes.Value.FileInfo.NumberOfChunk
                    _sbSignLog.AppendLine($"{i}/{ServerRes.Value.FileInfo.NumberOfChunk}. darab letöltése")

                    Dim ChunkReceiveRes As ReturnModelOfGetServerSignChunkResultModeliOGFhNrL =
                        Client.GetServerSignChunk(InitRes.Value.Guid, i)

                    If Not ChunkReceiveRes.Success Then
                        res.ErrorMessage = GetErrors(ChunkReceiveRes)
                        res.SignatureLog = _sbSignLog.ToString
                        mtSigned.Dispose()
                        Return res
                    End If

                    _sbSignLog.AppendLine($"{i}/{ServerRes.Value.FileInfo.NumberOfChunk}. darab írása az aláírt fájlt tartalmazó streambe")
                    mtSigned.Write(ChunkReceiveRes.Value.Chunk, 0, ChunkReceiveRes.Value.Chunk.Length)
                Next

                _sbSignLog.AppendLine($"Darabok letöltése befejeződött, {mtSigned.Length} byte kiírva az aláírt fájlt tartalmazó streambe")
            End If
        Catch ex As Exception
            res.ErrorMessage = ex.Message
            res.SignatureLog = ex.ToString
            mtSigned.Dispose()
            Return res
        End Try

        res.SignatureLog = _sbSignLog.ToString
        res.SignedFile = mtSigned

        Return res
    End Function

    Private Function GetHttpBinding() As BasicHttpBinding
        If _settings.MNBSignerURL.Scheme = Uri.UriSchemeHttp Then
            Dim sec = New BasicHttpSecurity With {
                .Mode = BasicHttpSecurityMode.TransportCredentialOnly
            }

            Dim bnd As New BasicHttpBinding With {
                .MaxReceivedMessageSize = Integer.MaxValue,
                .MaxBufferPoolSize = 0,
                .MaxBufferSize = Integer.MaxValue,
                .TransferMode = TransferMode.Buffered,
                .Security = sec,
                .SendTimeout = New TimeSpan(0, 0, _settings.MNBSignerWSTimeout)
            }

            Dim defReader = bnd.ReaderQuotas
            defReader.MaxArrayLength = Integer.MaxValue
            defReader.MaxStringContentLength = Integer.MaxValue
            bnd.ReaderQuotas = defReader

            Return bnd
        End If

        If _settings.MNBSignerURL.Scheme = Uri.UriSchemeHttps Then
            Dim bnd As New BasicHttpBinding(BasicHttpSecurityMode.Transport) With {
                .MaxReceivedMessageSize = Integer.MaxValue,
                .MaxBufferPoolSize = 0,
                .MaxBufferSize = Integer.MaxValue,
                .TransferMode = TransferMode.Buffered,
                .SendTimeout = New TimeSpan(0, 0, _settings.MNBSignerWSTimeout)
            }

            Dim defReader = bnd.ReaderQuotas
            defReader.MaxArrayLength = Integer.MaxValue
            defReader.MaxStringContentLength = Integer.MaxValue
            bnd.ReaderQuotas = defReader

            Return bnd
        End If

        Return Nothing
    End Function

    Private Function GetErrors(Response As ReturnModel) As String
        Dim res As String = String.Empty

        If Response.Messages IsNot Nothing Then
            'res = Join(Response.Messages.Textk__BackingField, ", ")
            For Each msg As Message In Response.Messages
                res += msg.Textk__BackingField + ", "
            Next
        End If

        If res.Length < 2 Then
            Return res
        End If

        Return res.Substring(0, res.Length - 2) ' cut off last comma and space
    End Function
End Class
