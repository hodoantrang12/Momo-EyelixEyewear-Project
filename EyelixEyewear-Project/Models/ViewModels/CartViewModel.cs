using System.Collections.Generic;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class CartViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public string SelectedShippingMethod { get; set; } = "standard";
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}