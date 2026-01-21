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
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
            {
                var member = await _context.Members
                    .Include(m => m.WorkoutProgram)
                    .Include(m => m.DietPlan)
                    .FirstOrDefaultAsync(m => m.Id == memberId);
                
                if (member != null)
                {
                    ViewBag.Member = member; // Pass the member object to ViewBag
                    
                    // Fetch attendance history for the calendar/list
                    ViewBag.AttendanceHistory = await _context.Attendances
                        .Where(a => a.MemberId == memberId)
                        .OrderByDescending(a => a.Date)
                        .ToListAsync();
                    
                    // Check if attendance is marked for today
                    var today = DateTime.Today;
                    ViewBag.IsAttendanceMarked = await _context.Attendances
                        .AnyAsync(a => a.MemberId == memberId && a.Date.Date == today);

                    // Fetch weight history
                    ViewBag.WeightHistory = await _context.MemberWeights
                        .Where(w => w.MemberId == memberId)
                        .OrderBy(w => w.Date)
                        .ToListAsync();
                }
            }
            // We return the same View(), but the view will handle the display based on Role/ViewBag
            return View();
        }

        // Admin/Trainer Dashboard Data
        ViewBag.MemberCount = await _context.Members.CountAsync();
        ViewBag.ProgramCount = await _context.Programs.CountAsync();
        ViewBag.DietPlanCount = await _context.DietPlans.CountAsync();

        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Member")]
    public async Task<IActionResult> MarkAttendance()
    {
        var userIdClaim = User.FindFirst("UserId");
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
        {
            var today = DateTime.Today;
            bool alreadyMarked = await _context.Attendances.AnyAsync(a => a.MemberId == memberId && a.Date.Date == today);

            if (!alreadyMarked)
            {
                var attendance = new Attendance
                {
                    MemberId = memberId,
                    Date = DateTime.Now,
                    IsPresent = true
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction(nameof(Index));
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
