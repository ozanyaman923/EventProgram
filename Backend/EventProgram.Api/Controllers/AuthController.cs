using System.Security.Claims;
using EventProgram.Api.Authentication;
using EventProgram.Application.Contracts.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventProgram.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    GoogleAuthenticationStatus googleAuthenticationStatus,
    IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("status")]
    public ActionResult<object> GetStatus() =>
        Ok(new { googleLoginConfigured = googleAuthenticationStatus.IsConfigured });

    [AllowAnonymous]
    [HttpGet("google-login")]
    public IActionResult GoogleLogin([FromQuery] string returnPath = "/")
    {
        if (!googleAuthenticationStatus.IsConfigured)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                title: "Google login is not configured.",
                detail: "Add the Google ClientId and ClientSecret to .NET User Secrets.");
        }

        var frontendBaseUrl = configuration["Frontend:BaseUrl"]?.TrimEnd('/');
        var safeReturnPath = IsSafeLocalPath(returnPath) ? returnPath : "/";
        var redirectUri = string.IsNullOrWhiteSpace(frontendBaseUrl)
            ? safeReturnPath
            : $"{frontendBaseUrl}{safeReturnPath}";

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUri },
            GoogleDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<AuthenticatedUser> Me()
    {
        var idClaim = User.FindFirstValue(EventProgramClaimTypes.UserId);
        var displayName = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (!Guid.TryParse(idClaim, out var userId)
            || string.IsNullOrWhiteSpace(displayName)
            || string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized();
        }

        return Ok(new AuthenticatedUser(userId, displayName, email, IsBlocked: false));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    private static bool IsSafeLocalPath(string path) =>
        path.StartsWith('/') && !path.StartsWith("//") && !path.Contains('\\');
}
