namespace EyelixEyewear_Project.Models.ViewModels
{
    public class ProductCategoryViewModel
    {
        public Category Category { get; set; } = new Category();
        public IEnumerable<Product> Product { get; set; } = Enumerable.Empty<Product>();
        public int TotalProducts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
