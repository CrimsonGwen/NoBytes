
using Microsoft.Extensions.Options;
using Restaurant.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

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
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _emailSettings.FromName,
                _emailSettings.FromEmail));

            email.To.Add(MailboxAddress.Parse(ontvanger));
            email.Subject = onderwerp;

            email.Body = new BodyBuilder
            {
                HtmlBody = bericht
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    _emailSettings.Host,
                    _emailSettings.Port,
                    SecureSocketOptions.StartTls);

                Console.WriteLine("SMTP verbinding geslaagd.");

                // Tijdelijke controle van Gmail-instellingen
                Console.WriteLine(
                    "SMTP-gebruiker: " + _emailSettings.Username);

                Console.WriteLine(
                    "Wachtwoord aanwezig: " +
                    !string.IsNullOrWhiteSpace(_emailSettings.Password));

                Console.WriteLine(
                    "Wachtwoordlengte: " +
                    (_emailSettings.Password?.Length ?? 0));

                Console.WriteLine(
                    "Bevat spaties: " +
                    (_emailSettings.Password?.Contains(' ') ?? false));

                await smtp.AuthenticateAsync(
                    _emailSettings.Username,
                    _emailSettings.Password);

                Console.WriteLine("SMTP authenticatie geslaagd.");

                await smtp.SendAsync(email);

                Console.WriteLine("E-mail verzonden.");
            }
            finally
            {
                if (smtp.IsConnected)
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}
