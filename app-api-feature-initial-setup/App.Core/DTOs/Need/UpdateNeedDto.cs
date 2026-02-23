using System.ComponentModel.DataAnnotations;
using App.Core.Enums;

namespace App.Core.DTOs.Need
{
    public class UpdateNeedDto
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

        public Priority Priority { get; set; }
    }
}
