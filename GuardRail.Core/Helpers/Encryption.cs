using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace GuardRail.Core.Helpers;

public static class Encryption
{
    private static readonly byte[] DefaultSalt = "GuardRail"u8.ToArray();

    /// <summary>
    /// Encrypts a string using AES encryption with a key derived from a password.
    /// </summary>
    /// <param name="plainText">The text to encrypt.</param>
    /// <param name="password">The password to use for key derivation.</param>
    /// <returns>The encrypted string, or null if an error occurs.</returns>
    public static string? Encrypt(
        string? plainText,
        string password)
    {
        try
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return null;
            }

            var (key, iv) = DeriveKeyAndIv(
                password,
                DefaultSalt);
            byte[] encrypted;
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                var encryptor = aesAlg.CreateEncryptor(
                    aesAlg.Key,
                    aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error encrypting data: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Decrypts a string using AES decryption with a key derived from a password.
    /// </summary>
    /// <param name="cipherText">The text to decrypt.</param>
    /// <param name="password">The password to use for key derivation.</param>
    /// <returns>The decrypted string, or null if an error occurs.</returns>
    public static string? Decrypt(string? cipherText, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return null;
            }

            // Derive the key and IV from the password and salt.
            var (key, iv) = DeriveKeyAndIv(password, DefaultSalt);
            var cipherBytes = Convert.FromBase64String(cipherText);
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;
            using var msDecrypt = new MemoryStream(cipherBytes);
            using var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV), CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error decrypting data: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Derives the AES key and IV from the provided password and salt using PBKDF2.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="salt">The salt.</param>
    /// <returns>A tuple containing the key and IV.</returns>
    private static (byte[] key, byte[] iv) DeriveKeyAndIv(string password, byte[] salt)
    {
        // Derive the key (256 bits for AES-256)
        var key = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32);

        // Derive the IV (128 bits for AES IV).  Use a different part of the derived key.
        var iv = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 16);
        return (key, iv);
    }
}