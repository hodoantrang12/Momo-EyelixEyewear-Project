using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyelixEyewear_Project.Services
{
    public class PayPalService
    {
        private readonly IConfiguration _config;

        public PayPalService(IConfiguration config)
        {
            _config = config;
        }

        private PayPalHttpClient GetClient()
        {
            var clientId = _config["PayPal:ClientId"];
            var clientSecret = _config["PayPal:ClientSecret"];
            var mode = _config["PayPal:Mode"];

            PayPalEnvironment environment = mode == "live"
                ? (PayPalEnvironment)new LiveEnvironment(clientId, clientSecret)
                : new SandboxEnvironment(clientId, clientSecret);

            return new PayPalHttpClient(environment);
        }

        // Tạo order và trả về URL redirect sang PayPal
        public async Task<string> CreateOrder(decimal amount)
        {
            var client = GetClient();
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = amount.ToString("F2")
                        }
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "https://localhost:7xxx/Checkout/PayPalSuccess",
                    CancelUrl = "https://localhost:7xxx/Checkout/PayPalCancel"
                }
            });

            var response = await client.Execute(request);
            var order = response.Result<Order>();

            foreach (var link in order.Links)
                if (link.Rel == "approve")
                    return link.Href;

            return null;
        }

        // Capture tiền sau khi user approve
        public async Task<bool> CaptureOrder(string orderId)
        {
            var client = GetClient();
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await client.Execute(request);
            var result = response.Result<Order>();

            return result.Status == "COMPLETED";
        }
    }
}