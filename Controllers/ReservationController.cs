using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OtoKiralama.Models;
using System.Linq;

namespace OtoKiralama.Controllers
{
    public class ReservationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Tüm rezervasyonları listele
        public IActionResult Index()
        {
            var reservations = _context.Reservations
                .Select(r => new
                {
                    r.Id,
                    r.Car.Brand,
                    r.Car.CarModel,
                    r.StartDate,
                    r.EndDate,
                    UserName = r.User.UserName
                }).ToList();

            return View(reservations);
        }

        // Yeni rezervasyon ekle (GET)
        public IActionResult Create()
        {
            ViewBag.Cars = _context.Cars.Where(c => c.IsAvailable).ToList();
            return View();
        }

        // Yeni rezervasyon ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                reservation.UserId = _userManager.GetUserId(User);

                var car = _context.Cars.Find(reservation.CarId);
                if (car != null)
                {
                    car.IsAvailable = false;
                }

                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Cars = _context.Cars.Where(c => c.IsAvailable).ToList();
            return View(reservation);
        }
    }
}