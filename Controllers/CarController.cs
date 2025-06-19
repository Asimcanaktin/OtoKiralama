using Microsoft.AspNetCore.Mvc;
using OtoKiralama.Models;
using System.Linq;

namespace OtoKiralama.Controllers
{
    public class CarController : Controller
    {
        private readonly AppDbContext context;

        public CarController(AppDbContext context)
        {
            this.context = context;
        }

        // Araçları listele
        public IActionResult Index()
        {
            var cars = context.Cars.ToList();
            return View(cars);
        }

        // Yeni araç ekle (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Yeni araç ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Car car)
        {
            if (ModelState.IsValid)
            {
                context.Cars.Add(car);
                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // Araç düzenle (GET)
        public IActionResult Edit(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();
            return View(car);
        }

        // Araç düzenle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Car updatedCar)
        {
            if (id != updatedCar.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var car = context.Cars.Find(id);
                if (car == null)
                    return NotFound();

                car.Brand = updatedCar.Brand;
                car.CarModel = updatedCar.CarModel; 
                car.Year = updatedCar.Year;
                car.PlateNumber = updatedCar.PlateNumber;
                car.IsAvailable = updatedCar.IsAvailable;
                car.UpdatedAt = DateTime.Now;

                context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(updatedCar);
        }

        // Araç sil (GET)
        public IActionResult Delete(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();
            return View(car);
        }

        // Araç sil (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();

            context.Cars.Remove(car);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}