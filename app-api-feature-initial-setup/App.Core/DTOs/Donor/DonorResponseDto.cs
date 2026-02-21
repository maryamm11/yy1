namespace App.Core.DTOs.Donor
{
    public class DonorResponseDto
    {
        public Guid DonorId { get; set; }
        public string DonorName { get; set; }
        public string? DonorOrganizationImage { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
