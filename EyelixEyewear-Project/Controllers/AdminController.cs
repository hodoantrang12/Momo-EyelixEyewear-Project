using EyelixEyewear_Project.Data;
using EyelixEyewear_Project.Helpers;
using EyelixEyewear_Project.Models;
using EyelixEyewear_Project.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EyelixEyewear_Project.Controllers
{
    [Authorize(AuthenticationSchemes = "Admin", Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============
        // 0. LOGIN PAGE 
        // ==============
        [HttpGet]
        [AllowAnonymous] 
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra User trong DB thay vì Hardcode
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            // Logic check password (giống bên AccountController)
            if (user != null && PasswordHelper.VerifyPassword(password, user.PasswordHash))
            {
                if (user.Role != "Admin") // Quan trọng: Kiểm tra Role
                {
                    return Json(new { success = false, message = "Bạn không có quyền Admin" });
                }

                // Tạo Cookie (Giống hệt AccountController)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("FullName", user.FullName)
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Admin");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                     "Admin",
                     new ClaimsPrincipal(claimsIdentity),
                     authProperties);

                return Json(new { success = true, message = "Login successful" });
            }

            return Json(new { success = false, message = "Invalid credentials" });
        }

        // ========
        // LOGOUT 
        // ========
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Admin");
            return RedirectToAction("Login", "Admin");
        }

        // ==========================================
        // DASHBOARD - Get Top Products
        // ==========================================
        [HttpGet]
        public IActionResult GetTopProducts()
        {
            var topProducts = _context.OrderDetails
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new
                {
                    name = g.Key.Name,
                    sold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.sold)
                .Take(5)
                .ToList();

            return Json(topProducts);
        }

        // ==============================
        // INDEX - DASHBOARD
        // ===============================
        public IActionResult Index()
        {
            var totalOrders = _context.Orders.Count();
            var totalSales = _context.Orders.Sum(o => o.TotalAmount);
            var totalProducts = _context.Products.Count();
            var pendingOrders = _context.Orders.Count(o => o.Status == "Pending");

            var model = new AdminDashboardViewModel
            {
                TotalOrders = totalOrders,
                TotalSales = totalSales,
                TotalProducts = totalProducts,
                PendingOrders = pendingOrders
            };

            return View(model);
        }

        // ==============================
        // DASHBOARD - Get Recent Orders
        // ===============================
        [HttpGet]
        public IActionResult GetRecentOrders()
        {
            var recentOrders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new
                {
                    orderNumber = o.OrderNumber,
                    customerName = o.ShippingName,
                    orderDate = o.OrderDate,
                    status = o.Status,
                    totalAmount = o.TotalAmount
                })
                .ToList();

            return Json(recentOrders);
        }

        // ================
        // CUSTOMER DETAIL
        // ================
        [HttpGet]
        public async Task<IActionResult> GetCustomerDetail(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Customer not found" });
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == id)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    orderNumber = o.OrderNumber,
                    orderDate = o.OrderDate,
                    status = o.Status,
                    totalAmount = o.TotalAmount
                })
                .ToListAsync();

            var totalOrders = orders.Count;
            var totalSpent = orders.Sum(o => o.totalAmount);

            return Json(new
            {
                success = true,
                customer = new
                {
                    id = user.Id,
                    name = user.FullName,
                    email = user.Email,
                    phone = user.PhoneNumber,
                    address = user.Address,
                    totalOrders = totalOrders,
                    totalSpent = totalSpent,
                    orders = orders
                }
            });
        }

        // ===============
        // DELETE CUSTOMER 
        // ================
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return Json(new { success = false, message = "Customer not found" });
            }

            // Không cho xóa customer có orders
            if (user.Orders.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Cannot delete customer with existing orders. Please block instead."
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Customer deleted successfully" });
        }

        // ==========================================
        // 2. ORDERS MANAGEMENT
        // ==========================================
        public IActionResult Orders()
        {
            var orders = _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new AdminOrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.ShippingName,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod
                })
                .ToList();

            return View(orders);
        }

        // ==========================================
        // 3. ORDER DETAIL
        // ==========================================
        [HttpGet]
        public IActionResult OrderDetail(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = order.Id,
                orderNumber = order.OrderNumber,
                customer = order.ShippingName,
                date = order.OrderDate.ToString("yyyy-MM-dd"),
                address = $"{order.ShippingAddress}, {order.ShippingWard}, {order.ShippingCity}",
                payment = order.PaymentMethod,
                status = order.Status,
                total = order.TotalAmount,
                items = order.OrderDetails.Select(od => new
                {
                    name = od.Product.Name,
                    qty = od.Quantity,
                    price = od.Price,
                    total = od.Price * od.Quantity
                }).ToList()
            });
        }

        // ==========================================
        // 4. UPDATE ORDER STATUS
        // ==========================================
        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return Json(new { success = false, message = "Order not found" });
            }

            order.Status = status;
            _context.SaveChanges();

            return Json(new { success = true, message = "Order status updated successfully" });
        }

        // ==========================================
        // 5. PRODUCTS MANAGEMENT
        // ==========================================
        public IActionResult Products()
        {
            var products = _context.Products
                .Select(p => new AdminProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    SKU = p.SKU,
                    Price = p.Price,
                    Category = p.Category.Name,
                    Stock = p.Stock,
                    Status = p.IsActive ? "Active" : "Inactive",
                    ImageUrl = p.ImageUrl
                })
                .ToList();
            ViewBag.Categories = _context.Categories.Where(c => c.IsActive).ToList();
            ViewBag.Collections = _context.Collections.Where(c => c.IsActive).ToList();
            return View(products);
        }

        // ==========================================
        // 6. CUSTOMERS MANAGEMENT
        // ==========================================
        public IActionResult Customers()
        {
            var customers = _context.Users
                .Select(u => new AdminCustomerViewModel
                {
                    Id = u.Id,
                    Name = u.FullName,
                    Email = u.Email,
                    Location = u.Address,
                    TotalOrders = _context.Orders.Count(o => o.UserId == u.Id),
                    TotalSpent = _context.Orders.Where(o => o.UserId == u.Id).Sum(o => o.TotalAmount)
                })
                .ToList();

            return View(customers);
        }

        // ==========================================
        // 7. GET PRODUCT (For Edit Modal)
        // ==========================================
        [HttpGet]
        public IActionResult GetProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .AsNoTracking()
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Json(new
            {
                id = product.Id,
                name = product.Name,
                sku = product.SKU,
                description = product.Description,
                price = product.Price,
                discountPrice = product.DiscountPrice,
                stock = product.Stock,
                imageUrl = product.ImageUrl,
                frameMaterial = product.FrameMaterial,
                lensMaterial = product.LensMaterial,
                frameShape = product.FrameShape,
                color = product.Color,
                categoryId = product.CategoryId,
                collectionId = product.CollectionId,
                isActive = product.IsActive,
                galleryImages = product.ProductImages.Select(img => new {
                    id = img.Id,
                    url = img.ImageUrl
                }).ToList()
            });
        }


        // ================
        // 8. ADD PRODUCT 
        // ================
        [HttpPost]
        public IActionResult AddProduct([FromBody] ProductFormViewModel model)
        {
            try
            {
                // Check SKU duplicate
                if (_context.Products.Any(p => p.SKU == model.SKU))
                {
                    return Json(new { success = false, message = "SKU already exists" });
                }

                var product = new Product
                {
                    Name = model.Name,
                    SKU = model.SKU,
                    Description = model.Description,
                    Price = model.Price,
                    DiscountPrice = model.DiscountPrice,
                    Stock = model.Stock,
                    ImageUrl = model.ImageUrl,
                    FrameMaterial = model.FrameMaterial,
                    LensMaterial = model.LensMaterial,
                    FrameShape = model.FrameShape,
                    Color = model.Color,
                    CategoryId = model.CategoryId,
                    CollectionId = model.CollectionId,
                    IsActive = model.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Products.Add(product);
                _context.SaveChanges();

                // ✅ ADD GALLERY IMAGES
                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var imageUrl in model.GalleryImages)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = imageUrl
                        };
                        _context.ProductImages.Add(productImage);
                    }
                    _context.SaveChanges();
                }

                return Json(new { success = true, message = "Product added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        // ==================
        // 9. EDIT PRODUCT 
        // ==================
        [HttpPost]
        public IActionResult EditProduct([FromBody] ProductFormViewModel model)
        {
            try
            {
                _context.ChangeTracker.Clear();

                var product = _context.Products
                    .Include(p => p.ProductImages)
                    .FirstOrDefault(p => p.Id == model.Id);

                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Check SKU duplicate
                if (_context.Products.Any(p => p.SKU == model.SKU && p.Id != model.Id))
                {
                    return Json(new { success = false, message = "SKU already exists" });
                }

                // Update product info
                product.Name = model.Name;
                product.SKU = model.SKU;
                product.Description = model.Description;
                product.Price = model.Price;
                product.DiscountPrice = model.DiscountPrice;
                product.Stock = model.Stock;
                product.ImageUrl = model.ImageUrl;
                product.FrameMaterial = model.FrameMaterial;
                product.LensMaterial = model.LensMaterial;
                product.FrameShape = model.FrameShape;
                product.Color = model.Color;
                product.CategoryId = model.CategoryId;
                product.CollectionId = model.CollectionId;
                product.IsActive = model.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                _context.Entry(product).State = EntityState.Modified;

                // Remove old gallery images
                if (product.ProductImages.Any())
                {
                    _context.ProductImages.RemoveRange(product.ProductImages);
                }

                // Add new gallery images
                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var imageUrl in model.GalleryImages)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = imageUrl
                        };
                        _context.ProductImages.Add(productImage);
                    }
                }

                _context.SaveChanges();

                return Json(new { success = true, message = "Product updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


        // ==========================================
        // 11. TOGGLE PRODUCT ACTIVE/INACTIVE
        // ==========================================
        [HttpPost]
        public IActionResult ToggleProductStatus(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            product.IsActive = !product.IsActive;
            product.UpdatedAt = DateTime.UtcNow;
            _context.SaveChanges();

            string status = product.IsActive ? "activated" : "deactivated";
            return Json(new { success = true, message = $"Product {status} successfully", isActive = product.IsActive });
        }

        // ==========================================
        // 12. DELETE PRODUCT
        // ==========================================
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products
                .Include(p => p.CartItems)
                .Include(p => p.OrderDetails)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Check if product has orders
            if (product.OrderDetails.Any())
            {
                return Json(new { success = false, message = "Cannot delete product with existing orders. Please deactivate instead." });
            }

            // Remove from carts first
            if (product.CartItems.Any())
            {
                _context.CartItems.RemoveRange(product.CartItems);
            }

            _context.Products.Remove(product);
            _context.SaveChanges();

            return Json(new { success = true, message = "Product deleted successfully" });
        }

        // ==========================================
        // DELETE GALLERY IMAGE
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> DeleteGalleryImage(int imageId)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(imageId);
                if (image == null)
                {
                    return Json(new { success = false, message = "Image not found" });
                }

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Image deleted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ==========================================
        // 13. FILTER/SEARCH ORDERS
        // ==========================================
        [HttpGet]
        public IActionResult FilterOrders(string search, string status, string payment, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders.AsQueryable();

            // Search by Order Number or Customer Name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.OrderNumber.Contains(search) || o.ShippingName.Contains(search));
            }

            // Filter by Status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            // Filter by Payment Method
            if (!string.IsNullOrEmpty(payment) && payment != "All")
            {
                query = query.Where(o => o.PaymentMethod == payment);
            }

            // Filter by Date Range
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value.AddDays(1));
            }

            var orders = query.OrderByDescending(o => o.OrderDate)
                .Select(o => new AdminOrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    CustomerName = o.ShippingName,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalAmount = o.TotalAmount,
                    PaymentMethod = o.PaymentMethod
                })
                .ToList();

            return Json(orders);
        }

        // ==========================================
        // UPLOAD IMAGE
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> UploadProductImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "No file selected" });
                }

                // Kiểm tra loại file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return Json(new { success = false, message = "Only image files are allowed (jpg, png, gif, webp)" });
                }

                // Kiểm tra kích thước file (max 5MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File size must be less than 5MB" });
                }

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{extension}";

                // Đường dẫn lưu file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products");

                // Tạo folder nếu chưa có
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về URL
                var imageUrl = $"/images/products/{fileName}";

                return Json(new { success = true, imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error uploading file: " + ex.Message });
            }
        }
    }

}