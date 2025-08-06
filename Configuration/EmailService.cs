using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using IntelliSoftAPIV2.Configuration;

namespace IntelliSoftAPIV2.Configuration
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml, byte[]? archivoAdjunto = null, string? nombreArchivo = null)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(MailboxAddress.Parse(destinatario));
            message.Subject = asunto;

            var builder = new BodyBuilder { HtmlBody = cuerpoHtml };

            if (archivoAdjunto != null && !string.IsNullOrWhiteSpace(nombreArchivo))
            {
                builder.Attachments.Add(nombreArchivo, archivoAdjunto, ContentType.Parse("application/pdf"));
            }

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
