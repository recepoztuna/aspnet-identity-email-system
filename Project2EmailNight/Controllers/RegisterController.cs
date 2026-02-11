using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;
using static Project2EmailNight.Dtos.UserRegisterDto;
using Project2EmailNight.Services;

namespace Project2EmailNight.Controllers
{
	public class RegisterController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IEmailService _emailService;  // ← YENİ EKLE

		public RegisterController(UserManager<AppUser> userManager, IEmailService emailService)
		{
			_userManager = userManager;
			_emailService = emailService;
		}

		[HttpGet]
		public IActionResult CreateUser()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateUser(UserRegisterDto model)
		{
			if (!ModelState.IsValid)
			{
				return View("CreateUser", model);
			}

			// 6 haneli kod
			Random random = new Random();
			string confirmCode = random.Next(100000, 999999).ToString();

			var user = new AppUser
			{
				UserName = model.Email,
				Email = model.Email,
				FirstName = model.FirstName,      // ← YENİ
				LastName = model.LastName,        // ← YENİ
				EmailConfirmed = false,
				EmailConfirmationCode = confirmCode,
				CodeExpireTime = DateTime.Now.AddMinutes(15)
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				 
				await _emailService.SendConfirmationCodeAsync(user.Email, confirmCode);

				TempData["Email"] = user.Email;
				//TempData["SuccessMessage"] = "Kayıt başarılı! Email adresinize doğrulama kodu gönderildi.";

				return RedirectToAction("ConfirmEmail", "Register");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View("CreateUser", model);
		}
		// GET: Email Doğrulama Sayfası
		[HttpGet]
		public IActionResult ConfirmEmail()
		{  // TempData.Peek() = Oku ama silme! (keep kullan)
			var email = TempData.Peek("Email")?.ToString();

			if (string.IsNullOrEmpty(email))
			{
				// Email yoksa register sayfasına yönlendir
				return RedirectToAction("CreateUser");
			}

			// Email'i ViewBag'e at
			ViewBag.Email = email;

			return View();
		}
		// POST: Email Doğrulama (Kod Kontrolü)
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ConfirmEmail(string email, string code)
		{
			// 1. Email ve kod boş mu kontrol et
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
			{
				ModelState.AddModelError("", "Email ve kod gereklidir");
				ViewBag.Email = email;
				return View();
			}

			// 2. Kullanıcıyı bul
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				ModelState.AddModelError("", "Kullanıcı bulunamadı");
				return View();
			}

			// 3. Kod doğru mu?
			if (user.EmailConfirmationCode != code)
			{
				ModelState.AddModelError("", "Doğrulama kodu hatalı");
				ViewBag.Email = email;
				return View();
			}

			// 4. Kod süresi dolmuş mu?
			if (user.CodeExpireTime < DateTime.Now)
			{
				ModelState.AddModelError("", "Doğrulama kodunun süresi dolmuş. Lütfen yeniden kayıt olun.");
				return View();
			}

			// 5. ✅ HER ŞEY DOĞRU - Email'i onayla
			user.EmailConfirmed = true;
			user.EmailConfirmationCode = null;  // Kodu sil (bir daha kullanılmasın)
			user.CodeExpireTime = null;

			await _userManager.UpdateAsync(user);

			// 6. Login sayfasına yönlendir
			TempData["SuccessMessage"] = "Email adresiniz doğrulandı! Giriş yapabilirsiniz.";
			return RedirectToAction("UserLogin", "Login");
		}

	}
}
