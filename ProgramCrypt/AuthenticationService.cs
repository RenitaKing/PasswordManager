using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PasswordManager
{
    public class AuthenticationService
    {
        //This module is responsible for handling the master password
        private readonly string _authFile = "auth.json";
        private AuthData _authData=null!;

        public AuthenticationService()
        {
            if (File.Exists(_authFile))
                LoadAuthData();
            else
            {
                // Initialize with empty default so _authData is never null
                _authData = new AuthData();
            }
        }

        public bool HasMasterPassword => File.Exists(_authFile);

        public void CreateMasterPassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty.");

            byte[] salt = GenerateSalt();
            byte[] hash = HashPassword(newPassword, salt);

            _authData = new AuthData
            {
                Salt = Convert.ToBase64String(salt),
                Hash = Convert.ToBase64String(hash)
            };

            File.WriteAllText(_authFile, JsonSerializer.Serialize(_authData));
        }

        public void ChangeMasterPassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty.", nameof(newPassword));

            byte[] salt = GenerateSalt();
            byte[] hash = HashPassword(newPassword, salt);

            _authData = new AuthData
            {
                Salt = Convert.ToBase64String(salt),
                Hash = Convert.ToBase64String(hash)
            };

            File.WriteAllText(_authFile, JsonSerializer.Serialize(_authData));
        }


        public bool Authenticate(string inputPassword)
        {
            byte[] salt = Convert.FromBase64String(_authData.Salt);
            byte[] storedHash = Convert.FromBase64String(_authData.Hash);
            byte[] enteredHash = HashPassword(inputPassword, salt);

            return CryptographicOperations.FixedTimeEquals(storedHash, enteredHash);
        }

        private void LoadAuthData()
        {
            var json = File.ReadAllText(_authFile);
            _authData = JsonSerializer.Deserialize<AuthData>(json)?? new AuthData();
        }

        //Convert our password to hash/gibberish...
        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32); // 256-bit hash
            }
        }
        private static byte[] GenerateSalt()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            return salt;
        }
        private class AuthData
        {
            public string Salt { get; set; } = string.Empty;
            public string Hash { get; set; } = string.Empty;
        }
    }
}


