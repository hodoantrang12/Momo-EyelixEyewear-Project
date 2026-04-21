using EyelixEyewear_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace EyelixEyewear_Project.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

        [MaxLength(20)]
        public string Role { get; set; } = "Customer";
        // Customer / Admin

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        // MFA Fields
        public string? TotpSecretKey { get; set; }   // Secret key được mã hóa base32
        public bool IsMfaEnabled { get; set; } = false; // User đã bật MFA chưa
    }
}

