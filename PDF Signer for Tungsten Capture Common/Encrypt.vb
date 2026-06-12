Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Common methods for encode/decode
''' </summary>
Public Class Encrypt
    Private Const AesIV256 As String = "uB$xu!yCE|2k}1M2"
    Private Const AesKey256 As String = "Ms@fr8U1#BUiVZ^*}MgR8E{gDekTctHM"
    Private Const CipherTextV2Prefix As String = "v2:"

    Private Function GetRSAKeyFromString(KeyString As String) As RSAParameters
        Dim sr = New IO.StringReader(KeyString)
        Dim xd = New Xml.Serialization.XmlSerializer(GetType(RSAParameters))
        Return xd.Deserialize(sr)
    End Function

    Public Function RSAVerifySignedString(OriginalMessage As String, SignatureString As String) As Boolean
        Dim res As Boolean

        Using rsa = New RSACryptoServiceProvider()
            'Don't do this, do the same as you did in SignData:
            'byte[] bytesToVerify = Convert.FromBase64String(originalMessage);
            Dim encoder = New UTF8Encoding()
            Dim bytesToVerify As Byte() = encoder.GetBytes(OriginalMessage)
            Dim signedBytes As Byte() = Convert.FromBase64String(SignatureString)

            Try
                rsa.ImportParameters(GetRSAKeyFromString(My.Resources.LicensePublicKey))
                Dim Hash = New SHA256Managed()
                Dim hashedData As Byte() = Hash.ComputeHash(signedBytes)
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
            Dim iv(15) As Byte
            Using rng As RandomNumberGenerator = RandomNumberGenerator.Create()
                rng.GetBytes(iv)
            End Using

            Using aes256 As New AesCryptoServiceProvider() With {
                .BlockSize = 128,
                .KeySize = 256,
                .IV = iv,
                .Key = Encoding.UTF8.GetBytes(AesKey256),
                .Mode = CipherMode.CBC,
                .Padding = PaddingMode.PKCS7
            }
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
    ''' legacy static-IV format for backward compatibility with existing setup data
    ''' and license files.
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
            If payload.Length <= 16 Then Return Nothing

            Dim iv(15) As Byte
            Buffer.BlockCopy(payload, 0, iv, 0, 16)

            Using aes256 As New AesCryptoServiceProvider() With {
                .BlockSize = 128,
                .KeySize = 256,
                .IV = iv,
                .Key = Encoding.UTF8.GetBytes(AesKey256),
                .Mode = CipherMode.CBC,
                .Padding = PaddingMode.PKCS7
            }
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(payload, 16, payload.Length - 16)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            End Using
        Catch
            Return Nothing
        End Try
    End Function

    Private Function AES256DecryptLegacy(StringToDecrypt As String) As String
        Using aes256 As New AesCryptoServiceProvider() With {
            .BlockSize = 128,
            .KeySize = 256,
            .IV = Encoding.UTF8.GetBytes(AesIV256),
            .Key = Encoding.UTF8.GetBytes(AesKey256),
            .Mode = CipherMode.CBC,
            .Padding = PaddingMode.PKCS7
        }
            Dim src As Byte() = System.Convert.FromBase64String(StringToDecrypt)

            Try
                Using decrypt As ICryptoTransform = aes256.CreateDecryptor()
                    Dim dest As Byte() = decrypt.TransformFinalBlock(src, 0, src.Length)
                    Return Encoding.Unicode.GetString(dest)
                End Using
            Catch
                Return Nothing
            End Try
        End Using
    End Function
End Class
