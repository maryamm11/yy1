using App.Core.Enums;

namespace App.Core.DTOs.Offer
{
    public class OfferResponseDto
    {
        public Guid OfferId { get; set; }
        public Guid DonorOrganizationId { get; set; }
        public string DonorName { get; set; }
        public Guid? AdminId { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string? ProductImage { get; set; }
        public DateTime ExpiryDate { get; set; }
        public OfferStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
