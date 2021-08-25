using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VueCore.Models.Options;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace VueCore.Services
{
    public class IdentityEmailSender : IEmailSender
    {
        private readonly ILogger<IdentityEmailSender> _logger;
        private readonly SmtpOptions _options;

        public IdentityEmailSender(ILogger<IdentityEmailSender> logger, IOptions<SmtpOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Sending email to {email} for {subject}");
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_options.DefaultSender));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) 
            {
                Text = htmlMessage
            };
            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, int.Parse(_options.Port), false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}