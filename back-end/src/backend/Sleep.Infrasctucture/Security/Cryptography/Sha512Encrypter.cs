using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sleep.Domain.Security.Cryptography;

namespace Sleep.Infrasctructure.Security.Cryptography
{
    public class Sha512Encrypter : IPasswordEncrypt
    {
        private readonly string _salt;

        public Sha512Encrypter(string salt) => _salt = salt;

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
