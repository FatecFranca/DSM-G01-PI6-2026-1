namespace Sleep.Domain.Security.Cryptography
{
    public interface IPasswordEncrypt
    {
        string Encrypt(string password);
    }
}
