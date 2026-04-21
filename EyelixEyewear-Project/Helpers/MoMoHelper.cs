using System.Security.Cryptography;
using System.Text;
namespace EyelixEyewear_Project.Helpers
{
    public class MoMoHelper
    {
        public static string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);
            using var hmac = new HMACSHA256(keyBytes);
            var hashBytes = hmac.ComputeHash(messageBytes);
            return BitConverter.ToString(hashBytes)
                               .Replace("-", "")
                               .ToLower();
        }
    }
}
