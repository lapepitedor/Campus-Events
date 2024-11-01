using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Campus_Events.Misc
{
    public class PasswordHelper
    {
        private Random random = new Random();
        private GeneralSettings settings;

        public PasswordHelper(IOptions<GeneralSettings> settings)
        {
            this.settings = settings.Value;
        }

        public string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public string GenerateToken()
        {
            return RandomString(64);
        }

        public string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(rawData + settings.EncryptionSalt);
                bytes = sha256Hash.ComputeHash(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
