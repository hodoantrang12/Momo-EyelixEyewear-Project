using EyelixEyewear_Project.Data;
using EyelixEyewear_Project.Models;
using EyelixEyewear_Project.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyelixEyewear_Project.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 9; // 9 products per page

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================================================
        // PRODUCT INDEX BY CATEGORY WITH PAGINATION
        // ===================================================
        public async Task<IActionResult> Index(string category, int page = 1)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.ProductVariants)
                .Where(p => p.IsActive);

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Name.ToLower() == category.ToLower());
            }

            // Count total products
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            // Apply pagination
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentCategory = category;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;

            return View(products);
        }

        // ===================================================
        // COLLECTION BY SLUG WITH PAGINATION
        // ===================================================
        public async Task<IActionResult> Collection(string slug, int page = 1)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // Find collection
            var collection = await _context.Collections
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (collection == null)
            {
                return NotFound();
            }

            // Query products in this collection
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.ProductVariants)
                .Where(p => p.CollectionId == collection.Id && p.IsActive);

            // Count total
            var totalProducts = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

            // Apply pagination
            var products = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Pass data to view
            ViewBag.CollectionName = collection.Name;
            ViewBag.CollectionDescription = collection.Description;
            ViewBag.CollectionBanner = collection.BannerImageUrl ?? "/images/banner/default-banner.jpg";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;

            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Collection)
                .Include(p => p.ProductVariants)
                .Include(p => p.ProductImages)
                .Include(p => p.Reviews.Where(r => r.IsApproved))
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products
            var relatedProducts = await _context.Products
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id && p.IsActive)
                .ToListAsync();

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }

        // ==========================================
        // ADD REVIEW
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { success = false, message = "Please login to submit a review" });
                }

                var username = User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

                if (existingReview != null)
                {
                    return Json(new { success = false, message = "You have already reviewed this product" });
                }

                if (rating < 1 || rating > 5)
                {
                    return Json(new { success = false, message = "Rating must be between 1 and 5" });
                }

                if (string.IsNullOrWhiteSpace(comment) || comment.Length < 10)
                {
                    return Json(new { success = false, message = "Review must be at least 10 characters" });
                }

                var review = new Review
                {
                    ProductId = productId,
                    UserId = user.Id,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow,
                    IsApproved = true
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Review submitted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ==========================================
        // CHECK IF USER HAS REVIEWED
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> HasReviewed(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { hasReviewed = false });
            }

            var username = User.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return Json(new { hasReviewed = false });
            }

            var hasReviewed = await _context.Reviews
                .AnyAsync(r => r.ProductId == productId && r.UserId == user.Id);

            return Json(new { hasReviewed = hasReviewed });
        }

        // ==========================================
        // SEARCH PRODUCTS
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Json(new { success = false, products = new List<object>() });
            }

            var products = await _context.Products
                .Where(p => p.IsActive && p.Name.Contains(query))
                .Take(8)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    discountPrice = p.DiscountPrice,
                    imageUrl = p.ImageUrl ?? "/images/default-product.jpg"
                })
                .ToListAsync();

            return Json(new { success = true, products = products });
        }
    }
}