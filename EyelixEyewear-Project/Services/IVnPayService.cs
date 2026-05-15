using EyelixEyewear_Project.Models.ViewModels;

namespace EyelixEyewear_Project.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(VnPaymentRequestModel model, HttpContext context);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
