using Itishnik.Domain.Constants;
using System.Reflection;
using Itishnik.Application.Common.Exceptions;
using Itishnik.Application.Common.Interfaces;
using Itishnik.Application.Common.Security;

namespace Itishnik.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(
        IUser user,
        IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            var userId = _user.Id.Value;

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(userId, role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    throw new ForbiddenAccessException();
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            if (authorizeAttributesWithPolicies.Any())
            {
                var resourceMetadataAttribute = request.GetType().GetCustomAttribute<ResourceMetadataAttribute>();

                foreach (var policyName in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    bool authorized;

                    if (policyName == Policies.OwnerOrAdmin && resourceMetadataAttribute is not null)
                    {
                        var resourceIdProperty = request.GetType().GetProperty(resourceMetadataAttribute.ResourceIdPropertyName);
                        if (resourceIdProperty is null)
                        {
                            throw new InvalidOperationException($"Property '{resourceMetadataAttribute.ResourceIdPropertyName}' not found on request '{typeof(TRequest).Name}' required by ResourceMetadataAttribute for policy '{policyName}'.");
                        }

                        var resourceId = resourceIdProperty.GetValue(request);
                        if (resourceId is null)
                        {
                             throw new InvalidOperationException($"Resource identifier from property '{resourceMetadataAttribute.ResourceIdPropertyName}' is null for policy '{policyName}' on request '{typeof(TRequest).Name}'.");
                        }

                        // Вызываем новый метод IIdentityService для ресурсной политики
                        authorized = await _identityService.AuthorizeAsync(userId, policyName, resourceId, resourceMetadataAttribute.ResourceType);
                    }
                    else
                    {
                        authorized = await _identityService.AuthorizeAsync(userId, policyName);
                    }


                    if (!authorized)
                    {
                        throw new ForbiddenAccessException();
                    }
                }
            }
        }

        // User is authorized / authorization not required
        return await next();
    }
}
