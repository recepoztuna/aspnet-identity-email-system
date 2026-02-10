namespace Project2EmailNight.Entities
{
	public class MessageCategory
	{
		public int MessageId { get; set; }
		public Message Message { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }
	}
}
