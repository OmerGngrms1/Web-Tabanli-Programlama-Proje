using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymSystem.Data;
using GymSystem.Models;

namespace GymSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            var attendances = _context.Attendances.Include(a => a.Member).OrderByDescending(a => a.Date);
            return View(await attendances.ToListAsync());
        }

        // GET: Attendance/Create
        public IActionResult Create()
        {
            var members = _context.Members.Select(m => new { Id = m.Id, FullName = m.Name + " " + m.Surname }).ToList();
            ViewData["MemberId"] = new SelectList(members, "Id", "FullName");
            return View();
        }

        // POST: Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberId,Date,IsPresent")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var members = _context.Members.Select(m => new { Id = m.Id, FullName = m.Name + " " + m.Surname }).ToList();
            ViewData["MemberId"] = new SelectList(members, "Id", "FullName", attendance.MemberId);
            return View(attendance);
        }
    }
}
