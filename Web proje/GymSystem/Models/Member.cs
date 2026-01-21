using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [Display(Name = "Ad")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [Display(Name = "Soyad")]
        public string Surname { get; set; } = null!;

        [Required(ErrorMessage = "Cinsiyet seçimi zorunludur.")]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = null!;

        [Display(Name = "Kayıt Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = false)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }


        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }

        [Display(Name = "Yaş")]
        public int? Age { get; set; }

        [Display(Name = "Kilo (kg)")]
        public double? Weight { get; set; }

        [Display(Name = "Boy (cm)")]
        public double? Height { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "123456"; // Default password for existing members

        [Required(ErrorMessage = "Paket süresi seçimi zorunludur.")]
        [Display(Name = "Paket Süresi (Ay)")]
        public int DurationInMonths { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = false)]
        public DateTime EndDate { get; set; }

        public int RemainingDays
        {
            get
            {
                var remaining = (EndDate - DateTime.Now).Days;
                return remaining > 0 ? remaining : 0;
            }
        }

        public string MembershipStatus
        {
            get
            {
                if (DateTime.Now > EndDate) return "Süresi Dolmuş";
                if (RemainingDays <= 7) return "Süre Azalıyor"; // Yakında bitiyor uyarısı için
                return "Aktif";
            }
        }

        [Display(Name = "Antrenman Programı")]
        public int? WorkoutProgramId { get; set; }
        public WorkoutProgram? WorkoutProgram { get; set; }

        [Display(Name = "Diyet Planı")]
        public int? DietPlanId { get; set; }
        public DietPlan? DietPlan { get; set; }

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
