// ViewModels/HomeViewModel.cs
using System;
using System.Collections.Generic;

namespace EyelixEyewear_Project.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<ProductCardViewModel> NewArrivals { get; set; }
        public List<ProductCardViewModel> BestSellers { get; set; }

        public HomeViewModel()
        {
            NewArrivals = new List<ProductCardViewModel>();
            BestSellers = new List<ProductCardViewModel>();
        }
    }

    public class ProductCardViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsNew { get; set; }
        public bool IsSale { get; set; }
        public int SoldCount { get; set; }
    }

    // ViewModel cho Contact Form
    public class ContactFormViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // ViewModel cho Newsletter Subscribe
    public class NewsletterSubscribeViewModel
    {
        public string Email { get; set; } = string.Empty;
    }
}