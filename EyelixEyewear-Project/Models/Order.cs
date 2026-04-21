using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EyelixEyewear_Project.Models;

public class Order
{
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;
    public int UserId { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal Discount { get; set; }

    // Thông tin giao hàng
    [MaxLength(100)]
    public string ShippingName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingDistrict { get; set; }= string.Empty;
    public string ShippingWard { get; set; } = string.Empty;

    // Status 
    [MaxLength(30)]
    public string Status { get; set; } = "Pending";
    // Pending, Processing, Shipped, Delivered, Cancelled

    [MaxLength(30)]
    public string PaymentMethod { get; set; } = "COD";
    // COD, Banking, Momo, VNPay

    [MaxLength(30)]
    public string PaymentStatus { get; set; } = "Unpaid";
    // Unpaid, Paid

    public string? Note { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ShippedDate { get; set; }
    public DateTime? DeliveredDate { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
