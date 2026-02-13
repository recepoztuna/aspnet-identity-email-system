namespace Project2EmailNight.Dtos
{
	public class UserEditDto
	{
		public string Name { get; set; }
		public string SurName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string ImageUrl { get; set; }
		public IFormFile Image { get; set; }

	}
}
