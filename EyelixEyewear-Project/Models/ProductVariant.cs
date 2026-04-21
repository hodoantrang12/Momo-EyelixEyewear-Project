
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EyelixEyewear_Project.Models;

public class ProductVariant
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string VariantName { get; set; } = string.Empty;
    // Ví dụ: Đen – Size M, Bạc – Lens Polarized

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    // Nếu null → dùng Product.Price

    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;

    // FK
    public int ProductId { get; set; } public string ProductName { get; set; } = string.Empty;

    // Navigation
    public Product Product { get; set; } = null!;
}
