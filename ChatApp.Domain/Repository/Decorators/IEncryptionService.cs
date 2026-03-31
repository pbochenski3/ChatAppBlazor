using System;
using System.Collections.Generic;
using System.Text;

namespace ChatApp.Domain.Repository.Decorators
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
