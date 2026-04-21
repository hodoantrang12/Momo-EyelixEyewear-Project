using OtpNet;
using System.Text;

namespace EyelixEyewear_Project.Helpers
{
    public static class TotpHelper
    {
        // Tạo secret key ngẫu nhiên (base32)
        public static string GenerateSecretKey()
        {
            var key = KeyGeneration.GenerateRandomKey(20); // 160 bits
            return Base32Encoding.ToString(key);
        }

        // Tạo URI để Google Authenticator scan QR
        public static string GenerateQrCodeUri(string secretKey, string email, string issuer = "Eyelix Eyewear")
        {
            // Format chuẩn: otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}
            return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
                   $"?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}&algorithm=SHA1&digits=6&period=30";
        }

        // Xác thực OTP người dùng nhập
        public static bool VerifyOtp(string secretKey, string otpCode)
        {
            try
            {
                if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(otpCode))
                    return false;

                var keyBytes = Base32Encoding.ToBytes(secretKey);
                var totp = new Totp(keyBytes);

                // VerificationWindow(1,1) = chấp nhận OTP trước/sau 30 giây
                // để tránh lỗi do lệch giờ nhẹ giữa server và điện thoại
                return totp.VerifyTotp(otpCode, out _, new VerificationWindow(1, 1));
            }
            catch
            {
                return false;
            }
        }

        // Lấy OTP hiện tại (dùng để test/debug)
        public static string GetCurrentOtp(string secretKey)
        {
            var keyBytes = Base32Encoding.ToBytes(secretKey);
            var totp = new Totp(keyBytes);
            return totp.ComputeTotp();
        }
    }
}