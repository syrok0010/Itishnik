using Itishnik.Application.Common.Models;
using Itishnik.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace Itishnik.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(Guid userId);

    Task<bool> IsInRoleAsync(Guid userId, string role);

    Task<bool> AuthorizeAsync(Guid userId, string policyName);
    
    Task<bool> AuthorizeAsync(Guid userId, string policyName, object resourceId, Type resourceType);

    Task<(Result Result, ApplicationUser User)> CreateUserAsync<TUser>(string email) where TUser : ApplicationUser, new();

    Task<(bool Success, string? Token)> GetPasswordResetTokenAsync(Guid userId);
    
    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName,
        string password,
        string name,
        string surname,
        string? patronymic = null);

    Task<Result> DeleteUserAsync(Guid userId);
}
