using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace Itishnik.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }

    //TODO: IT-31
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation("Письмо отправлено на {email}: {htmlMessage}", email, htmlMessage);
        return Task.CompletedTask;
    }
}
