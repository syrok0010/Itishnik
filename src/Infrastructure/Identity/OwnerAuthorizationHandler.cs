using System.Security.Claims;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Domain.Common;
using Microsoft.AspNetCore.Authorization;

namespace Itishnik.Infrastructure.Identity;

public class OwnerAuthorizationHandler(IIdentityService identityService)
    : AuthorizationHandler<OwnerRequirement, IOwnedResource>
{
    private readonly IIdentityService _identityService = identityService;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerRequirement requirement,
        IOwnedResource resource)
    {
        var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out var userId))
        {
            context.Fail(new AuthorizationFailureReason(this, "User ID not found or invalid."));
            return Task.CompletedTask;
        }

        if (resource.GetOwnerId() != userId)
        {
            context.Fail(new AuthorizationFailureReason(this, "User is not the owner."));
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
