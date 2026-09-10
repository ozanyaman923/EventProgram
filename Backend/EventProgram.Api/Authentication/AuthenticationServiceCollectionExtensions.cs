using System.Security.Claims;
using EventProgram.Application.Abstractions.Users;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace EventProgram.Api.Authentication;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddEventProgramAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var googleClientId = configuration["Authentication:Google:ClientId"];
        var googleClientSecret = configuration["Authentication:Google:ClientSecret"];
        var googleIsConfigured = !string.IsNullOrWhiteSpace(googleClientId)
            && !string.IsNullOrWhiteSpace(googleClientSecret);

        services.AddSingleton(new GoogleAuthenticationStatus(googleIsConfigured));

        var authentication = services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "EventProgram.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });

        if (googleIsConfigured)
        {
            authentication.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = googleClientId!;
                options.ClientSecret = googleClientSecret!;
                options.CallbackPath = "/api/auth/google-callback";
                options.SaveTokens = false;
                options.Events.OnCreatingTicket = async context =>
                {
                    var googleSubject = context.User.GetProperty("sub").GetString();
                    var displayName = context.User.GetProperty("name").GetString();
                    var email = context.User.GetProperty("email").GetString();

                    if (string.IsNullOrWhiteSpace(googleSubject)
                        || string.IsNullOrWhiteSpace(displayName)
                        || string.IsNullOrWhiteSpace(email))
                    {
                        context.Fail("Google did not provide the required profile information.");
                        return;
                    }

                    var userAccountService = context.HttpContext.RequestServices
                        .GetRequiredService<IUserAccountService>();
                    var user = await userAccountService.FindOrCreateGoogleUserAsync(
                        googleSubject,
                        displayName,
                        email,
                        context.HttpContext.RequestAborted);

                    if (user.IsBlocked)
                    {
                        context.Fail("This account has been blocked.");
                        return;
                    }

                    if (context.Principal?.Identity is ClaimsIdentity identity)
                    {
                        identity.AddClaim(new Claim(EventProgramClaimTypes.UserId, user.Id.ToString()));
                    }
                };
            });
        }

        services.AddAuthorization();
        return services;
    }
}
