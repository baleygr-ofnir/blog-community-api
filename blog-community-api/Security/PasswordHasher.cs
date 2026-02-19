using System.Security.Cryptography;

namespace blog_community_api.Security;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private const char Delimiter = ';';
    private static readonly HashAlgorithmName s_algorithmName = HashAlgorithmName.SHA256;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2
            (
                password,
                salt,
                Iterations,
                s_algorithmName,
                HashSize
            );

        return string.Join
            (
                Delimiter,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(hash),
                Iterations.ToString()
            );
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        string[] parts = storedHash.Split(Delimiter);
        if (parts.Length != 3) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);
        int iterations = int.Parse(parts[2]);
        
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2
            (
                password,
                salt,
                iterations,
                s_algorithmName,
                hash.Length
            );
        
        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}