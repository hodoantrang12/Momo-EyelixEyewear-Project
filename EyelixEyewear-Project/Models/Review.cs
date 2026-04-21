using EyelixEyewear_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models
{
    public class Review
    {
        public int Id { get; set; }

        // FK
        public int ProductId { get; set; }
        public int UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsApproved { get; set; } = false;

        // Navigation
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}