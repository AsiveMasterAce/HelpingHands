using System.Security.Cryptography;
using System.Text;

namespace HelpingHands.Services
{
    public class Crypto
    {
        static byte[] GenerateSalt()
        {
            const int SaltLength = 64;
            byte[] salt = new byte[SaltLength];
            var rngRand = new RNGCryptoServiceProvider();
            rngRand.GetBytes(salt);
            return salt;
        }

        static byte[] GenerateMD5Hash(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword =
            new byte[salt.Length + passwordBytes.Length];
            using var hash = new MD5CryptoServiceProvider();
            return hash.ComputeHash(saltedPassword);
        }

        public byte[] Encrypt(string plainText, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            using var memStream = new MemoryStream();
            memStream.Write(aes.IV, 0, aes.IV.Length);
            using var cryptoStream = new CryptoStream(
            memStream,
            aes.CreateEncryptor(),
            CryptoStreamMode.Write);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            cryptoStream.Write(plainTextBytes);
            cryptoStream.FlushFinalBlock();
            memStream.Position = 0;
            return memStream.ToArray();
        }

        public string Decrypt(byte[] cypherBytes, byte[] key)
        {
            using var memStream = new MemoryStream();
            memStream.Write(cypherBytes);
            memStream.Position = 0;
            using var aes = Aes.Create();
            byte[] iv = new byte[aes.IV.Length];
            memStream.Read(iv, 0, iv.Length);
            using var cryptoStream = new CryptoStream(
            memStream,
            aes.CreateDecryptor(key, iv),
            CryptoStreamMode.Read);

            int plainTextByteLength = cypherBytes.Length - iv.Length;
            var plainTextBytes = new byte[plainTextByteLength];
            cryptoStream.Read(plainTextBytes, 0, plainTextByteLength);
            return Encoding.UTF8.GetString(plainTextBytes);
        }


        static byte[] GenerateKey()
        {
            const int KeyLength = 32;
            byte[] key = new byte[KeyLength];
            var rngRand = new RNGCryptoServiceProvider();
            rngRand.GetBytes(key);
            return key;
        }
    }
}
