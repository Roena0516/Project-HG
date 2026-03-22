using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class EncryptionHelper
{
    private static readonly string encryptionKey = "RoenaSecureKey12";  // 16, 24, or 32 characters

    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);  // 키는 고정된 값 (16, 24, 또는 32 바이트)
        byte[] iv = new byte[16];  // AES는 16바이트 IV를 사용

        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(iv);  // IV를 난수로 생성
        }

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.IV = iv;
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                // IV를 함께 저장 (IV + 암호화된 텍스트)
                byte[] result = new byte[iv.Length + encryptedBytes.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);
                return Convert.ToBase64String(result);
            }
        }
    }

    public static string Decrypt(string encryptedText)
    {
        try
        {
            Debug.Log($"[Decrypt] 입력 문자열: {encryptedText}");

            // 🔹 URL 인코딩 해제 + Base64 정규화 시도
            string normalized = Uri.UnescapeDataString(encryptedText)
                .Replace('-', '+')
                .Replace('_', '/');

            switch (normalized.Length % 4)
            {
                case 2: normalized += "=="; break;
                case 3: normalized += "="; break;
            }

            Debug.Log($"[Decrypt] 정규화된 문자열: {normalized}");

            byte[] fullCipher = Convert.FromBase64String(normalized);

            byte[] iv = new byte[16];
            byte[] cipherText = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    byte[] plainBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(plainBytes);
                }
            }
        }
        catch (FormatException e)
        {
            Debug.LogError($"[Decrypt] Base64 변환 실패! 입력값: {encryptedText}");
            Debug.LogException(e);
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"[Decrypt] 일반 예외 발생! 입력값: {encryptedText}");
            Debug.LogException(e);
            return null;
        }
    }
}