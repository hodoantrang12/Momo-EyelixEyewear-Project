using System.ComponentModel.DataAnnotations;
using EyelixEyewear_Project.Models;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class UserProfileViewModel
    {
        // Thông tin hiển thị
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        // Dữ liệu đơn hàng
        public List<Order> Orders { get; set; } = new List<Order>();

        // Form đổi mật khẩu (Optional)
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu mới phải từ 6 ký tự")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string? ConfirmNewPassword { get; set; }

        public bool IsMfaEnabled { get; set; }
    }
}