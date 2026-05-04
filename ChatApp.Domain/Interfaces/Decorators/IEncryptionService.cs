namespace ChatApp.Domain.Interfaces.Decorators
{
    public interface IEncryptionService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
