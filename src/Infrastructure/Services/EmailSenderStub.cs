using Microsoft.AspNetCore.Identity.UI.Services;

namespace Itishnik.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    //TODO: IT-31
    public Task SendEmailAsync(string email, string subject, string htmlMessage) => Task.CompletedTask;
}
