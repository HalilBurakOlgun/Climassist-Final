using Microsoft.AspNetCore.Mvc;
using Climassist.Models;
using System.Threading.Tasks;
using Climassist.Data;

namespace Climassist.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context; // DbContext'inizi buraya ekleyin

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
                // Kullanıcı tipini varsayılan olarak "customer" olarak ayarlayın
                user.UserType = "customer";

                // Şifreyi hashleme
                user.Password = HashPassword(user.Password); // Burayı güvenli bir hashleme algoritmasıyla değiştirin.

                try
                {
                    // Veritabanına kaydet
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home"); // Kayıt sonrası yönlendirme
                }
                catch (Exception ex)
                {
                    // Hata mesajını göster
                    ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                }
            }

            return View(user);
        }


        private string HashPassword(string password)
        {
            // Şifreleme işlemi için bir algoritma kullanın (örneğin, BCrypt)
            // Burada sadece bir örnek
            return password; // Bu, gerçek bir hashleme algoritması ile değiştirilmelidir.
        }
    }
}
