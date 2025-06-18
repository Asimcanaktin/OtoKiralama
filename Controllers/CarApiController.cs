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
        private readonly AppDbContext _context;

        public CarApiController(AppDbContext context)
        {
            _context = context;
        }

        // Tüm araçları listele
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = _context.Cars.ToList();
            return Ok(cars);
        }

        // Belirli bir aracı getir
        [HttpGet("{id}")]
        public IActionResult GetCar(int id)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();
            return Ok(car);
        }

        // Yeni araç ekle
        [HttpPost]
        public IActionResult AddCar([FromBody] Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
            return Ok(car);
        }

        // Araç güncelle
        [HttpPut("{id}")]
        public IActionResult UpdateCar(int id, [FromBody] Car updatedCar)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();

            car.Brand = updatedCar.Brand;
            car.Model = updatedCar.Model;
            car.Year = updatedCar.Year;
            car.PlateNumber = updatedCar.PlateNumber;
            car.IsAvailable = updatedCar.IsAvailable;
            car.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok(car);
        }

        // Araç sil
        [HttpDelete("{id}")]
        public IActionResult DeleteCar(int id)
        {
            var car = _context.Cars.Find(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            _context.SaveChanges();
            return Ok("Araç silindi.");
        }
    }
}