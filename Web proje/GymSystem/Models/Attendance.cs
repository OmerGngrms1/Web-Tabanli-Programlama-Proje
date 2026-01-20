using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;

        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Durum")]
        public bool IsPresent { get; set; } = true;
    }
}
