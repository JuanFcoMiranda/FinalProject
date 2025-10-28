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

        if (authorizeAttributes?.Any() is not true)
        {
            return await next();
        }

        EnsureUserIsAuthenticated();

        await ValidateRoleBasedAuthorization(authorizeAttributes);
        await ValidatePolicyBasedAuthorization(authorizeAttributes);

        return await next();
    }

    private void EnsureUserIsAuthenticated()
    {
        if (user.Id == null)
        {
            throw new UnauthorizedAccessException();
        }
    }

    private async Task ValidateRoleBasedAuthorization(List<AuthorizeAttribute> authorizeAttributes)
    {
        var attributesWithRoles = authorizeAttributes
    .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
          .ToList();

        if (!attributesWithRoles.Any())
        {
            return;
        }

        var isAuthorized = attributesWithRoles
            .SelectMany(a => a.Roles.Split(','))
     .Any(role => IsUserInRole(role.Trim()));

        if (!isAuthorized)
        {
            throw new ForbiddenAccessException();
        }
    }

    private bool IsUserInRole(string role)
    {
        return user.Roles?.Any(x => role == x) ?? false;
    }

    private async Task ValidatePolicyBasedAuthorization(List<AuthorizeAttribute> authorizeAttributes)
    {
        var attributesWithPolicies = authorizeAttributes
                 .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
                 .ToList();

        if (!attributesWithPolicies.Any())
        {
            return;
        }

        foreach (var policy in attributesWithPolicies.Select(a => a.Policy))
        {
            var authorized = await identityService.AuthorizeAsync(user.Id!, policy);

            if (!authorized)
            {
                throw new ForbiddenAccessException();
            }
        }
    }
}
