using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project2EmailNight.Context;
using Project2EmailNight.Dtos;
using Project2EmailNight.Entities;
using MimeKit;
using MailKit.Net.Smtp;

namespace Project2EmailNight.Controllers
{
	[Authorize]  // ← Giriş yapmış kullanıcılar erişebilsin
	public class EmailController : Controller
	{
		private readonly EmailContext _context;
		private readonly UserManager<AppUser> _userManager;

		public EmailController(
			EmailContext context,
			UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		// GET: Gelen Kutusu (Ana Sayfa)
		public async Task<IActionResult> Index(int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			var messages = await _context.Messages
				.Where(m => m.ReceiverId == currentUser.Id
						 && !m.IsDeletedByReceiver
						 && !m.IsDraft)
				.Include(m => m.Sender)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;

			return View(messages);
		}
		// YILDIZLI MESAJLAR
		public async Task<IActionResult> Starred(int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			var messages = await _context.Messages
				.Where(m => m.ReceiverId == currentUser.Id
						 && m.IsStarred
						 && !m.IsDeletedByReceiver
						 && !m.IsDraft)
				.Include(m => m.Sender)
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;
			ViewData["PageTitle"] = "Yıldızlı Mesajlar";

			return View("Index", messages);
		}

		// GÖNDERİLENLER
		public async Task<IActionResult> Sent(int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			var messages = await _context.Messages
				.Where(m => m.SenderId == currentUser.Id
						 && !m.IsDraft
						 && !m.IsDeletedBySender)
				.Include(m => m.Receiver) // Receiver çünkü gönderen benim
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;
			ViewData["PageTitle"] = "Gönderilenler";
			ViewData["IsSentPage"] = true; // Sender yerine Receiver göstermek için

			return View("Index", messages);
		}

		// TASLAKLAR
		public async Task<IActionResult> Draft(int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			var messages = await _context.Messages
				.Where(m => m.SenderId == currentUser.Id
						 && m.IsDraft
						 && !m.IsDeletedBySender)
				.Include(m => m.Receiver)
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;
			ViewData["PageTitle"] = "Taslaklar";
			ViewData["IsDraftPage"] = true;

			return View("Index", messages);
		}

		// ÇÖP KUTUSU
		public async Task<IActionResult> Trash(int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			// Hem gelen hem gönderilen silinmiş mesajlar
			var messages = await _context.Messages
				.Where(m => (m.ReceiverId == currentUser.Id && m.IsDeletedByReceiver)
						 || (m.SenderId == currentUser.Id && m.IsDeletedBySender))
				.Include(m => m.Sender)
				.Include(m => m.Receiver)
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;
			ViewData["PageTitle"] = "Çöp Kutusu";
			ViewData["IsTrashPage"] = true;

			return View("Index", messages);
		}

		// KATEGORİ MESAJLARI
		public async Task<IActionResult> Category(int id, int page = 1)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			// Kategori kontrolü (kullanıcının kategorisi mi?)
			var category = await _context.Categories
				.FirstOrDefaultAsync(c => c.Id == id && c.UserId == currentUser.Id);

			if (category == null)
				return NotFound();

			// Kategoriye ait mesajlar (MessageCategories çoka-çok)
			var messages = await _context.Messages
				.Where(m => m.ReceiverId == currentUser.Id
						 && !m.IsDeletedByReceiver
						 && !m.IsDraft
						 && m.MessageCategories.Any(mc => mc.CategoryId == id))
				.Include(m => m.Sender)
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.OrderByDescending(m => m.SentDate)
				.ToListAsync();

			ViewBag.CurrentPage = page;
			ViewData["PageTitle"] = category.Name;
			ViewData["CategoryName"] = category.Name;

			return View("Index", messages);
		}
		// YILDIZ TOGGLE (AJAX)
		[HttpPost]
		public async Task<IActionResult> ToggleStar(int id)
		{
			try
			{
				var currentUser = await _userManager.GetUserAsync(User);
				if (currentUser == null)
					return Json(new { success = false, message = "Oturum bulunamadı" });

				var message = await _context.Messages
					.FirstOrDefaultAsync(m => m.Id == id
										   && (m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id));

				if (message == null)
					return Json(new { success = false, message = "Mesaj bulunamadı" });

				// Yıldızı toggle et
				message.IsStarred = !message.IsStarred;
				await _context.SaveChangesAsync();

				return Json(new { success = true, isStarred = message.IsStarred });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
			}
		}

