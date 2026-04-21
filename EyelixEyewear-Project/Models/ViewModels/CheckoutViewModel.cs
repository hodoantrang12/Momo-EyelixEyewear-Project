using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Thông tin người mua
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required]
        public string Province { get; set; }
        [Required]
        public string Ward { get; set; }

        // Thông tin người nhận (nếu khác người mua)
        public bool DifferentShippingAddress { get; set; }
        public string? RecipientFullName { get; set; }
        public string? RecipientPhone { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingProvince { get; set; }
        public string? ShippingWard { get; set; }

        // Thông tin đơn hàng
        public string? OrderNotes { get; set; }
        public string PaymentMethod { get; set; } = "COD"; // COD, Momo, Credit
        public string ShippingMethod { get; set; } = "standard";

        // Dữ liệu hiển thị lại (để không bị null khi load trang)
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public int EyelixPoints { get; set; }
        public string? EstimatedDelivery { get; set; }
        public bool SubscribeEmail { get; set; }
    }
}