using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models.ViewModels
{
    // Dashboard ViewModel
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalProducts { get; set; }
        public int PendingOrders { get; set; }
    }

    // Order ViewModel
    public class AdminOrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
    }

    // Product ViewModel
    public class AdminProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
    }

    // Customer ViewModel
    public class AdminCustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    // Product Form ViewModel (Cho Add/Edit)
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(200)]
        public string Name { get; set; } 

        [Required(ErrorMessage = "SKU is required")]
        public string SKU { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public decimal? DiscountPrice { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
        public string FrameMaterial { get; set; }
        public string LensMaterial { get; set; }
        public string FrameShape { get; set; }
        public string Color { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }
        public int? CollectionId { get; set; }  

        public bool IsActive { get; set; } = true;
        public List<string> GalleryImages { get; set; } = new List<string>();


        // For dropdown
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}