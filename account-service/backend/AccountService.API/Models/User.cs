using System.ComponentModel.DataAnnotations;

namespace AccountService.API.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string FullName => $"{FirstName} {LastName}";
    }

    public enum UserType
    {
        Seller = 1,
        Buyer = 2,
        Carrier = 3,
        Agent = 4
    }

    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Deleted = 4
    }
}