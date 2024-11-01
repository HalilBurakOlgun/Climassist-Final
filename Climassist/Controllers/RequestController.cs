using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Climassist.Models;
using Climassist.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Climassist.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Kullanıcı bilgilerini Session'dan al
            var userType = HttpContext.Session.GetString("UserType");
            var userEmail = HttpContext.Session.GetString("UserEmail");
            // Kullanıcı tipine göre talepleri getir
            if (userType == "customer")
            {
                // Müşteri ise sadece kendi taleplerini göster
                var requests = _context.Requests
                    .Where(r => r.Email == userEmail)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
                return View(requests);
            }
            else if (userType == "admin" || userType == "staff")
            {
                // Admin veya personel ise tüm talepleri göster
                var requests = _context.Requests
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();
                return View(requests);
            }
            // Yetkisiz erişim durumunda ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var userType = HttpContext.Session.GetString("UserType");
            // Sadece admin ve staff statü güncelleyebilir
            if (userType != "admin" && userType != "staff")
            {
                return Json(new { success = false, message = "Yetkisiz işlem!" });
            }

            var request = _context.Requests.Find(id);
            if (request != null)
            {
                request.Status = status;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Talep bulunamadı!" });
        }
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user != null)
            {
                ViewBag.UserName = user.Name;
                ViewBag.UserSurname = user.SurName;
                ViewBag.UserEmail = user.Email;
                ViewBag.UserPhone = user.Phone; // Telefon bilgisini ViewBag'e ekliyoruz

                // Debugging için
                Console.WriteLine($"Phone: {ViewBag.UserPhone}");
            }
            else
            {
                ViewBag.UserName = string.Empty;
                ViewBag.UserSurname = string.Empty;
                ViewBag.UserEmail = string.Empty;
                ViewBag.UserPhone = string.Empty;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Requests request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
                if (user != null)
                {
                    var fullName = $"{user.Name?.Trim()} {user.SurName?.Trim()}".Trim();
                    var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var lastName = nameParts.Length > 0 ? nameParts[nameParts.Length - 1] : string.Empty;
                    var firstName = nameParts.Length > 1
                        ? string.Join(" ", nameParts.Take(nameParts.Length - 1))
                        : fullName;

                    request.UserName = firstName;
                    request.UserSurname = lastName;
                    request.Email = user.Email?.Trim() ?? string.Empty;
                    request.Phone = user.Phone?.Trim() ?? string.Empty; // Phone ekledik
                }

                request.Status = "Beklemede";
                request.CreatedAt = DateTime.Now;

                _context.Requests.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRequest", "Request");
            }
            catch (Exception ex)
            {
                // Loglama yapılabilir
                ModelState.AddModelError("", "Talep oluşturulurken bir hata oluştu.");
                return View(request);
            }
        }
    }
}