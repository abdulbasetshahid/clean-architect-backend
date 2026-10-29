using System.Security.Claims;
using System.Text.Encodings.Web;
using EShop.Application.Contracts;
using EShop.Application.Models.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace EShop.Api.Authentication;

public sealed class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string BearerPrefix = "Bearer ";

    private readonly IFirebaseTokenVerifier _tokenVerifier;

    public FirebaseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IFirebaseTokenVerifier tokenVerifier)
        : base(options, logger, encoder)
    {
        _tokenVerifier = tokenVerifier;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? header = Request.Headers[HeaderNames.Authorization];
        if (string.IsNullOrEmpty(header) || !header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        var token = header[BearerPrefix.Length..].Trim();
        if (token.Length == 0)
            return AuthenticateResult.Fail("Invalid customer token.");

        var verified = await _tokenVerifier.VerifyAsync(token, Context.RequestAborted);
        if (verified is null)
            return AuthenticateResult.Fail("Invalid customer token.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, verified.Uid),
            new(CustomerAuthenticationDefaults.FirebaseUidClaimType, verified.Uid)
        };
        if (!string.IsNullOrWhiteSpace(verified.Email))
            claims.Add(new Claim(ClaimTypes.Email, verified.Email));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties) =>
        WriteProblemAsync(
            StatusCodes.Status401Unauthorized,
            "Unauthorized",
            "Customer authentication is required or token is invalid.");

    protected override Task HandleForbiddenAsync(AuthenticationProperties properties) =>
        WriteProblemAsync(
            StatusCodes.Status403Forbidden,
            "Forbidden",
            "You are not allowed to access this resource.");

    private Task WriteProblemAsync(int status, string title, string detail)
    {
        if (Response.HasStarted)
            return Task.CompletedTask;

        Response.StatusCode = status;
        return Response.WriteAsJsonAsync(
            new { status, title, detail },
            options: null,
            contentType: "application/problem+json");
    }
}
