using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Entities
{
	public class Message
	{
		public int Id { get; set; }

		[Required]
		[StringLength(200)]
		public string Subject { get; set; }

		[Required]
		public string Body { get; set; }

		public DateTime SentDate { get; set; } = DateTime.Now;

		// Durum Flagleri
		public bool IsRead { get; set; } = false;
		public bool IsStarred { get; set; } = false;
		public bool IsDraft { get; set; } = false;
		public bool IsImportant { get; set; } = false;

		// Silme (her kullanıcı için ayrı)
		public bool IsDeletedBySender { get; set; } = false;
		public bool IsDeletedByReceiver { get; set; } = false;

		// Gönderen
		[Required]
		public string SenderId { get; set; }
		public AppUser Sender { get; set; }

		// Alıcı (Taslakta null olabilir)
		public string? ReceiverId { get; set; }
		public AppUser? Receiver { get; set; }

		// Kategoriler
		public ICollection<MessageCategory> MessageCategories { get; set; }


	}
}
