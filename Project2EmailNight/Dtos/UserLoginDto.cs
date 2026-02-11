using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Dtos
{
	public class UserLoginDto
	{
		[Required(ErrorMessage = "Email gereklidir")]
		[EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
		[Display(Name = "Email Address")]
		public string Username { get; set; }  // Email olarak kullanılacak

		[Required(ErrorMessage = "Şifre gereklidir")]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

	}
}
