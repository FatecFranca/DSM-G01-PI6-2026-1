using Sleep.Domain.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Sleep.Application.Services.Cryptography
{
    public class PasswordEncrypter : IPasswordEncrypt
    {
        private readonly string _salt;

        public PasswordEncrypter(string salt) => _salt = salt;

        public string Encrypt(string password)
        {
            var newPassword = $"{password}{_salt}";

            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = SHA512.HashData(bytes);

            return StringBytes(hashBytes);
        }

        private static string StringBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }
    }
}
