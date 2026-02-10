using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;
using static Project2EmailNight.Dtos.UserRegisterDto;

namespace Project2EmailNight.Controllers
{
	public class RegisterController : Controller
	{
		private readonly UserManager<AppUser> _userManager;

		public RegisterController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult CreateUser()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> CreateUser(UserRegisterDto model)
		{
			if (!ModelState.IsValid)
			{
				return View("Index", model);
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
				// Email gönder (şimdilik yorum)
				// await _emailService.SendConfirmationCodeAsync(user.Email, confirmCode);

				TempData["Email"] = user.Email;
				TempData["SuccessMessage"] = "Kayıt başarılı! Email adresinize doğrulama kodu gönderildi.";

				return RedirectToAction("ConfirmEmail", "Register");
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View("Index", model);
		}
	}
}
