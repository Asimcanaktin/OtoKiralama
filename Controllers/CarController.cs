using Microsoft.AspNetCore.Mvc;
using OtoKiralama.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
namespace OtoKiralama.Controllers
{
    public class CarController : Controller
    {
        private readonly AppDbContext context;
        private readonly ILogger<CarController> logger;
        public CarController(AppDbContext context, ILogger<CarController> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        // Filtreli araç listesi
        public IActionResult Index(string brand, string model, int? year, bool? isAvailable)
        {
            var cars = context.Cars.AsQueryable();
            if (!string.IsNullOrEmpty(brand))
                cars = cars.Where(c => c.Brand.Contains(brand));
            if (!string.IsNullOrEmpty(model))
                cars = cars.Where(c => c.CarModel.Contains(model));
            if (year.HasValue)
                cars = cars.Where(c => c.Year == year.Value);
            if (isAvailable.HasValue)
                cars = cars.Where(c => c.IsAvailable == isAvailable.Value);
            var carList = cars.ToList();
            logger.LogInformation("Araç listesi filtrelendi. Toplam: {Count}", carList.Count);
            return View(carList);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            logger.LogInformation("Araç ekleme sayfası açıldı.");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Car car, IFormFile? photo)
        {
            if (ModelState.IsValid)
            {
                if (photo != null && photo.Length > 0)
                {
                    var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(imagesPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }
                    car.PhotoPath = "/images/" + fileName;
                }
                context.Cars.Add(car);
                context.SaveChanges();
                logger.LogInformation("Yeni araç eklendi: {Brand} {Model} ({Plate})", car.Brand, car.CarModel, car.PlateNumber);
                return RedirectToAction(nameof(Index));
            }
            logger.LogWarning("Araç eklenemedi. ModelState geçersiz.");
            return View(car);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
            {
                logger.LogWarning("Düzenlenmek istenen araç bulunamadı. Id: {Id}", id);
                return NotFound();
            }
            logger.LogInformation("Araç düzenleme sayfası açıldı. Id: {Id}", id);
            return View(car);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Car updatedCar, IFormFile? photo)
        {
            if (id != updatedCar.Id)
            {
                logger.LogWarning("Düzenleme için gönderilen id ile model id uyuşmuyor. Id: {Id}", id);
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var car = context.Cars.Find(id);
                if (car == null)
                {
                    logger.LogWarning("Düzenlenmek istenen araç bulunamadı. Id: {Id}", id);
                    return NotFound();
                }
                car.Brand = updatedCar.Brand;
                car.CarModel = updatedCar.CarModel;
                car.Year = updatedCar.Year;
                car.PlateNumber = updatedCar.PlateNumber;
                car.IsAvailable = updatedCar.IsAvailable;
                car.UpdatedAt = System.DateTime.Now;
                if (photo != null && photo.Length > 0)
                {
                    var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    if (!Directory.Exists(imagesPath))
                        Directory.CreateDirectory(imagesPath);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var filePath = Path.Combine(imagesPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(stream);
                    }
                    car.PhotoPath = "/images/" + fileName;
                }
                context.SaveChanges();
                logger.LogInformation("Araç güncellendi. Id: {Id}, Marka: {Brand}, Model: {Model}", car.Id, car.Brand, car.CarModel);
                return RedirectToAction(nameof(Index));
            }
            logger.LogWarning("Araç güncellenemedi. ModelState geçersiz. Id: {Id}", id);
            return View(updatedCar);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
            {
                logger.LogWarning("Silinmek istenen araç bulunamadı. Id: {Id}", id);
                return NotFound();
            }
            logger.LogInformation("Araç silme sayfası açıldı. Id: {Id}", id);
            return View(car);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
            {
                logger.LogWarning("Silinmek istenen araç bulunamadı. Id: {Id}", id);
                return NotFound();
            }
            context.Cars.Remove(car);
            context.SaveChanges();
            logger.LogInformation("Araç silindi. Id: {Id}, Marka: {Brand}, Model: {Model}", car.Id, car.Brand, car.CarModel);
            return RedirectToAction(nameof(Index));
        }
    }
}