using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymSystem.Data;
using GymSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace GymSystem.Controllers
{
    [Authorize]
    public class ProgramsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Programs
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Programs.ToListAsync());
        }

        // GET: Programs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs
                .Include(p => p.Members)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (program == null) return NotFound();
            
            // Security check: If user is a member, ensure they are assigned this program
            if (User.IsInRole("Member"))
            {
               var userIdClaim = User.FindFirst("UserId");
               if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
               {
                    var member = await _context.Members.FindAsync(memberId);
                    if (member != null && member.WorkoutProgramId != id)
                    {
                        return RedirectToAction("AccessDenied", "Account"); // Or simply NotFound/Unauthorized
                    }
               }
            }

            return View(program);
        }

        // GET: Programs/Create
        [Authorize(Roles = "Trainer")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Programs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Details")] WorkoutProgram program)
        {
            if (ModelState.IsValid)
            {
                _context.Add(program);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(program);
        }

        // GET: Programs/Edit/5
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs.FindAsync(id);
            if (program == null) return NotFound();
            return View(program);
        }

        // POST: Programs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Details")] WorkoutProgram program)
        {
            if (id != program.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(program);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProgramExists(program.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(program);
        }

        // GET: Programs/Delete/5
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var program = await _context.Programs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (program == null) return NotFound();

            return View(program);
        }

        // POST: Programs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var program = await _context.Programs.FindAsync(id);
            if (program != null)
            {
                _context.Programs.Remove(program);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProgramExists(int id)
        {
            return _context.Programs.Any(e => e.Id == id);
        }
    }
}
