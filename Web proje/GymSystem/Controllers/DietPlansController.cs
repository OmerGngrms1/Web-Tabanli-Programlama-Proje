using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymSystem.Data;
using GymSystem.Models;

namespace GymSystem.Controllers
{
    [Authorize]
    public class DietPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DietPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.DietPlans.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dietPlan = await _context.DietPlans
                .Include(d => d.Members)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dietPlan == null) return NotFound();

            // Security check: If user is a member, ensure they are assigned this plan
            if (User.IsInRole("Member"))
            {
                var userIdClaim = User.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int memberId))
                {
                    var member = await _context.Members.FindAsync(memberId);
                    if (member != null && member.DietPlanId != id)
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                }
            }
            
            return View(dietPlan);
        }

        [Authorize(Roles = "Trainer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Create([Bind("Id,Name,Details")] DietPlan dietPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dietPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dietPlan);
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dietPlan = await _context.DietPlans.FindAsync(id);
            if (dietPlan == null) return NotFound();
            return View(dietPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Details")] DietPlan dietPlan)
        {
            if (id != dietPlan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dietPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietPlanExists(dietPlan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dietPlan);
        }

        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dietPlan = await _context.DietPlans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dietPlan == null) return NotFound();

            return View(dietPlan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Trainer")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dietPlan = await _context.DietPlans.FindAsync(id);
            if (dietPlan != null)
            {
                _context.DietPlans.Remove(dietPlan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DietPlanExists(int id)
        {
            return _context.DietPlans.Any(e => e.Id == id);
        }
    }
}
