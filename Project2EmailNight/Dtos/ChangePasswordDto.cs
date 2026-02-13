using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Dtos
{
	public class ChangePasswordDto
	{
		[Required(ErrorMessage = "Mevcut şifre gerekli")]
		public string CurrentPassword { get; set; }

		[Required(ErrorMessage = "Yeni şifre gerekli")]
		[MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalı")]
		public string NewPassword { get; set; }

		[Required(ErrorMessage = "Şifre tekrarı gerekli")]
		[Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
		public string ConfirmPassword { get; set; }
	}
}
