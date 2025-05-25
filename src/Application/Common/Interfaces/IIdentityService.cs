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

    Task<Result> CreateUserAsync<TUser>(TUser user) where TUser : ApplicationUser;

    Task<(Result Result, Guid UserId)> CreateUserAsync(string userName,
        string password,
        string name,
        string surname,
        string? patronymic = null);

    Task<Result> DeleteUserAsync(Guid userId);
}
