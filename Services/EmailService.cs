using Microsoft.Extensions.Options;
using Restaurant.Configuration;
using System.Net;
using System.Net.Mail;

namespace Restaurant.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task VerstuurEmailAsync(
            string ontvanger,
            string onderwerp,
            string bericht)
        {
            using SmtpClient smtpClient = new SmtpClient(
                _emailSettings.Host,
                _emailSettings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _emailSettings.Username,
                    _emailSettings.Password)
            };

            using MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(
                    _emailSettings.FromEmail,
                    _emailSettings.FromName),

                Subject = onderwerp,
                Body = bericht,
                IsBodyHtml = true
            };

            mailMessage.To.Add(ontvanger);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
