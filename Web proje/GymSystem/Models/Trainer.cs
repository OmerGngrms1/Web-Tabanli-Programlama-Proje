using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class Trainer
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = null!;
    }
}
