using System.Security.Cryptography;
using System.Text;

namespace EyelixEyewear_Project.Helpers
{
    public static class PasswordHelper
    {
        // Hash password với salt
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password cannot be null");

            using (var sha256 = SHA256.Create())
            {
                // Tạo salt ngẫu nhiên
                byte[] saltBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                string salt = Convert.ToBase64String(saltBytes);

                // Hash password + salt
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hash = Convert.ToBase64String(hashBytes);

                // Trả về định dạng: salt:hash
                return $"{salt}:{hash}";
            }
        }

        // Kiểm tra password
        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                string salt = parts[0];
                string hash = parts[1];

                using (var sha256 = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password + salt);
                    byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                    string computedHash = Convert.ToBase64String(hashBytes);

                    return hash == computedHash;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}