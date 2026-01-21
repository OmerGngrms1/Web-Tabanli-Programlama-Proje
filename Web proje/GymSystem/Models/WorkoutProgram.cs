using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class WorkoutProgram
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Program adı zorunludur.")]
        [Display(Name = "Program Adı")]
        public string Name { get; set; } = null!;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Program Detayları")]
        public string? Details { get; set; }



        public ICollection<Member> Members { get; set; } = new List<Member>();
    }
}
