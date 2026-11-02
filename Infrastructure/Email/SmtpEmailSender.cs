using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Email
{
    // Slanje preko SMTP-a (Gmail, port 587 + STARTTLS, app password). Ako Email:Address/AppPassword nisu podeseni,
    // poruka se NE salje nego ispisuje u log - da se lokalno moze testirati tok (kod se vidi u logu) bez pravog naloga.
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IOptions<EmailOptions> options, ILogger<SmtpEmailSender> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string toAddress, string subject, string htmlBody, string textBody, CancellationToken ct = default)
        {
            if (!_options.IsConfigured)
            {
                _logger.LogWarning("E-mail nije konfigurisan (Email:Address/AppPassword) - poruka NIJE poslata. Za {To}: {Subject}\n{Body}", toAddress, subject, textBody);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.DisplayName, _options.Address));
            message.To.Add(MailboxAddress.Parse(toAddress));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody, TextBody = textBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_options.Address, _options.AppPassword, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
