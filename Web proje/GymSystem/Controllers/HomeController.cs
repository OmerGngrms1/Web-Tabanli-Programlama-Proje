using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GymSystem.Models;
using Microsoft.EntityFrameworkCore; // Added for CountAsync()

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GymSystem.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GymSystem.Data.ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, GymSystem.Data.ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Member"))
        {
            return RedirectToAction("MemberDashboard");
        }

        ViewBag.MemberCount = await _context.Members.CountAsync();
        ViewBag.ProgramCount = await _context.Programs.CountAsync();
        ViewBag.TodayAttendance = await _context.Attendances.CountAsync(a => a.Date.Date == DateTime.Today);
        return View();
    }

    [Authorize(Roles = "Member")]
    public async Task<IActionResult> MemberDashboard()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim == null) return RedirectToAction("Login", "Account");

        if (int.TryParse(userIdClaim.Value, out int memberId))
        {
            var member = await _context.Members
                .Include(m => m.Attendances)
                .Include(m => m.WorkoutProgram)
                .Include(m => m.DietPlan)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null) return RedirectToAction("Login", "Account");
            
            return View(member);
        }
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
