using System.ComponentModel.DataAnnotations;

namespace GymSystem.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre gereklidir.")]
        [Display(Name = "Mevcut Şifre")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        [MinLength(3, ErrorMessage = "Şifre en az 3 karakter olmalıdır.")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Şifre onayı gereklidir.")]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
