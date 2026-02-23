using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Offer
{
    public class UpdateOfferDto
    {
        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? ProductImage { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
    }
}
