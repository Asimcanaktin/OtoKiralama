using System;
using System.ComponentModel.DataAnnotations;

namespace OtoKiralama.Models
{
    public class Reservation : BaseEntity
    {
        [Required(ErrorMessage = "Araç seçimi zorunludur.")]
        public int CarId { get; set; }
        public Car Car { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi zorunludur.")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi zorunludur.")]
        public DateTime EndDate { get; set; }
    }
}