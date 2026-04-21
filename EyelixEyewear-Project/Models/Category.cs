using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Property
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
