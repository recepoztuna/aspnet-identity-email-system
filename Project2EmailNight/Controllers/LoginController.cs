using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;
using Project2EmailNight.Services;  

namespace Project2EmailNight.Controllers
{
	public class LoginController : Controller
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager; 
		private readonly IEmailService _emailService;

		public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IEmailService emailService)
		{
			_signInManager = signInManager;
			_userManager = userManager;
			_emailService = emailService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult UserLogin()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UserLogin(UserLoginDto model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// 1. Kullanıcıyı email ile bul
			var user = await _userManager.FindByEmailAsync(model.Username);

			if (user == null)
			{
				ModelState.AddModelError("", "Email veya şifre hatalı");
				return View(model);
			}

			// 2. Şifre kontrolü
			var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

			if (!checkPassword)
			{
				ModelState.AddModelError("", "Email veya şifre hatalı");
				return View(model);
			}

			// 3. ✅ EMAIL CONFIRM KONTROLÜ
			if (!user.EmailConfirmed)
			{
				TempData["Email"] = user.Email;
				TempData["ErrorMessage"] = "Email adresiniz doğrulanmamış. Lütfen email adresinize gönderilen kodu girin.";
				return RedirectToAction("EmailNotConfirmed", "Login");
			}

			// 4. GİRİŞ BAŞARILI
			var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Email");
			}

			ModelState.AddModelError("", "Giriş başarısız");
			return View(model);
		}

		[HttpGet]
		public IActionResult EmailNotConfirmed()
		{
			ViewBag.Email = TempData.Peek("Email");
			ViewBag.ErrorMessage = TempData.Peek("ErrorMessage");
			return View();
		}

		// POST: Doğrulama Kodunu Yeniden Gönder
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResendConfirmationCode(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null)
			{
				return RedirectToAction("UserLogin");
			}

			// Yeni 6 haneli kod oluştur
			Random random = new Random();
			string confirmCode = random.Next(100000, 999999).ToString();

			user.EmailConfirmationCode = confirmCode;
			user.CodeExpireTime = DateTime.Now.AddMinutes(15);

			await _userManager.UpdateAsync(user);

			// Email gönder
			await _emailService.SendConfirmationCodeAsync(user.Email, confirmCode);

			TempData["Email"] = user.Email;
			TempData["SuccessMessage"] = "Yeni doğrulama kodu email adresinize gönderildi.";

			return RedirectToAction("ConfirmEmail", "Register");
		}
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			TempData["SuccessMessage"] = "Başarıyla çıkış yaptınız.";
			return RedirectToAction("UserLogin");
		}
	}
}
