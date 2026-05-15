using System.ComponentModel.DataAnnotations.Schema;

namespace EyelixEyewear_Project.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string TransactionId { get; set; }      // Mã giao dịch từ cổng trả về
        public string PaymentMethod { get; set; }       // "VNPay", "MoMo", "PayPal", "COD"
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
        public string Status { get; set; }              // "Success", "Failed", "Pending"
        public string GatewayResponse { get; set; }     // Raw response từ cổng (JSON/query string)
        public string IPAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Order Order { get; set; }
    }
}
