using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project2EmailNight.Context;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Controllers
{
	public class MessageController : Controller
	{
		private readonly EmailContext _emailContext;
		private readonly UserManager<AppUser> _userManager;

		public MessageController(EmailContext emailContext, UserManager<AppUser> userManager)
		{
			_emailContext = emailContext;
			_userManager = userManager;
		}

		[HttpGet]
		public IActionResult CreateMessage()
		{
			return View();
		}
	
	}
}
