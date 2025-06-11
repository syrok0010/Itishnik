using Itishnik.Application.Common.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Itishnik.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailSettings _emailSettings;

    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _logger = logger;
        _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>()!;
    }
    
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port);
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Письмо успешно отправлено на {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при отправке письма на {Email}, {Message}", email, htmlMessage);
        }
    }
}
