namespace App.Core.DTOs.Charity
{
    public class CharityResponseDto
    {
        public Guid CharityId { get; set; }
        public string CharityName { get; set; }
        public string CharityDescription { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
    }
}
