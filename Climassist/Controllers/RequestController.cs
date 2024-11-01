using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Climassist.Models;
using Climassist.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Climassist.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var userRequests = await _context.Requests
                .Where(r => r.Email == user.Email)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(userRequests);
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            List<Requests> requests;
            if (user.UserType == "customer")
            {
                requests = await _context.Requests
                    .Where(r => r.Email == user.Email)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            else if (user.UserType == "admin" || user.UserType == "staff")
            {
                requests = await _context.Requests
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            return View(requests);
        }

        [HttpPost]
        [Authorize(Roles = "admin,staff")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return Json(new { success = false, message = "Talep bulunamadı!" });
            }

            request.Status = status;
            await _context.SaveChangesAsync();
            return Json(new { success = true });
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
                ViewBag.UserPhone = user.Phone;
            }
            else
            {
                return NotFound("Kullanıcı bulunamadı.");
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

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            request.UserName = user.Name;
            request.UserSurname = user.SurName;
            request.Email = user.Email;
            request.Phone = user.Phone;
            request.Status = "Beklemede";
            request.CreatedAt = DateTime.Now;

            try
            {
                _context.Requests.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
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