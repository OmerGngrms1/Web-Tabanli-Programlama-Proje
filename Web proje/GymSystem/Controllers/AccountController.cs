using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GymSystem.Data;
using GymSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usernameOrEmail, string password)
        {
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Lütfen kullanıcı adı/e-posta ve şifre giriniz.";
                return View();
            }

            // TEMP: Seed default trainer if none exists
            if (!await _context.Trainers.AnyAsync())
            {
                _context.Trainers.Add(new Trainer { Username = "admin", Password = "123" });
                await _context.SaveChangesAsync();
            }

            // 1. Check Trainer
            var trainer = _context.Trainers.FirstOrDefault(t => t.Username == usernameOrEmail && t.Password == password);
            if (trainer != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, trainer.Username),
                    new Claim(ClaimTypes.Role, "Trainer"),
                    new Claim("UserId", trainer.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            // 2. Check Member
            // Allow login with Email or Phone for members? Let's assume Email or Name for now? 
            // The prompt said "Üye girişi", "Spor hocası girişi".
            // Let's check Email
            var member = _context.Members.FirstOrDefault(m => (m.Email == usernameOrEmail || m.Phone == usernameOrEmail) && m.Password == password);
            
            if (member != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, member.Name + " " + member.Surname),
                    new Claim(ClaimTypes.Role, "Member"),
                    new Claim("UserId", member.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Geçersiz kullanıcı adı veya şifre.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userIdClaim = User.FindFirst("UserId");
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null) return RedirectToAction("Login");
            
            int userId = int.Parse(userIdClaim.Value);

            if (roleClaim?.Value == "Member")
            {
                var member = await _context.Members.FindAsync(userId);
                if (member == null) return NotFound();

                if (member.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                    return View(model);
                }

                member.Password = model.NewPassword;
                _context.Update(member);
                await _context.SaveChangesAsync();
                
                ViewBag.Message = "Şifreniz başarıyla değiştirildi.";
                return View();
            }
            else if (roleClaim?.Value == "Trainer")
            {
                 var trainer = await _context.Trainers.FindAsync(userId);
                if (trainer == null) return NotFound();

                if (trainer.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError("CurrentPassword", "Mevcut şifre yanlış.");
                    return View(model);
                }

                trainer.Password = model.NewPassword;
                _context.Update(trainer);
                await _context.SaveChangesAsync();
                
                ViewBag.Message = "Şifreniz başarıyla değiştirildi.";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditProfile()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login");
            
            var member = await _context.Members.FindAsync(int.Parse(userIdClaim.Value));
            if (member == null) return NotFound();

            return View(member);
        }

        [HttpPost]
        [Authorize(Roles = "Member")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Member model)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null) return RedirectToAction("Login");
            
            var memberToUpdate = await _context.Members.FindAsync(int.Parse(userIdClaim.Value));
            if (memberToUpdate == null) return NotFound();

            if (model.Id != memberToUpdate.Id) return Unauthorized();

            // Check if weight changed and log it
            if (model.Weight != memberToUpdate.Weight)
            {
               if (model.Weight.HasValue)
               {
                   _context.MemberWeights.Add(new MemberWeight
                   {
                       MemberId = memberToUpdate.Id,
                       Weight = model.Weight.Value,
                       Date = DateTime.Now
                   });
               }
            }

            memberToUpdate.Name = model.Name;
            memberToUpdate.Surname = model.Surname;
            memberToUpdate.Email = model.Email;
            memberToUpdate.Phone = model.Phone;
            memberToUpdate.Age = model.Age;
            memberToUpdate.Weight = model.Weight;
            memberToUpdate.Height = model.Height;
            memberToUpdate.Gender = model.Gender;
            
            // Password update if provided optional removed for simplicity or create separate input in view? 
            // User said "password change place is unnecessary there", maybe keep it separate or include here.
            // Let's assume we keep it strictly profile details for now as requested "weight/age".

            _context.Update(memberToUpdate);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = "Bilgileriniz güncellendi.";
            return RedirectToAction("Index", "Home");
        }
    }
}
