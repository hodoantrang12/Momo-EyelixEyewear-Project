using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }

        [Range(1, 999)]
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        // Navigation Properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
