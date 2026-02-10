using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Entities
{
	public class AppUser :IdentityUser
	{
		[StringLength(50)]
		public string? FirstName { get; set; }  // ← YENİ

		[StringLength(50)]
		public string? LastName { get; set; }   // ← YENİ
		public string? ProfileImageUrl { get; set; }
		public string? About { get; set; }

		// Bildirim tercihleri
		public bool EmailNotificationsEnabled { get; set; } = true;

		// ✅ EMAIL DOĞRULAMA İÇİN
		public string? EmailConfirmationCode { get; set; }  // 6 haneli kod
		public DateTime? CodeExpireTime { get; set; }        // Kod geçerlilik süresi


		// Gönderdiği mesajlar
		public ICollection<Message> SentMessages { get; set; }

		// Aldığı mesajlar
		public ICollection<Message> ReceivedMessages { get; set; }

		// Kategorileri
		public ICollection<Category> Categories { get; set; }
	

	}
}
