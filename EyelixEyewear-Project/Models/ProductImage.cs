using System.Text.Json.Serialization;

namespace EyelixEyewear_Project.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        [JsonIgnore] 
        public Product Product { get; set; } = null!;
    }
}