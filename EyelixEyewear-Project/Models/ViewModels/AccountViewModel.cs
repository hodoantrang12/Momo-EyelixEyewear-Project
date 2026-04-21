using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class SignupViewModel
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải từ 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }
    }

    // ViewModel cho trang nhập OTP sau khi login
    public class MfaVerifyViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã OTP")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải đúng 6 số")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Chỉ nhập số")]
        public string OtpCode { get; set; } = string.Empty;

        // Lưu thông tin để sau khi xác thực xong biết đăng nhập user nào
        public string ReturnUrl { get; set; } = string.Empty;
    }

    // ViewModel cho trang setup MFA lần đầu
    public class MfaSetupViewModel
    {
        public string SecretKey { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string QrCodeUri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mã OTP để xác nhận")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải đúng 6 số")]
        public string OtpCode { get; set; } = string.Empty;
    }
}