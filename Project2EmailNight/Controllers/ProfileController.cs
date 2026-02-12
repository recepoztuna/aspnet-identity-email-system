using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2EmailNight.Context;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
	[Authorize]
	public class ProfileController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly EmailContext _context;

		public ProfileController(UserManager<AppUser> userManager, EmailContext context)
		{
			_userManager = userManager;
			_context = context;
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
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			return View(user);
		}

		// ==========================================
		// SETTINGS (Ayarlar - Şifre Değiştirme vb.)
		// ==========================================
		public async Task<IActionResult> Settings()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
				return RedirectToAction("UserLogin", "Login");

			return View(user);
		}
	}
}
//	public class ProfileController : Controller
//	{
//		private readonly UserManager<AppUser> _userManager;

//		public ProfileController(UserManager<AppUser> userManager)
//		{
//			_userManager = userManager;
//		}

//		//public async Task<IActionResult> Index()
//		//{
//		//	var values = await _userManager.FindByNameAsync(User.Identity.Name);
//		//	UserEditDto userEditDto= new UserEditDto();
//		//	userEditDto.Name = values.;
//		//	userEditDto.SurName = values.SurName;
//		//	userEditDto.ImageUrl = values.ImageUrl;
//		//	userEditDto.Email = values.Email;
//		//	return View(userEditDto);
//		//}
//		//[HttpPost]
//		//public async Task<IActionResult> Index(UserEditDto userEditDto)
//		//{
//		//	var user = await _userManager.FindByNameAsync(User.Identity.Name);
//		//	user.Name = userEditDto.Name;
//		//	user.SurName = userEditDto.SurName;
//		//	user.Email = userEditDto.Email;
//		//	user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userEditDto.Password);

//		//	var resource= Directory.GetCurrentDirectory();
//		//	var extension=Path.GetExtension(userEditDto.Image.FileName);
//		//	var imageName = Guid.NewGuid() + extension;
//		//	var saveLocation = resource + "/wwwroot/Images/" + imageName;
//		//	var stream = new FileStream(saveLocation, FileMode.Create);
//		//	await userEditDto.Image.CopyToAsync(stream);
//		//	user.ImageUrl= imageName;

//		//	var resault = await _userManager.UpdateAsync(user);
//		//	if (resault.Succeeded)
//		//	{
//		//		return RedirectToAction("Inbox", "Message");
//		//	}
//		//	return View();
//		//}
//	}
//}
