using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OtoKiralama.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OtoKiralama.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı giriş yaptı: {Username}", username);
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı!";
            _logger.LogWarning("Başarısız giriş denemesi: {Username}", username);
            return View();
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
                _logger.LogWarning("Kayıt başarısız: Şifreler uyuşmuyor. Kullanıcı adı: {Username}", username);
                return View();
            }

            var userExists = await _userManager.FindByNameAsync(username);
            if (userExists != null)
            {
                ViewBag.Error = "Bu kullanıcı adı zaten alınmış.";
                _logger.LogWarning("Kayıt başarısız: Kullanıcı adı zaten alınmış. Kullanıcı adı: {Username}", username);
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
                _logger.LogError("Kayıt başarısız: Kullanıcı oluşturulamadı. Kullanıcı adı: {Username}", username);
                return View();
            }

            ViewBag.Success = "Kullanıcı başarıyla oluşturuldu.";
            _logger.LogInformation("Yeni kullanıcı kaydı başarılı: {Username}", username);
            return View();
        }
    }
}
