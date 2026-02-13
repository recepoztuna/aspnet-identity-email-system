using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2EmailNight.Context;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly EmailContext _context;
		private readonly SignInManager<AppUser> _signInManager;

		public ProfileController(UserManager<AppUser> userManager, EmailContext context, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_context = context;
			_signInManager = signInManager;
		}


		public async Task<IActionResult> Dashboard()
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			// 8 Widget İstatistikleri
			var stats = new
			{
				// 1. Toplam Mesaj (Gelen + Giden)
				TotalMessages = await _context.Messages
					.CountAsync(m => m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id),

				// 2. Gelen Mesajlar
				InboxCount = await _context.Messages
					.CountAsync(m => m.ReceiverId == currentUser.Id && !m.IsDeletedByReceiver && !m.IsDraft),

				// 3. Gönderilen Mesajlar
				SentCount = await _context.Messages
					.CountAsync(m => m.SenderId == currentUser.Id && !m.IsDraft && !m.IsDeletedBySender),

				// 4. Okunmamış Mesajlar
				UnreadCount = await _context.Messages
					.CountAsync(m => m.ReceiverId == currentUser.Id && !m.IsRead && !m.IsDeletedByReceiver),

				// 5. Yıldızlı Mesajlar
				StarredCount = await _context.Messages
					.CountAsync(m => (m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id) && m.IsStarred),

				// 6. Taslaklar
				DraftCount = await _context.Messages
					.CountAsync(m => m.SenderId == currentUser.Id && m.IsDraft),

				// 7. Toplam Kategori
				CategoryCount = await _context.Categories
					.CountAsync(c => c.UserId == currentUser.Id),

				// 8. Bugün Gelen Mesajlar
				TodayMessages = await _context.Messages
					.CountAsync(m => m.ReceiverId == currentUser.Id
								  && m.SentDate.Date == DateTime.Today)
			};

			// Kategoriye Göre Mesaj Sayıları
			var categoryStats = new List<dynamic>();

			foreach (var category in await _context.Categories.Where(c => c.UserId == currentUser.Id).ToListAsync())
			{
				var messageCount = await _context.MessageCategories
					.CountAsync(mc => mc.CategoryId == category.Id
								   && (mc.Message.ReceiverId == currentUser.Id || mc.Message.SenderId == currentUser.Id));

				categoryStats.Add(new
				{
					CategoryName = category.Name,
					MessageCount = messageCount
				});
			}

			ViewData["CategoryStats"] = categoryStats;

			// Son 5 Mesaj
			var recentMessages = await _context.Messages
				.Where(m => m.ReceiverId == currentUser.Id && !m.IsDeletedByReceiver)
				.Include(m => m.Sender)
				.OrderByDescending(m => m.SentDate)
				.Take(5)
				.ToListAsync();

			ViewData["Stats"] = stats;
			ViewData["CategoryStats"] = categoryStats;
			ViewData["RecentMessages"] = recentMessages;

			return View();
		}

		// ==========================================
		// PROFILE (Profil Görüntüleme/Düzenleme)
		// ==========================================
		public async Task<IActionResult> Profile()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			var model = new UserEditDto
			{
				Name = user.FirstName,
				SurName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				ImageUrl = user.ProfileImageUrl
			};

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Profile(UserEditDto model)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			try
			{
				// Bilgileri güncelle
				user.FirstName = model.Name;
				user.LastName = model.SurName;
				user.Email = model.Email;
				user.UserName = model.Email;
				user.PhoneNumber = model.PhoneNumber;

				// Profil fotoğrafı yükleme
				if (model.Image != null && model.Image.Length > 0)
				{
					var resource = Directory.GetCurrentDirectory();
					var extension = Path.GetExtension(model.Image.FileName);
					var imageName = Guid.NewGuid() + extension;
					var saveLocation = resource + "/wwwroot/Images/" + imageName;

					// Images klasörü yoksa oluştur
					var imagesFolder = Path.Combine(resource, "wwwroot/Images");
					if (!Directory.Exists(imagesFolder))
					{
						Directory.CreateDirectory(imagesFolder);
					}

					// Eski fotoğrafı sil
					if (!string.IsNullOrEmpty(user.ProfileImageUrl))
					{
						var oldImagePath = resource + "/wwwroot/Images/" + user.ProfileImageUrl;
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					// Yeni fotoğrafı kaydet
					var stream = new FileStream(saveLocation, FileMode.Create);
					await model.Image.CopyToAsync(stream);
					stream.Close();

					user.ProfileImageUrl = imageName;
				}

				var result = await _userManager.UpdateAsync(user);

				if (result.Succeeded)
				{
					TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi!";
					return RedirectToAction("Profile");
				}
				else
				{
					TempData["ErrorMessage"] = "Profil güncellenirken bir hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
			}

			return View(model);
		}

		public async Task<IActionResult> Settings()
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			return View(new ChangePasswordDto());
		}

		// Şifre Değiştirme
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
		{
			if (!ModelState.IsValid)
			{
				TempData["ErrorMessage"] = "Lütfen tüm alanları doğru doldurun.";
				return View("Settings", model);
			}

			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			try
			{
				var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

				if (result.Succeeded)
				{
					TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirildi!";
					return RedirectToAction("Settings");
				}
				else
				{
					TempData["ErrorMessage"] = string.Join(", ", result.Errors.Select(e => e.Description));
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
			}

			return View("Settings", model);
		}

		// Hesap Silme
		[HttpPost]
		public async Task<IActionResult> DeleteAccount(string confirmEmail)
		{
			var user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			// Email doğrulama
			if (confirmEmail != user.Email)
			{
				TempData["ErrorMessage"] = "Email adresi eşleşmiyor. Hesap silinemedi.";
				return RedirectToAction("Settings");
			}

			try
			{
				// Kullanıcının mesajlarını sil
				var messages = _context.Messages
					.Where(m => m.SenderId == user.Id || m.ReceiverId == user.Id)
					.ToList();
				_context.Messages.RemoveRange(messages);

				// Kategorilerini sil
				var categories = _context.Categories
					.Where(c => c.UserId == user.Id)
					.ToList();
				_context.Categories.RemoveRange(categories);

				await _context.SaveChangesAsync();

				// Kullanıcıyı sil
				var result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					await _signInManager.SignOutAsync();
					TempData["SuccessMessage"] = "Hesabınız başarıyla silindi.";
					return RedirectToAction("UserLogin", "Login");
				}
				else
				{
					TempData["ErrorMessage"] = "Hesap silinirken bir hata oluştu: " + string.Join(", ", result.Errors.Select(e => e.Description));
				}
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = "Bir hata oluştu: " + ex.Message;
			}

			return RedirectToAction("Settings");
		}

	}
}
