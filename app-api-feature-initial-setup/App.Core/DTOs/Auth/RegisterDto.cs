using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [StringLength(20)]
        public string? Whatsapp { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? Governorate { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        public string UserType { get; set; } // "Charity" or "Donor"

        [StringLength(200)]
        public string? OrganizationName { get; set; }

        [StringLength(1000)]
        public string? OrganizationDescription { get; set; }
    }
}
