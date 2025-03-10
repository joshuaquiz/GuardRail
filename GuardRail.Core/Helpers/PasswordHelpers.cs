using System.Security.Cryptography;
using System;

namespace GuardRail.Core.Helpers;

/// <summary>
/// Helpers for password hashing.
/// </summary>
/// <remarks>
/// Taken from https://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129 and updated.
/// </remarks>
public static class PasswordHelpers
{
    /// <summary>
    /// Size of salt.
    /// </summary>
    private const int SaltSize = 16;

    /// <summary>
    /// Size of hash.
    /// </summary>
    private const int HashSize = 20;

    /// <summary>
    /// Creates a hash from a password.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="iterations">Number of iterations.</param>
    /// <returns>The hash.</returns>
    public static string Hash(
        string password,
        int iterations)
    {
        // Create salt
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Create hash
        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA3_256);
        var hash = pbkdf2.GetBytes(
            HashSize);

        // Combine salt and hash
        var hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(
            salt,
            0,
            hashBytes,
            0,
            SaltSize);
        Array.Copy(
            hash,
            0,
            hashBytes,
            SaltSize,
            HashSize);

        // Convert to base64
        var base64Hash = Convert.ToBase64String(
            hashBytes);

        // Format hash with extra information
        return $"$MYHASH$V1${iterations}${base64Hash}";
    }

    /// <summary>
    /// Creates a hash from a password with 10000 iterations
    /// </summary>
    /// <param name="password">The password.</param>
    /// <returns>The hash.</returns>
    public static string Hash(
        string password) =>
        Hash(
            password,
            10000);

    /// <summary>
    /// Checks if hash is supported.
    /// </summary>
    /// <param name="hashString">The hash.</param>
    /// <returns>Is supported?</returns>
    public static bool IsHashSupported(
        string hashString) =>
        hashString
            .Contains(
                "$MYHASH$V1$");

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    /// <param name="password">The password.</param>
    /// <param name="hashedPassword">The hash.</param>
    /// <returns>Could be verified?</returns>
    public static bool Verify(
        string password,
        string hashedPassword)
    {
        // Check hash
        if (!IsHashSupported(
                hashedPassword))
        {
            throw new NotSupportedException(
                "The hashtype is not supported");
        }

        // Extract iteration and Base64 string
        var splitHashString = hashedPassword
            .Replace(
                "$MYHASH$V1$",
                string.Empty)
            .Split(
                '$');
        var iterations = int.Parse(
            splitHashString[0]);
        var base64Hash = splitHashString[1];

        // Get hash bytes
        var hashBytes = Convert.FromBase64String(
            base64Hash);

        // Get salt
        var salt = new byte[SaltSize];
        Array.Copy(
            hashBytes,
            0,
            salt,
            0,
            SaltSize);

        // Create hash with given salt
        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA3_256);
        var hash = pbkdf2.GetBytes(
            HashSize);

        // Get result
        for (var i = 0; i < HashSize; i++)
        {
            if (hashBytes[i + SaltSize] != hash[i])
            {
                return false;
            }
        }

        return true;
    }
}