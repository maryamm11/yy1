using App.Core.Enums;

namespace App.Core.DTOs.Need
{
    public class NeedResponseDto
    {
        public Guid CharityNeedId { get; set; }
        public Guid CharityId { get; set; }
        public string CharityName { get; set; }
        public Guid? AdminId { get; set; }
        public string Category { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public Priority Priority { get; set; }
        public NeedStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
