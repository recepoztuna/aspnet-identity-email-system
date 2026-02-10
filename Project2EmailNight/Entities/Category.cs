using System.ComponentModel.DataAnnotations;

namespace Project2EmailNight.Entities
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[StringLength(50)]
		public string Name { get; set; }

		[StringLength(50)]
		public string Icon { get; set; } = "bx-folder";

		[StringLength(50)]
		public string Color { get; set; } = "bg-primary";

		// Hangi kullanıcının kategorisi
		[Required]
		public string UserId { get; set; }
		public AppUser User { get; set; }

		public ICollection<MessageCategory> MessageCategories { get; set; }
	}
}
