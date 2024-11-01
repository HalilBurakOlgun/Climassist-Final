using Microsoft.AspNetCore.Mvc;
using Climassist.Models;
using System.Threading.Tasks;
using Climassist.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Climassist.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.UserType = "customer";
                user.Password = HashPassword(user.Password);

                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Geçersiz email veya şifre");
                return View(model);
            }

            try
            {
                var fullName = $"{user.Name?.Trim()} {user.SurName?.Trim()}".Trim();
                var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var lastName = nameParts.Length > 0 ? nameParts[nameParts.Length - 1] : string.Empty;
                var firstName = nameParts.Length > 1
                    ? string.Join(" ", nameParts.Take(nameParts.Length - 1))
                    : fullName;

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, firstName),
                new Claim(ClaimTypes.Surname, lastName),
                new Claim("FullName", fullName)
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                ModelState.AddModelError("", "Giriş işlemi sırasında bir hata oluştu.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            // Gerçek bir uygulama için güvenli bir şifreleme yöntemi kullanın
            return password;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Gerçek bir uygulama için güvenli bir doğrulama yöntemi kullanın
            return enteredPassword == storedPassword;
        }
    }
}