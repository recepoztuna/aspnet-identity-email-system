using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project2EmailNight.Entities;

namespace Project2EmailNight.Context
{
	public class EmailContext: IdentityDbContext<AppUser>
	{

		public EmailContext(DbContextOptions<EmailContext> options)
			: base(options)
		{
		}

		public DbSet<Message> Messages { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<MessageCategory> MessageCategories { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Message - Sender İlişkisi
			builder.Entity<Message>()
				.HasOne(m => m.Sender)
				.WithMany(u => u.SentMessages)
				.HasForeignKey(m => m.SenderId)
				.OnDelete(DeleteBehavior.Restrict);

			// Message - Receiver İlişkisi
			builder.Entity<Message>()
				.HasOne(m => m.Receiver)
				.WithMany(u => u.ReceivedMessages)
				.HasForeignKey(m => m.ReceiverId)
				.OnDelete(DeleteBehavior.Restrict);

			// MessageCategory - Composite Key
			builder.Entity<MessageCategory>()
				.HasKey(mc => new { mc.MessageId, mc.CategoryId });

			builder.Entity<MessageCategory>()
				.HasOne(mc => mc.Message)
				.WithMany(m => m.MessageCategories)
				.HasForeignKey(mc => mc.MessageId);

			builder.Entity<MessageCategory>()
				.HasOne(mc => mc.Category)
				.WithMany(c => c.MessageCategories)
				.HasForeignKey(mc => mc.CategoryId);

			// Category - User İlişkisi
			builder.Entity<Category>()
				.HasOne(c => c.User)
				.WithMany(u => u.Categories)
				.HasForeignKey(c => c.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
