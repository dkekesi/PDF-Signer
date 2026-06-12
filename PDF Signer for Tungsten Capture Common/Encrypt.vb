Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Common methods for encode/decode
''' </summary>
Public Class Encrypt
    Private Const AesIV256 As String = "uB$xu!yCE|2k}1M2"
    Private Const AesKey256 As String = "Ms@fr8U1#BUiVZ^*}MgR8E{gDekTctHM"
    Private Const CipherTextV2Prefix As String = "v2:"
    Private Const IVSizeBytes As Integer = 16 ' AES block size; also the length of the v2 prepended IV

    Private Function CreateAes(IV As Byte()) As AesCryptoServiceProvider
        Return New AesCryptoServiceProvider() With {
            .BlockSize = 128,
            .KeySize = 256,
            .IV = IV,
            .Key = Encoding.UTF8.GetBytes(AesKey256),
            .Mode = CipherMode.CBC,
            .Padding = PaddingMode.PKCS7
        }
    End Function

    Private Function GetRSAKeyFromString(KeyString As String) As RSAParameters
        Dim sr = New IO.StringReader(KeyString)
        Dim xd = New Xml.Serialization.XmlSerializer(GetType(RSAParameters))
        Return xd.Deserialize(sr)
    End Function

    Public Function RSAVerifySignedString(OriginalMessage As String, SignatureString As String) As Boolean
        Dim res As Boolean

        Using rsa = New RSACryptoServiceProvider()
            Dim encoder = New UTF8Encoding()
            Dim bytesToVerify As Byte() = encoder.GetBytes(OriginalMessage)
            Dim signedBytes As Byte() = Convert.FromBase64String(SignatureString)

            Try
                rsa.ImportParameters(GetRSAKeyFromString(My.Resources.LicensePublicKey))
                res = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID("SHA256"), signedBytes)
                Return res
            Catch
                Return False
            Finally
                rsa.PersistKeyInCsp = False
            End Try
        End Using
    End Function

    ''' <summary>
    ''' Encrypt string with AES256 (v2 format: random IV prepended to the ciphertext).
    ''' Use it to encrypt stored login passwords that application must have access to.
    ''' </summary>
    ''' <param name="StringToEncrypt">String to encrypt</param>
    ''' <returns>Encrypted string value ("v2:" + Base64(IV || ciphertext))</returns>
    Public Function AES256Encrypt(StringToEncrypt As String) As String
        If String.IsNullOrEmpty(StringToEncrypt) Then Return String.Empty

        Try
            Dim iv(IVSizeBytes - 1) As Byte
            Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()
                rng.GetBytes(iv)
            End Using

            Using aes256 As AesCryptoServiceProvider = CreateAes(iv)
                Dim src As Byte() = Encoding.Unicode.GetBytes(StringToEncrypt)
                Using encrypt As ICryptoTransform = aes256.CreateEncryptor()
                    Dim cipherBytes As Byte() = encrypt.TransformFinalBlock(src, 0, src.Length)
                    Dim payload(iv.Length + cipherBytes.Length - 1) As Byte
                    Buffer.BlockCopy(iv, 0, payload, 0, iv.Length)
                    Buffer.BlockCopy(cipherBytes, 0, payload, iv.Length, cipherBytes.Length)
                    Return CipherTextV2Prefix + Convert.ToBase64String(payload)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Decrypt string with AES256. Accepts both the v2 format (random IV) and the
    ''' legacy static-IV format for backward compatibility with existing setup data.
    ''' </summary>
    ''' <param name="StringToDecrypt">String to decrypt</param>
    ''' <returns>Decrypted string value</returns>
    Public Function AES256Decrypt(StringToDecrypt As String) As String
        If String.IsNullOrEmpty(StringToDecrypt) Then Return String.Empty

        If StringToDecrypt.StartsWith(CipherTextV2Prefix, StringComparison.Ordinal) Then
            Return AES256DecryptV2(StringToDecrypt.Substring(CipherTextV2Prefix.Length))
        End If

        Return AES256DecryptLegacy(StringToDecrypt)
    End Function

    Private Function AES256DecryptV2(Base64Payload As String) As String
        Try
            Dim payload As Byte() = Convert.FromBase64String(Base64Payload)
            If payload.Length < IVSizeBytes * 2 OrElse (payload.Length - IVSizeBytes) Mod IVSizeBytes <> 0 Then Return Nothing

            Dim iv(IVSizeBytes - 1) As Byte
            Buffer.BlockCopy(payload, 0, iv, 0, IVSizeBytes)

            Using aes256 As AesCryptoServiceProvider = CreateAes(iv)
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(payload, IVSizeBytes, payload.Length - IVSizeBytes)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    Private Function AES256DecryptLegacy(StringToDecrypt As String) As String
        Try
            Dim src As Byte() = Convert.FromBase64String(StringToDecrypt)

            Using aes256 As AesCryptoServiceProvider = CreateAes(Encoding.UTF8.GetBytes(AesIV256))
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(src, 0, src.Length)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function
End Class
