using MailKit.Net.Smtp;  // ← SMTP (email göndermek için gerekli)
using MimeKit;           // ← MimeMessage (email mesajı oluşturmak için)
namespace Project2EmailNight.Services
{
	public class EmailService : IEmailService
	{
		public async Task SendConfirmationCodeAsync(string toEmail, string code)
		{
			// 1. Email mesajı oluştur
			var mimeMessage = new MimeMessage();

			// 2. Gönderen bilgisi (kimden gidiyor)
			mimeMessage.From.Add(new MailboxAddress("EmailApp", "327hastanesibilgisistemi@gmail.com"));

			// 3. Alıcı bilgisi (kime gidiyor)
			mimeMessage.To.Add(new MailboxAddress("", toEmail));

			// 4. Email konusu
			mimeMessage.Subject = "Email Doğrulama Kodu";

			// 5. Email içeriği (HTML)
			var bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = $@"
            <h2>Email Doğrulama</h2>
            <p>Merhaba,</p>
            <p>Email adresinizi doğrulamak için aşağıdaki kodu kullanın:</p>
            <h1 style='color: blue;'>{code}</h1>
            <p>Bu kod 15 dakika geçerlidir.</p>
        ";
			mimeMessage.Body = bodyBuilder.ToMessageBody();

			// 6. SMTP ile gönder
			using (var smtpClient = new SmtpClient())
			{
				// Gmail SMTP sunucusuna bağlan
				await smtpClient.ConnectAsync("smtp.gmail.com", 587, false);

				// Gmail hesabı ile kimlik doğrula
				await smtpClient.AuthenticateAsync(
					"327hastanesibilgisistemi@gmail.com",
					"jvjk qkor afdo ptoi"
				);

				// Email'i gönder
				await smtpClient.SendAsync(mimeMessage);

				// Bağlantıyı kapat
				await smtpClient.DisconnectAsync(true);
			}
		}
	}
}
