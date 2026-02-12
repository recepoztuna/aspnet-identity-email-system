namespace Project2EmailNight.Dtos
{
	public class ComposeDto
	{
		public string To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public List<int> Categories { get; set; }
	}
}
