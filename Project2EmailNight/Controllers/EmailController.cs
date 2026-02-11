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
		public IActionResult SendEmail()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SendEmail(MailRequestDto mailRequestDto)
		{
			MimeMessage mimeMessage = new MimeMessage();

			MailboxAddress mailboxAddressFrom = new MailboxAddress("Identity Admin", "327hastanesibilgisistemi@gmail.com");
			mimeMessage.From.Add(mailboxAddressFrom);

			MailboxAddress mailboxAddressTo = new MailboxAddress("User",mailRequestDto.ReceiverEmail);
			mimeMessage.To.Add(mailboxAddressTo);

			mimeMessage.Subject=mailRequestDto.Subject;

			var bodyBuilder = new BodyBuilder();
			bodyBuilder.TextBody=mailRequestDto.MessageDetail;
			mimeMessage.Body=bodyBuilder.ToMessageBody();


			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Connect("smtp.gmail.com", 587, false);
			smtpClient.Authenticate("327hastanesibilgisistemi@gmail.com", "jvjk qkor afdo ptoi");
			smtpClient.Send(mimeMessage);
			smtpClient.Disconnect(true);

			return RedirectToAction("UserList");
		}
	}
}
