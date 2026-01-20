using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymSystem.Data;
using GymSystem.Models;

namespace GymSystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var members = await _context.Members.Include(m => m.WorkoutProgram).ToListAsync();
            return View(members);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            ViewData["WorkoutProgramId"] = new SelectList(_context.Programs, "Id", "Name");
            ViewData["DietPlanId"] = new SelectList(_context.DietPlans, "Id", "Name");
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,Phone,WorkoutProgramId,DietPlanId,Password")] Member member)
        {
            member.RegistrationDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkoutProgramId"] = new SelectList(_context.Programs, "Id", "Name", member.WorkoutProgramId);
            ViewData["DietPlanId"] = new SelectList(_context.DietPlans, "Id", "Name", member.DietPlanId);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            ViewData["WorkoutProgramId"] = new SelectList(_context.Programs, "Id", "Name", member.WorkoutProgramId);
            ViewData["DietPlanId"] = new SelectList(_context.DietPlans, "Id", "Name", member.DietPlanId);
            return View(member);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,RegistrationDate,Email,Phone,WorkoutProgramId,DietPlanId")] Member member, string? NewPassword)
        {
            if (id != member.Id) return NotFound();

            // Remove Password from ModelState because we are handling it manually/optionally
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                try
                {
                    var memberToUpdate = await _context.Members.FindAsync(id);
                    if (memberToUpdate == null) return NotFound();

                    memberToUpdate.Name = member.Name;
                    memberToUpdate.Surname = member.Surname;
                    memberToUpdate.Email = member.Email;
                    memberToUpdate.Phone = member.Phone;
                    memberToUpdate.WorkoutProgramId = member.WorkoutProgramId;
                    memberToUpdate.DietPlanId = member.DietPlanId;
                    
                    if (!string.IsNullOrEmpty(NewPassword))
                    {
                        memberToUpdate.Password = NewPassword;
                    }

                    _context.Update(memberToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["WorkoutProgramId"] = new SelectList(_context.Programs, "Id", "Name", member.WorkoutProgramId);
            ViewData["DietPlanId"] = new SelectList(_context.DietPlans, "Id", "Name", member.DietPlanId);
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Members
                .Include(m => m.WorkoutProgram)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null) return NotFound();

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}
