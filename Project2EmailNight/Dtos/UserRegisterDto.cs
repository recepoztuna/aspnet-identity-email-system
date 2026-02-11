using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Dtos
{
	public class UserRegisterDto
	{
		
		
			[Required(ErrorMessage = "Ad gereklidir")]
			[StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir")]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[Required(ErrorMessage = "Soyad gereklidir")]
			[StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir")]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[Required(ErrorMessage = "Email gereklidir")]
			[EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
			[Display(Name = "Email Address")]
			public string Email { get; set; }

			[Required(ErrorMessage = "Şifre gereklidir")]
			[StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[Required(ErrorMessage = "Şifre tekrarı gereklidir")]
			[DataType(DataType.Password)]
			[Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
			[Display(Name = "Confirm Password")]
			public string ConfirmPassword { get; set; }

		// ✅ BASİT CHECKBOX
		    [Range(typeof(bool), "true", "true", ErrorMessage = "Kullanım şartlarını kabul etmelisiniz")]
		    public bool AcceptTerms { get; set; }


	}
}
