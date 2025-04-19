using System.Security.Claims;
using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Web.Services;

/// <summary>
/// Реализация интерфейса <see cref="IUser"/> для получения данных текущего пользователя из HttpContext.
/// </summary>
/// <param name="httpContextAccessor">Аксессор для доступа к HttpContext.</param>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <inheritdoc/>
    public Guid? Id => Guid.Parse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString() );

    /// <inheritdoc/>
    public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? [];
}
