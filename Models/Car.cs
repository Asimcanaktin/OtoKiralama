using System.ComponentModel.DataAnnotations;

namespace OtoKiralama.Models
{
    public class Car : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required]
        [StringLength(50)]
        public string CarModel { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(20)]
        public string PlateNumber { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}