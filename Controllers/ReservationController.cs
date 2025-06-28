using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OtoKiralama.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
namespace OtoKiralama.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<ReservationController> logger;
        public ReservationController(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<ReservationController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            var reservations = context.Reservations
            .Select(r => new
            {
                r.Id,
                r.Car.Brand,
                r.Car.CarModel,
                r.StartDate,
                r.EndDate,
                UserName = r.User.UserName
            }).ToList();
            logger.LogInformation("Rezervasyon listesi görüntülendi. Toplam: {Count}", reservations.Count);
            return View(reservations);
        }
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Cars = context.Cars.Where(c => c.IsAvailable).ToList();
            logger.LogInformation("Rezervasyon oluşturma sayfası açıldı.");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.UserId = userManager.GetUserId(User);
                var car = context.Cars.Find(reservation.CarId);
                if (car != null)
                {
                    car.IsAvailable = false;
                }
                context.Reservations.Add(reservation);
                context.SaveChanges();
                logger.LogInformation("Yeni rezervasyon eklendi. Kullanıcı: {UserId}, Araç: {CarId}, Başlangıç: {Start}, Bitiş: {End}", reservation.UserId, reservation.CarId, reservation.StartDate, reservation.EndDate);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cars = context.Cars.Where(c => c.IsAvailable).ToList();
            logger.LogWarning("Rezervasyon eklenemedi. ModelState geçersiz.");
            return View(reservation);
        }
    }
}