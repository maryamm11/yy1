using System.ComponentModel.DataAnnotations;

namespace App.Core.DTOs.Donor
{
    public class UpdateDonorDto
    {
        [Required]
        [StringLength(200)]
        public string DonorName { get; set; }

        [StringLength(500)]
        public string? DonorOrganizationImage { get; set; }
    }
}
