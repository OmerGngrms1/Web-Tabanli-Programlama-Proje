using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class DietPlan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Diyet planı adı zorunludur.")]
        [Display(Name = "Plan Adı")]
        public string Name { get; set; } = null!;

        [Display(Name = "Diyet Listesi")]
        public string? Details { get; set; } // Daily/Weekly breakdown

        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
