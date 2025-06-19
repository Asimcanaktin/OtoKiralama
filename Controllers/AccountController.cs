using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OtoKiralama.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OtoKiralama.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IHttpClientFactory httpClientFactory, UserManager<ApplicationUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var client = _httpClientFactory.CreateClient();
            var loginData = new { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/auth/login", content); // Portunu kendi projenin portuna göre ayarla
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                ViewBag.Token = responseBody;
                return View("LoginSuccess");
            }
            else
            {
                ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Şifreler uyuşmuyor.";
                return View();
            }
            var userExists = await _userManager.FindByNameAsync(username);
            if (userExists != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                return View();
            }
            ApplicationUser user = new ApplicationUser()
            {
                UserName = username,
                Email = email,
                SecurityStamp = System.Guid.NewGuid().ToString()
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                ViewBag.Error = "Kullanıcı oluşturulamadı.";
                return View();
            }
            ViewBag.Success = "Kullanıcı başarıyla oluşturuldu.";
            return View();
        }
    }
}