// using System.Security.Cryptography;
// using System.Text;

// public class PasswordHash
// {
//     private string _password;
//     private byte[] salt;
//     private int keySize = 64;
//     private int iterations = 350000;
//     private HashAlgorithmName hashAlgorithm ;

//     public PasswordHash( byte[] salt){

//         this.salt = salt; 
//         this.hashAlgorithm = HashAlgorithmName.SHA512;
//     }

//     public string HashPassword(string password)
//     {
//         var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), this.salt, this.iterations, this.hashAlgorithm, this.keySize);

//         return Convert.ToHexString(hash);
//     }

//     public bool VerifyPassword(string password, string hash)
//     {
//         var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, this.salt, this.iterations, this.hashAlgorithm, this.keySize);

//         return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
//     }
// }


using System;
using System.Security.Cryptography;
using System.Text;

public class PasswordHasher
{
    // Generate a password hash
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    // Verify a password against a hashed password
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        string hashedInput = HashPassword(password);
        return string.Equals(hashedInput, hashedPassword);
    }
}
