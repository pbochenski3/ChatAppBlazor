using ChatApp.Domain.Repository.Decorators;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Infrastructure.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        public AesEncryptionService(IConfiguration config)
        {
            var keyString = config["Encryption:Key"];
            _key = Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
        }
        public string Decrypt(string cipherText)
        {
            if(string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                var fullCipher = Convert.FromBase64String(cipherText);
                using var aes = Aes.Create();
                aes.Key = _key;
                var iv = new byte[16];
                var cipher = new byte[fullCipher.Length - iv.Length];
                Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception)
            {
                return cipherText;
            }
        }

        public string Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }
            return Convert.ToBase64String(ms.ToArray());
        }
    }
}