		// MESAJ SİLME (AJAX - SOFT DELETE)
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var currentUser = await _userManager.GetUserAsync(User);
				if (currentUser == null)
					return Json(new { success = false, message = "Oturum bulunamadı" });

				var message = await _context.Messages
					.FirstOrDefaultAsync(m => m.Id == id
										   && (m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id));

				if (message == null)
					return Json(new { success = false, message = "Mesaj bulunamadı" });

				// Soft delete - Kullanıcıya göre işaretle
				if (message.ReceiverId == currentUser.Id)
				{
					message.IsDeletedByReceiver = true;
				}

				if (message.SenderId == currentUser.Id)
				{
					message.IsDeletedBySender = true;
				}

				await _context.SaveChangesAsync();

				return Json(new { success = true, message = "Mesaj çöp kutusuna taşındı" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
			}
		}
		// ARAMA (AJAX)
		[HttpPost]
		public async Task<IActionResult> Search(string query)
		{
			try
			{
				var currentUser = await _userManager.GetUserAsync(User);
				if (currentUser == null)
					return Json(new { success = false, message = "Oturum bulunamadı" });

				if (string.IsNullOrWhiteSpace(query))
					return Json(new { success = true, results = new List<object>() });

				var messages = await _context.Messages
					.Where(m => m.ReceiverId == currentUser.Id
							 && !m.IsDeletedByReceiver
							 && !m.IsDraft
							 && (m.Subject.Contains(query)
							  || m.Body.Contains(query)
							  || (m.Sender.FirstName + " " + m.Sender.LastName).Contains(query)))
					.Include(m => m.Sender)
					.OrderByDescending(m => m.SentDate)
					.Take(50) // İlk 50 sonuç
					.Select(m => new {
						id = m.Id,
						senderName = m.Sender.FirstName + " " + m.Sender.LastName,
						senderInitials = m.Sender.FirstName.Substring(0, 1) + m.Sender.LastName.Substring(0, 1),
						subject = m.Subject,
						body = m.Body.Length > 50 ? m.Body.Substring(0, 50) + "..." : m.Body,
						sentDate = m.SentDate,
						isRead = m.IsRead,
						isStarred = m.IsStarred
					})
					.ToListAsync();

				return Json(new { success = true, results = messages });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Arama hatası: " + ex.Message });
			}
		}

