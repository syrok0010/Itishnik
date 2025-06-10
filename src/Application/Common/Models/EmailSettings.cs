namespace Itishnik.Application.Common.Models;

public class EmailSettings
{
    public string SmtpServer { get; init; } = null!;
    public int Port { get; init; }
    public string FromName { get; init; } = null!;
    public string FromEmail { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}
