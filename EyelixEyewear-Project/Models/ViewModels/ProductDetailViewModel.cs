using EyelixEyewear_Project.Models;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; } = null!;
        public List<Product> RelatedProducts { get; set; } = new List<Product>();
    }
}