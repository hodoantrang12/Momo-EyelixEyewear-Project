using EyelixEyewear_Project.Models;
using System.Collections.Generic;

namespace EyelixEyewear.Models.ViewModels
{
    // ViewModel cho Product/Index
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        // Filter options
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 12;
    }

    // ViewModel cho Product/Category
    public class ProductCategoryViewModel
    {
        public Category Category { get; set; }
        public IEnumerable<Product> Products { get; set; }

        // Selected filters
        public IEnumerable<int> SelectedBrands { get; set; }
        public IEnumerable<string> SelectedMaterials { get; set; }
        public IEnumerable<string> SelectedShapes { get; set; }
        public string SelectedPriceRange { get; set; }

        // Sort & Pagination
        public string CurrentSort { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
        public int PageSize { get; set; } = 12;
    }

    // ViewModel cho Cart Item (display)
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }

    // ViewModel cho Checkout
    public class CheckoutViewModel
    {
        public IEnumerable<CartItemViewModel> CartItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total => Subtotal + ShippingFee;

        // Billing Info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }

        // Payment
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
    }
}