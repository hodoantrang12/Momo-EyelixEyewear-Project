using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models
{
    public class Collection
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty; // URL-friendly: "fall-winter-2025"

        public string? Description { get; set; }

        public string? BannerImageUrl { get; set; } // Ảnh banner cho collection

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}