		// TOPLU SİLME (AJAX)
		[HttpPost]
		public async Task<IActionResult> DeleteMultiple([FromBody] List<int> messageIds)
		{
			try
			{
				var currentUser = await _userManager.GetUserAsync(User);
				if (currentUser == null)
					return Json(new { success = false, message = "Oturum bulunamadı" });

				if (messageIds == null || !messageIds.Any())
					return Json(new { success = false, message = "Silinecek mesaj seçilmedi" });

				var messages = await _context.Messages
					.Where(m => messageIds.Contains(m.Id)
							 && (m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id))
					.ToListAsync();

				int deletedCount = 0;
				foreach (var message in messages)
				{
					if (message.ReceiverId == currentUser.Id)
					{
						message.IsDeletedByReceiver = true;
						deletedCount++;
					}

					if (message.SenderId == currentUser.Id)
					{
						message.IsDeletedBySender = true;
						deletedCount++;
					}
				}

				await _context.SaveChangesAsync();

				return Json(new { success = true, message = $"{deletedCount} mesaj silindi" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Silme hatası: " + ex.Message });
			}
		}
		// MESAJ DETAYI
		public async Task<IActionResult> Read(int id)
		{
			var currentUser = await _userManager.GetUserAsync(User);
			if (currentUser == null)
				return RedirectToAction("UserLogin", "Login");

			var message = await _context.Messages
				.Include(m => m.Sender)
				.Include(m => m.Receiver)
				.Include(m => m.MessageCategories)
					.ThenInclude(mc => mc.Category)
				.FirstOrDefaultAsync(m => m.Id == id
									  && (m.ReceiverId == currentUser.Id || m.SenderId == currentUser.Id));

			if (message == null)
				return NotFound();

			// Okundu olarak işaretle (sadece alıcı için)
			if (message.ReceiverId == currentUser.Id && !message.IsRead)
			{
				message.IsRead = true;
				await _context.SaveChangesAsync();
			}

			return View(message);
		}
		// MESAJ GÖNDER (AJAX - COMPOSE MODAL)
		[HttpPost]
		public async Task<IActionResult> Compose([FromBody] ComposeDto model)
		{
			try
			{
				var currentUser = await _userManager.GetUserAsync(User);
				if (currentUser == null)
					return Json(new { success = false, message = "Oturum bulunamadı" });

				// Alıcıyı bul
				var receiver = await _userManager.FindByEmailAsync(model.To);
				if (receiver == null)
					return Json(new { success = false, message = "Alıcı bulunamadı. Geçerli bir email adresi girin." });

				// Kendine mesaj göndermesini engelle
				if (receiver.Id == currentUser.Id)
					return Json(new { success = false, message = "Kendinize mesaj gönderemezsiniz." });

				// Mesajı oluştur
				var message = new Message
				{
					SenderId = currentUser.Id,
					ReceiverId = receiver.Id,
					Subject = model.Subject,
					Body = model.Body,
					SentDate = DateTime.Now,
					IsRead = false,
					IsStarred = false,
					IsDraft = false,
					IsDeletedBySender = false,
					IsDeletedByReceiver = false
				};

				_context.Messages.Add(message);
				await _context.SaveChangesAsync();

				// Kategorileri ekle (varsa)
				if (model.Categories != null && model.Categories.Any())
				{
					foreach (var categoryId in model.Categories)
					{
						// Kategorinin kullanıcıya ait olduğunu kontrol et
						var categoryExists = await _context.Categories
							.AnyAsync(c => c.Id == categoryId && c.UserId == currentUser.Id);

						if (categoryExists)
						{
							_context.MessageCategories.Add(new MessageCategory
							{
								MessageId = message.Id,
								CategoryId = categoryId
							});
						}
					}
					await _context.SaveChangesAsync();
				}

				return Json(new { success = true, message = "Mesaj başarıyla gönderildi!" });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = "Bir hata oluştu: " + ex.Message });
			}
		}

		//İlerde Gerçek Email atmak istersek burayı çalıştırırız.


		//public IActionResult SendEmail()
		//{
		//	return View();
		//}

		//[HttpPost]
		//public IActionResult SendEmail(MailRequestDto mailRequestDto)
		//{
		//	MimeMessage mimeMessage = new MimeMessage();

		//	MailboxAddress mailboxAddressFrom = new MailboxAddress("Identity Admin", "327hastanesibilgisistemi@gmail.com");
		//	mimeMessage.From.Add(mailboxAddressFrom);

		//	MailboxAddress mailboxAddressTo = new MailboxAddress("User",mailRequestDto.ReceiverEmail);
		//	mimeMessage.To.Add(mailboxAddressTo);

		//	mimeMessage.Subject=mailRequestDto.Subject;

		//	var bodyBuilder = new BodyBuilder();
		//	bodyBuilder.TextBody=mailRequestDto.MessageDetail;
		//	mimeMessage.Body=bodyBuilder.ToMessageBody();


		//	SmtpClient smtpClient = new SmtpClient();
		//	smtpClient.Connect("smtp.gmail.com", 587, false);
		//	smtpClient.Authenticate("327hastanesibilgisistemi@gmail.com", "jvjk qkor afdo ptoi");
		//	smtpClient.Send(mimeMessage);
		//	smtpClient.Disconnect(true);

		//	return RedirectToAction("UserList");
		//}
	}
}
