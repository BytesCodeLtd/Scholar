using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Scholar.Common.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public SmtpEmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            using MailMessage message = new()
            {
                From = new MailAddress(_options.FromEmail, _options.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            using SmtpClient client = new(_options.Host, _options.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_options.User, _options.Password)
            };

            await client.SendMailAsync(message);
        }
    }
}
