using System.Reflection;
using FinalProject.Application.Common.Exceptions;
using FinalProject.Application.Common.Interfaces;
using FinalProject.Application.Common.Security;

namespace FinalProject.Application.Common.Behaviours;

public class AuthorizationBehaviour<TRequest, TResponse>(
    IUser user,
    IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (authorizeAttributes?.Any() is true)
        {
            // Must be authenticated user
            if (user.Id == null)
            {
                throw new UnauthorizedAccessException();
            }

            // Role-based authorization
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles)).ToList();

            if (authorizeAttributesWithRoles?.Any() is true)
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = user.Roles?.Any(x => role == x)??false;
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
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy)).ToList();
            if (authorizeAttributesWithPolicies?.Any() is true)
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await identityService.AuthorizeAsync(user.Id, policy);

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
