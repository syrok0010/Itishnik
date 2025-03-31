using System.Security.Claims;
using Itishnik.Application.Common.Interfaces;

namespace Itishnik.Web.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    public Guid? Id => Guid.Parse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
}
