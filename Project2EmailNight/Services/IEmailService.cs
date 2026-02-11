namespace Project2EmailNight.Services
{
	public interface IEmailService
	{
		Task SendConfirmationCodeAsync(string toEmail, string code);
	}
}
