using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordManager
{
    public class EncryptionService
    {
        // Tunables
        private const int SaltSize = 16;        // 128-bit
        private const int NonceSize = 12;       // 96-bit (AES-GCM recommended)
        private const int TagSize = 16;         // 128-bit tag
        private const int Iterations = 100_000; // PBKDF2 iterations
        private const int KeySize = 32;         // 256-bit key

        // Encrypts plaintext using master password. Output = Base64( salt | nonce | ciphertext | tag )
        public string EncryptString(string plaintext, string masterPassword)
        {
            if (plaintext == null) plaintext = string.Empty;
            byte[] salt = RandomBytes(SaltSize);
            byte[] key = DeriveKey(masterPassword, salt);
            byte[] nonce = RandomBytes(NonceSize);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            using (var aes = new AesGcm(key,16))
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }

            // Concatenate salt | nonce | ciphertext | tag
            byte[] packed = new byte[salt.Length + nonce.Length + cipherBytes.Length + tag.Length];
            Buffer.BlockCopy(salt, 0, packed, 0, salt.Length);
            Buffer.BlockCopy(nonce, 0, packed, salt.Length, nonce.Length);
            Buffer.BlockCopy(cipherBytes, 0, packed, salt.Length + nonce.Length, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, packed, salt.Length + nonce.Length + cipherBytes.Length, tag.Length);

            return Convert.ToBase64String(packed);
        }

        // Decrypts Base64( salt | nonce | ciphertext | tag ) using master password
        public string DecryptString(string packedBase64, string masterPassword)
        {
            if (string.IsNullOrWhiteSpace(packedBase64)) return string.Empty;

            byte[] packed = Convert.FromBase64String(packedBase64);
            if (packed.Length < SaltSize + NonceSize + TagSize)
                throw new CryptographicException("Ciphertext is too short.");

            byte[] salt = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            int cipherLen = packed.Length - (SaltSize + NonceSize + TagSize);
            byte[] cipherBytes = new byte[cipherLen];

            Buffer.BlockCopy(packed, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(packed, SaltSize, nonce, 0, NonceSize);
            Buffer.BlockCopy(packed, SaltSize + NonceSize, cipherBytes, 0, cipherLen);
            Buffer.BlockCopy(packed, SaltSize + NonceSize + cipherLen, tag, 0, TagSize);

            byte[] key = DeriveKey(masterPassword, salt);
            byte[] plainBytes = new byte[cipherBytes.Length];

            using (var aes = new AesGcm(key,16))
            {
                aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }

        private static byte[] DeriveKey(string masterPassword, byte[] salt)
        {
            using var kdf = new Rfc2898DeriveBytes(masterPassword, salt, Iterations, HashAlgorithmName.SHA256);
            return kdf.GetBytes(KeySize);
        }

        private static byte[] RandomBytes(int size)
        {
            byte[] b = new byte[size];
            RandomNumberGenerator.Fill(b);
            return b;
        }
    }
}
