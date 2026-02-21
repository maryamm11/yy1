using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Charity
{
    public class UpdateCharityDto
    {
        [Required]
        [StringLength(200)]
        public string CharityName { get; set; }

        [StringLength(1000)]
        public string? CharityDescription { get; set; }
    }
}
