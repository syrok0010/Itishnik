namespace Itishnik.Application.Common.Interfaces;

public interface IResetPasswordService
{
    Task SendResetPasswordEmail(string email);
}
