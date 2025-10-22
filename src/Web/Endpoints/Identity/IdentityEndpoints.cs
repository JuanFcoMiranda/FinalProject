using FinalProject.Infrastructure.Identity;

namespace FinalProject.Web.Endpoints.Identity;

/// <summary>
/// Endpoint group for ASP.NET Core Identity API endpoints (register, login, etc.)
/// These endpoints provide built-in user management functionality.
/// </summary>
public static class IdentityEndpoints
{
    public static void MapIdentityEndpoints(this IEndpointRouteBuilder app)
    {
        // Map the built-in Identity API endpoints for registration, login, 2FA, etc.
        // This provides endpoints like:
        // POST /identity/register
        // POST /identity/login
        // POST /identity/refresh
        // GET /identity/confirmEmail
        // POST /identity/resendConfirmationEmail
        // POST /identity/forgotPassword
        // POST /identity/resetPassword
        // POST /identity/manage/2fa
        // GET /identity/manage/info
        // POST /identity/manage/info
        
        app.MapGroup("/identity")
           .MapIdentityApi<ApplicationUser>()
           .WithTags("Identity");
    }
}
