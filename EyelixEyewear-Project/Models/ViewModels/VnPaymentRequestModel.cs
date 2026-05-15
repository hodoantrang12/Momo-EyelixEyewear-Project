namespace EyelixEyewear_Project.Models.ViewModels
{
    public class VnPaymentRequestModel
    {
        public string OrderId { get; set; }
        public string OrderDescription { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
