using System;
using System.ComponentModel.DataAnnotations;

namespace OtoKiralama.Models
{
    public class Reservation : BaseEntity
    {
        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}