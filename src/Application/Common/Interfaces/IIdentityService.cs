using Itishnik.Application.Common.Models;

namespace Itishnik.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(Guid userId);

    Task<bool> IsInRoleAsync(Guid userId, string role);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);

    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName,
        string password,
        string name,
        string surname,
        string? patronymic = null);

    Task<Result> DeleteUserAsync(Guid userId);
}
