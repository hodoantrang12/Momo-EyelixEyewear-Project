using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EyelixEyewear_Project.Data;
using Microsoft.AspNetCore.Authorization;
using AppUser = EyelixEyewear_Project.Models.User;
using EyelixEyewear_Project.Models.ViewModels;
using EyelixEyewear_Project.Helpers;

namespace EyelixEyewear_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // SIGNUP
        [HttpGet]
        public IActionResult Signup()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Username == model.Username || u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Username or Email already exists.");
                    return View(model);
                }

                // tạo users
                var newUser = new AppUser
                {
                    FullName = model.FullName,
                    Username = model.Username,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    Role = "Customer",

                    // Gọi PasswordHelper bạn đã có
                    PasswordHash = PasswordHelper.HashPassword(model.Password),

                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Successful! Please log in.";
                return RedirectToAction("Login");
            }
            return View(model);
        }

        // login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (user != null && PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                {
                    // === THÊM MỚI: Kiểm tra MFA ===
                    if (user.IsMfaEnabled)
                    {
                        // Lưu userId vào Session tạm thời (chưa đăng nhập thật)
                        HttpContext.Session.SetInt32("MfaPendingUserId", user.Id);
                        HttpContext.Session.SetString("MfaRememberMe", model.RememberMe.ToString());

                        // Chuyển sang trang nhập OTP
                        return RedirectToAction("MfaVerify");
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim("FullName", user.FullName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "Customer");
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                    };

                    await HttpContext.SignInAsync(
                        "Customer",
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Incorrect username or password.");
            }
            return View(model);
        }

        // logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Customer");
            return RedirectToAction("Login");
        }

        // PROFILE DASHBOARD 
        [Authorize(AuthenticationSchemes = "Customer")] 
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login");

            int userId = int.Parse(userIdStr);

            //Lấy thông tin User + Đơn hàng từ DB
            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");

            // Đổ dữ liệu vào ViewModel
            var model = new UserProfileViewModel
            {
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Phone = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                Orders = user.Orders.OrderByDescending(o => o.OrderDate).ToList(),
                IsMfaEnabled = user.IsMfaEnabled
            };

            return View(model);
        }

        [Authorize(AuthenticationSchemes = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return RedirectToAction("Login");

            // Load lại danh sách đơn hàng để hiển thị nếu có lỗi 
            model.Orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            if (ModelState.IsValid)
            {
                //update thông tin cá nhân
                user.FullName = model.FullName;
                user.PhoneNumber = model.Phone;
                user.Address = model.Address;

                // change password 
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    // check previous password
                    if (!PasswordHelper.VerifyPassword(model.CurrentPassword, user.PasswordHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                        TempData["ErrorMessage"] = "Update failed: Incorrect old password.";
                        return View("Profile", model);
                    }
                    user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Information updated successfully!";
                return RedirectToAction("Profile");
            }

            TempData["ErrorMessage"] = "Please check the information you have entered.";
            return View("Profile", model);
        }

        // forgot password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("", "Please enter your email address.");
                return View();
            }

            //tìm=email trong db
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email address.");
                return View();
            }

            // Generate password
            var tempPassword = GenerateRandomPassword(8);

            // Hash và lưu vào DB
            user.PasswordHash = PasswordHelper.HashPassword(tempPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.TempPassword = tempPassword;
            ViewBag.UserEmail = email;
            ViewBag.Success = true;

            return View("ForgotPasswordSuccess");
        }
        // Helper method để generate random password
        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Order Detail
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrderDetail(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Lấy order với đầy đủ thông tin
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                return NotFound(new { success = false, message = "Order not found" });
            }

            // Chuẩn bị dữ liệu trả về
            var orderDetail = new
            {
                success = true,
                order = new
                {
                    orderNumber = order.OrderNumber,
                    orderDate = order.OrderDate.ToString("dd/MM/yyyy HH:mm"),
                    status = order.Status,
                    paymentMethod = order.PaymentMethod,
                    note = order.Note ?? "",

                    // Thông tin giao hàng
                    shippingName = order.ShippingName,
                    shippingPhone = order.ShippingPhone,
                    shippingAddress = order.ShippingAddress,
                    shippingWard = order.ShippingWard,
                    shippingDistrict = order.ShippingDistrict,
                    shippingCity = order.ShippingCity,

                    // Chi tiết sản phẩm
                    items = order.OrderDetails.Select(od => new
                    {
                        productName = od.Product.Name,
                        productImage = od.Product.ImageUrl ?? "/images/default-product.jpg",
                        quantity = od.Quantity,
                        price = od.Price,
                        subtotal = od.Quantity * od.Price
                    }).ToList(),

                    // Tổng tiền
                    subtotal = order.OrderDetails.Sum(od => od.Quantity * od.Price),
                    shippingFee = order.ShippingFee,
                    discount = order.Discount,
                    totalAmount = order.TotalAmount
                }
            };

            return Json(orderDetail);
        }

        // ==========================================
        // MFA VERIFY — Bước 2 sau khi login
        // ==========================================
        [HttpGet]
        public IActionResult MfaVerify()
        {
            // Kiểm tra có pending MFA session không
            var pendingUserId = HttpContext.Session.GetInt32("MfaPendingUserId");
            if (pendingUserId == null)
                return RedirectToAction("Login");

            return View(new MfaVerifyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> MfaVerify(MfaVerifyViewModel model)
        {
            var pendingUserId = HttpContext.Session.GetInt32("MfaPendingUserId");
            if (pendingUserId == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(pendingUserId.Value);
            if (user == null)
                return RedirectToAction("Login");

            // Xác thực OTP
            bool isValid = TotpHelper.VerifyOtp(user.TotpSecretKey!, model.OtpCode);

            if (!isValid)
            {
                ModelState.AddModelError("OtpCode", "Mã OTP không đúng hoặc đã hết hạn. Vui lòng thử lại.");
                return View(model);
            }

            // OTP đúng → đăng nhập thật sự
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FullName", user.FullName)
    };

            bool rememberMe = HttpContext.Session.GetString("MfaRememberMe") == "True";

            var claimsIdentity = new ClaimsIdentity(claims, "Customer");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = DateTime.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync("Customer", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Xóa session tạm
            HttpContext.Session.Remove("MfaPendingUserId");
            HttpContext.Session.Remove("MfaRememberMe");

            return RedirectToAction("Index", "Home");
        }

        // ==========================================
        // MFA SETUP — Bật MFA trong trang Profile
        // ==========================================
        [Authorize(AuthenticationSchemes = "Customer")]
        [HttpGet]
        public async Task<IActionResult> MfaSetup()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            // Nếu đã bật rồi thì redirect về Profile
            if (user.IsMfaEnabled)
            {
                TempData["SuccessMessage"] = "MFA đã được bật cho tài khoản của bạn.";
                return RedirectToAction("Profile");
            }

            // Tạo secret key mới (chưa lưu DB, chờ user xác nhận)
            var secretKey = TotpHelper.GenerateSecretKey();
            var qrUri = TotpHelper.GenerateQrCodeUri(secretKey, user.Email);
            var qrBase64 = QrCodeHelper.GenerateQrCodeBase64(qrUri);

            // Lưu secret key tạm vào Session (chưa lưu DB)
            HttpContext.Session.SetString("MfaPendingSecret", secretKey);

            var viewModel = new MfaSetupViewModel
            {
                SecretKey = secretKey,
                QrCodeBase64 = qrBase64,
                QrCodeUri = qrUri
            };

            return View(viewModel);
        }

        [Authorize(AuthenticationSchemes = "Customer")]
        [HttpPost]
        public async Task<IActionResult> MfaSetup(MfaSetupViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            // Lấy secret key từ Session
            var secretKey = HttpContext.Session.GetString("MfaPendingSecret");
            if (string.IsNullOrEmpty(secretKey))
            {
                TempData["ErrorMessage"] = "Phiên cài đặt MFA đã hết hạn. Vui lòng thử lại.";
                return RedirectToAction("MfaSetup");
            }

            // Xác thực OTP với secret key tạm
            bool isValid = TotpHelper.VerifyOtp(secretKey, model.OtpCode);

            if (!isValid)
            {
                // Tạo lại QR để hiển thị lại
                var qrUri = TotpHelper.GenerateQrCodeUri(secretKey, user.Email);
                model.SecretKey = secretKey;
                model.QrCodeBase64 = QrCodeHelper.GenerateQrCodeBase64(qrUri);
                model.QrCodeUri = qrUri;

                ModelState.AddModelError("OtpCode", "Mã OTP không đúng. Hãy đảm bảo bạn đã quét đúng QR code và nhập mã mới nhất.");
                return View(model);
            }

            // Lưu secret key vào DB và bật MFA
            user.TotpSecretKey = secretKey;
            user.IsMfaEnabled = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("MfaPendingSecret");

            TempData["SuccessMessage"] = " Xác thực 2 yếu tố (MFA) đã được bật thành công!";
            return RedirectToAction("Profile");
        }

        // ==========================================
        // MFA DISABLE — Tắt MFA
        // ==========================================
        [Authorize(AuthenticationSchemes = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MfaDisable()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            user.IsMfaEnabled = false;
            user.TotpSecretKey = null;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "MFA đã được tắt.";
            return RedirectToAction("Profile");
        }
    }
}