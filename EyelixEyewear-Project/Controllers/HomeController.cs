// Controllers/HomeController.cs
using EyelixEyewear_Project.Data;
using EyelixEyewear_Project.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EyelixEyewear.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                // Lấy 6 sản phẩm mới nhất
                NewArrivals = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(6)
                    .Select(p => new ProductCardViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        DiscountPrice = p.DiscountPrice,
                        ImageUrl = p.ImageUrl,
                        IsNew = true,
                        IsSale = p.DiscountPrice.HasValue
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        //  GET: Home/Collection 
        public async Task<IActionResult> Collection(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // Tìm collection theo slug
            var collection = await _context.Collections
                .Include(c => c.Products)
                    .ThenInclude(p => p.Category)
                .Include(c => c.Products)
                    .ThenInclude(p => p.Reviews)
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductVariants)
                .FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

            if (collection == null)
            {
                return NotFound();
            }

            var activeProducts = collection.Products.Where(p => p.IsActive).ToList();

            ViewBag.CollectionName = collection.Name;
            ViewBag.CollectionDescription = collection.Description;
            ViewBag.CollectionBanner = collection.BannerImageUrl;

            return View(activeProducts);
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Terms & Privacy
        public IActionResult Terms()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Contact
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Home/Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Xử lý gửi email hoặc lưu vào database
                // Ví dụ: await _emailService.SendContactEmailAsync(model);

                TempData["Message"] = "Thanks for contacting us! We will respond as soon as possible.";
                return RedirectToAction(nameof(Contact));
            }

            return View(model);
        }

        // POST: Home/Subscribe (Newsletter)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(NewsletterSubscribeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Lưu email vào database newsletter
                TempData["Message"] = "Subscribtion Completed! Thank you for your support.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/FAQ
        public IActionResult FAQ()
        {
            return View();
        }

        // GET: Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}