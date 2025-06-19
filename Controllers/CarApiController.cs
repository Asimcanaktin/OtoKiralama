using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OtoKiralama.Models;
namespace OtoKiralama.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT ile koruma
    public class CarApiController : ControllerBase
    {
        private readonly AppDbContext context;
        public CarApiController(AppDbContext context)
        {
            context = context;
        }
        // Tüm araçları listele
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = context.Cars.ToList();
            return Ok(cars);
        }
        // Belirli bir aracı getir
        [HttpGet("{id}")]
        public IActionResult GetCar(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();
            return Ok(car);
        }
        // Yeni araç ekle
        [HttpPost]
        public IActionResult AddCar([FromBody] Car car)
        {
            context.Cars.Add(car);
            context.SaveChanges();
            return Ok(car);
        }
        // Araç güncelle
        [HttpPut("{id}")]
        public IActionResult UpdateCar(int id, [FromBody] Car updatedCar)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();
            car.Brand = updatedCar.Brand;
            car.CarModel = updatedCar.CarModel; // <-- DÜZELTİLDİ
            car.Year = updatedCar.Year;
            car.PlateNumber = updatedCar.PlateNumber;
            car.IsAvailable = updatedCar.IsAvailable;
            car.UpdatedAt = DateTime.Now;
            context.SaveChanges();
            return Ok(car);
        }
        // Araç sil
        [HttpDelete("{id}")]
        public IActionResult DeleteCar(int id)
        {
            var car = context.Cars.Find(id);
            if (car == null)
                return NotFound();
            context.Cars.Remove(car);
            context.SaveChanges();
            return Ok("Araç silindi.");
        }
    }
}