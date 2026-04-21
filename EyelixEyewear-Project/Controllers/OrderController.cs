using EyelixEyewear_Project.Data;
using EyelixEyewear_Project.Helpers;
using EyelixEyewear_Project.Models;
using EyelixEyewear_Project.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EyelixEyewear_Project.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public OrderController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- HELPER: Lấy User ID ---
        private int GetCurrentUserId()
        {
            // Nếu đã đăng nhập thì lấy ID thật
            var username = User.Identity?.Name;
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null) return user.Id;

            // Nếu đang test chưa login, trả về ID = 2 (User Customer test của bạn)
            return 2;
        }

        // ==========================================
        // 1. CHỨC NĂNG: THÊM VÀO GIỎ (ADD TO CART)
        // ==========================================
        [HttpPost] // Chỉ nhận POST để bảo mật
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            // Kiểm tra đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please login to add items to cart";
                return RedirectToAction("Login", "Account");
            }
            int userId = GetCurrentUserId();

            // 1. Kiểm tra xem User đã có Giỏ hàng chưa?
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId);

            // Nếu chưa có, tạo giỏ mới
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
                _context.SaveChanges(); // Lưu để lấy CartId
            }

            // 2. Kiểm tra xem Sản phẩm này đã có trong giỏ chưa?
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                // Nếu có rồi -> Cộng dồn số lượng
                cartItem.Quantity += quantity;
                _context.CartItems.Update(cartItem);
            }
            else
            {
                // Nếu chưa có -> Tạo dòng mới
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            _context.SaveChanges();

            // 1. Tạo tin nhắn thông báo (sẽ hiện ở View)
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng!";

            // 2. Quay lại đúng trang vừa bấm (thay vì nhảy sang trang Cart)
            string referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(referer);
        }

        // Action dành riêng cho AJAX (Không load trang)
        [HttpPost]
        public IActionResult AddToCartAjax(int productId, int quantity = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { success = false, requireLogin = true, message = "Please login to add items to cart" });
            }

            try
            {
                int userId = GetCurrentUserId();

                // 1. Kiểm tra/Tạo Giỏ hàng
                var cart = _context.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow };
                    _context.Carts.Add(cart);
                    _context.SaveChanges();
                }

                // 2. Thêm/Cập nhật sản phẩm
                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    _context.CartItems.Update(cartItem);
                }
                else
                {
                    cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };
                    _context.CartItems.Add(cartItem);
                }

                _context.SaveChanges();

                // 3. Tính tổng số lượng để update icon giỏ hàng (nếu có)
                int totalItems = cart.CartItems.Sum(x => x.Quantity);

                // Trả về JSON báo thành công
                return Json(new { success = true, message = "Added to cart!", totalItems = totalItems });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
        // ==========================================
        // XEM GIỎ HÀNG
        // ==========================================
        public IActionResult Cart()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please login to view your cart";
                return RedirectToAction("Login", "Account");
            }

            int userId = GetCurrentUserId();

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) 
                .FirstOrDefault(c => c.UserId == userId);

            var viewModel = new CartViewModel();

            if (cart != null && cart.CartItems.Any())
            {
                viewModel.CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.ImageUrl,
                    Price = ci.Product.DiscountPrice ?? ci.Product.Price,
                    Quantity = ci.Quantity,
                    Color = ci.Product.Color
                }).ToList();

                // Tính toán tổng tiền
                viewModel.Subtotal = viewModel.CartItems.Sum(x => x.Price * x.Quantity);
                viewModel.Total = viewModel.Subtotal;
            }

            return View(viewModel);
        }

        // ==========================================
        // XÓA SP KHỎI GIỎ
        // ==========================================
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                int userId = GetCurrentUserId();
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart != null)
                {
                    var itemToRemove = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                    if (itemToRemove != null)
                    {
                        _context.CartItems.Remove(itemToRemove);
                        _context.SaveChanges();
                        return Json(new { success = true, message = "Item removed successfully" });
                    }
                }
                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // ==========================================
        // CẬP NHẬT SỐ LƯỢNG
        // ==========================================
        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            try
            {
                int userId = GetCurrentUserId();
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart != null)
                {
                    var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                    if (item != null)
                    {
                        if (quantity > 0)
                        {
                            item.Quantity = quantity;
                            _context.CartItems.Update(item);
                        }
                        else
                        {
                            _context.CartItems.Remove(item);
                        }
                        _context.SaveChanges();
                        return Json(new { success = true, message = "Cart updated successfully" });
                    }
                }
                return Json(new { success = false, message = "Item not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ==========================================
        // 5. TRANG CHECKOUT 
        // ==========================================
        [HttpGet]
        public IActionResult Checkout()
        {
            int userId = GetCurrentUserId();
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == userId);

            // Nếu giỏ trống -> Về trang chủ
            if (cart == null || !cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Users.Find(userId);

            var model = new CheckoutViewModel
            {
                FullName = user?.FullName,
                Email = user?.Email,
                Phone = user?.PhoneNumber,
                Address = user?.Address,

                // Load items
                CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.DiscountPrice ?? ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList()
            };

            // Tính tiền
            decimal subtotal = model.CartItems.Sum(x => x.Price * x.Quantity);
            model.Subtotal = subtotal;
            model.Total = subtotal + 1000; // Mặc định + Ship 20k
            model.EstimatedDelivery = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");

            return View(model);
        }

        // ==========================================
        // 6. XỬ LÝ ĐẶT HÀNG (PLACE ORDER)
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            // DEBUG: Kiểm tra user có đăng nhập không
            if (!User.Identity.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Please login to place order";
                return RedirectToAction("Login", "Account");
            }

            int userId = GetCurrentUserId();
            var cart = _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                                     .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty";
                return RedirectToAction("Cart", "Order");
            }

            // DEBUG: In ra tất cả lỗi validation
            if (!ModelState.IsValid)
            {
                // Log lỗi ra Console/Debug
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine("ModelState Error: " + error.ErrorMessage);
                }

                // Hiển thị lỗi cho user
                TempData["ErrorMessage"] = "Please fill in all required fields correctly";

                // Load lại dữ liệu để hiển thị form
                model.CartItems = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Price = ci.Product.DiscountPrice ?? ci.Product.Price,
                    Quantity = ci.Quantity
                }).ToList();
                model.Subtotal = model.CartItems.Sum(x => x.Price * x.Quantity);
                model.Total = model.Subtotal + (model.ShippingMethod == "instant" ? 50000 : 1000);
                model.EstimatedDelivery = DateTime.Now.AddDays(3).ToString("dd/MM/yyyy");

                return View("Checkout", model);
            }

            // Nếu ModelState.IsValid = true, tiếp tục xử lý
            if (ModelState.IsValid)
            {
                // Tạo đơn hàng (Order)
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,
                    OrderNumber = "ORD-" + DateTime.Now.Ticks.ToString().Substring(10),
                    Status = "Pending",
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = "Unpaid",
                    Note = model.OrderNotes,
                    ShippingName = model.DifferentShippingAddress ? model.RecipientFullName : model.FullName,
                    ShippingPhone = model.DifferentShippingAddress ? model.RecipientPhone : model.Phone,
                    ShippingAddress = model.DifferentShippingAddress ? model.ShippingAddress : model.Address,
                    ShippingCity = model.DifferentShippingAddress ? model.ShippingProvince : model.Province,
                    ShippingWard = model.DifferentShippingAddress ? model.ShippingWard : model.Ward,
                    ShippingFee = model.ShippingMethod == "instant" ? 50000 : 1000,
                    TotalAmount = 0
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Chuyển từ Giỏ sang Chi tiết đơn (OrderDetail)
                decimal itemsTotal = 0;
                foreach (var item in cart.CartItems)
                {
                    decimal price = item.Product.DiscountPrice ?? item.Product.Price;
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = price
                    };
                    _context.OrderDetails.Add(orderDetail);
                    itemsTotal += (price * item.Quantity);
                }

                // Cập nhật tổng tiền & Xóa giỏ hàng
                order.TotalAmount = itemsTotal + order.ShippingFee;
                _context.Orders.Update(order);
                _context.CartItems.RemoveRange(cart.CartItems);
                _context.SaveChanges();

                if (model.PaymentMethod == "momo")
                {
                    return RedirectToAction("CreateMoMoPayment", new { orderId = order.Id });
                }
                return RedirectToAction("OrderConfirmation", new { id = order.Id });
            }

            // Nếu lỗi form -> Load lại dữ liệu checkout
            model.CartItems = cart.CartItems.Select(ci => new CartItemViewModel
            {
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Price = ci.Product.DiscountPrice ?? ci.Product.Price,
                Quantity = ci.Quantity
            }).ToList();
            model.Subtotal = model.CartItems.Sum(x => x.Price * x.Quantity);
            model.Total = model.Subtotal + (model.ShippingMethod == "instant" ? 50000 : 1000);

            return View("Checkout", model);
        }

        [HttpGet]
        public IActionResult GetCartCount()
        {
            try
            {
                int userId = GetCurrentUserId();
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefault(c => c.UserId == userId);

                int count = cart?.CartItems.Sum(x => x.Quantity) ?? 0;
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        [HttpPost]
        public IActionResult UpdateShipping(string shippingMethod)
        {
            try
            {
                int userId = GetCurrentUserId();
                var cart = _context.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == userId);

                if (cart != null)
                {
                    decimal subtotal = cart.CartItems.Sum(x =>
                        (x.Product.DiscountPrice ?? x.Product.Price) * x.Quantity);

                    decimal shippingFee = shippingMethod == "instant" ? 50000 : 1000;
                    decimal total = subtotal + shippingFee;

                    // Lưu shipping method vào session
                    HttpContext.Session.SetString("ShippingMethod", shippingMethod);

                    return Json(new { success = true, total = total, shippingFee = shippingFee });
                }

                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 7. TRANG CẢM ƠN
        public IActionResult OrderConfirmation(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            return View(order);
        }

        // ==========================================
        // BƯỚC 1: TẠO YÊU CẦU THANH TOÁN MOMO
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> CreateMoMoPayment(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null) return NotFound();

            var momoConfig = _configuration.GetSection("MoMo");
            string partnerCode = momoConfig["PartnerCode"]!;
            string accessKey = momoConfig["AccessKey"]!;
            string secretKey = momoConfig["SecretKey"]!;
            string endpoint = momoConfig["Endpoint"]!;
            string returnUrl = momoConfig["ReturnUrl"]!;
            string notifyUrl = momoConfig["NotifyUrl"]!;

            string requestId = Guid.NewGuid().ToString("N")[..20];
            string momoOrderId = $"EYX{order.Id}{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            long amount = (long)order.TotalAmount;
            string orderInfo = $"Thanh toan don hang #{order.OrderNumber}";
            string extraData = "";
            string requestType = "captureWallet";

            // Signature — sort a-z theo docs MoMo
            string rawSignature =
                $"accessKey={accessKey}" +
                $"&amount={amount}" +
                $"&extraData={extraData}" +
                $"&ipnUrl={notifyUrl}" +
                $"&orderId={momoOrderId}" +
                $"&orderInfo={orderInfo}" +
                $"&partnerCode={partnerCode}" +
                $"&redirectUrl={returnUrl}" +
                $"&requestId={requestId}" +
                $"&requestType={requestType}";

            string signature = MoMoHelper.ComputeHmacSha256(rawSignature, secretKey);

            var requestBody = new
            {
                partnerCode,
                storeName = "Eyelix Eyewear",
                storeId = partnerCode,
                requestId,
                amount,
                orderId = momoOrderId,
                orderInfo,
                redirectUrl = returnUrl,
                ipnUrl = notifyUrl,
                lang = "vi",
                extraData,
                requestType,
                signature
            };

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json");

            try
            {
                var response = await httpClient.PostAsync(endpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var responseData = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(responseString);

                if (responseData != null &&
                    responseData.TryGetValue("resultCode", out var rc) && rc.GetInt32() == 0 &&
                    responseData.TryGetValue("payUrl", out var payUrl) &&
                    !string.IsNullOrEmpty(payUrl.GetString()))
                {
                    HttpContext.Session.SetString("MoMoOrderId", momoOrderId);
                    HttpContext.Session.SetInt32("PendingOrderId", orderId);
                    return Redirect(payUrl.GetString()!);
                }

                var errMsg = responseData?.GetValueOrDefault("message").GetString() ?? "Lỗi không xác định";
                TempData["ErrorMessage"] = $"MoMo lỗi: {errMsg}";
            }
            catch (TaskCanceledException)
            {
                TempData["ErrorMessage"] = "Kết nối MoMo timeout. Vui lòng thử lại.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction("Cart");
        }

        // ==========================================
        // BƯỚC 2: NHẬN KẾT QUẢ SAU THANH TOÁN (Return URL)
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> MoMoReturn()
        {
            var resultCode = Request.Query["resultCode"].ToString();
            var pendingOrderId = HttpContext.Session.GetInt32("PendingOrderId");

            if (resultCode == "0")
            {
                if (pendingOrderId.HasValue)
                {
                    var order = await _context.Orders.FindAsync(pendingOrderId.Value);
                    if (order != null)
                    {
                        order.PaymentStatus = "Paid";
                        order.Status = "Processing";
                        _context.Orders.Update(order);
                        await _context.SaveChangesAsync();
                    }
                }
                TempData["SuccessMessage"] = "Thanh toán MoMo thành công!";
                return RedirectToAction("OrderConfirmation", new { id = pendingOrderId ?? 0 });
            }

            var message = Request.Query["message"].ToString();
            TempData["ErrorMessage"] = $"Thanh toán thất bại: {message}";
            return RedirectToAction("Cart");
        }

        // ==========================================
        // BƯỚC 3: IPN — MoMo gọi ngầm xác nhận server-to-server
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> MoMoNotify()
        {
            using var reader = new System.IO.StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var data = System.Text.Json.JsonSerializer.Deserialize<MoMoViewModel>(
                body,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (data == null) return Ok();

            var momoConfig = _configuration.GetSection("MoMo");
            string secretKey = momoConfig["SecretKey"]!;
            string accessKey = momoConfig["AccessKey"]!;

            // Verify signature IPN — sort a-z theo docs MoMo
            string rawSignature =
                $"accessKey={accessKey}" +
                $"&amount={data.Amount}" +
                $"&extraData={data.ExtraData}" +
                $"&message={data.Message}" +
                $"&orderId={data.OrderId}" +
                $"&orderInfo={data.OrderInfo}" +
                $"&orderType={data.OrderType}" +
                $"&partnerCode={data.PartnerCode}" +
                $"&payType={data.PayType}" +
                $"&requestId={data.RequestId}" +
                $"&responseTime={data.ResponseTime}" +
                $"&resultCode={data.ResultCode}" +
                $"&transId={data.TransId}";

            string expectedSig = MoMoHelper.ComputeHmacSha256(rawSignature, secretKey);

            if (data.Signature != expectedSig) return Ok(); // Chữ ký sai, bỏ qua

            if (data.ResultCode == 0)
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => data.OrderId.Contains(o.Id.ToString()));

                if (order != null && order.PaymentStatus != "Paid")
                {
                    order.PaymentStatus = "Paid";
                    order.Status = "Processing";
                    _context.Orders.Update(order);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }
    }
}