using GymSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GymSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Member> Members { get; set; }
        public DbSet<WorkoutProgram> Programs { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<DietPlan> DietPlans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Relationships
            modelBuilder.Entity<Member>()
                .HasOne(m => m.WorkoutProgram)
                .WithMany(p => p.Members)
                .HasForeignKey(m => m.WorkoutProgramId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Member>()
                .HasOne(m => m.DietPlan)
                .WithMany(p => p.Members)
                .HasForeignKey(m => m.DietPlanId)
                .OnDelete(DeleteBehavior.SetNull);

             modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Member)
                .WithMany(m => m.Attendances)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
