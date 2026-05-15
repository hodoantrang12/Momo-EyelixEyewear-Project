namespace EyelixEyewear_Project.Models.ViewModels
{
    public class VnPaymentResponseModel
    {
        public string OrderDescription { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public bool Success { get; set; }
        public string VnPayResponseCode { get; set; }
    }
}
