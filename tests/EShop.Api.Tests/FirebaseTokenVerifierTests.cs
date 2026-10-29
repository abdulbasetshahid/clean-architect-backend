using System.Text;
using System.Text.Json;
using EShop.Application.Models.Authentication;
using EShop.Infrastructure.Authentication;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EShop.Api.Tests;

/// <summary>
/// Exercises the real Firebase Admin SDK verification path. Claim checks run before the signature
/// check, so these rejections happen offline and without production credentials.
/// </summary>
public class FirebaseTokenVerifierTests
{
    private const string ProjectId = "eshop-verifier-test";

    private static readonly FirebaseTokenVerifier Verifier = new(
        Options.Create(new FirebaseSettings { ProjectId = ProjectId }),
        NullLogger<FirebaseTokenVerifier>.Instance,
        _ => GoogleCredential.FromAccessToken("test-access-token"),
        "eshop-verifier-tests");

    [Fact]
    public async Task MalformedToken_IsRejected()
    {
        Assert.Null(await Verifier.VerifyAsync("not-a-jwt", CancellationToken.None));
        Assert.Null(await Verifier.VerifyAsync("", CancellationToken.None));
    }

    [Fact]
    public async Task WrongProjectToken_IsRejected()
    {
        var token = UnsignedToken(audience: "another-project", issuer: "https://securetoken.google.com/another-project");

        Assert.Null(await Verifier.VerifyAsync(token, CancellationToken.None));
    }

    [Fact]
    public async Task WrongIssuerToken_IsRejected()
    {
        var token = UnsignedToken(audience: ProjectId, issuer: "https://evil.example.com");

        Assert.Null(await Verifier.VerifyAsync(token, CancellationToken.None));
    }

    [Fact]
    public async Task ExpiredToken_IsRejected()
    {
        var token = UnsignedToken(
            audience: ProjectId,
            issuer: $"https://securetoken.google.com/{ProjectId}",
            expiresAt: DateTimeOffset.UtcNow.AddHours(-1));

        Assert.Null(await Verifier.VerifyAsync(token, CancellationToken.None));
    }

    [Fact]
    public async Task MissingProjectId_RejectsEverything()
    {
        var verifier = new FirebaseTokenVerifier(
            Options.Create(new FirebaseSettings { ProjectId = "" }),
            NullLogger<FirebaseTokenVerifier>.Instance);

        var token = UnsignedToken(audience: ProjectId, issuer: $"https://securetoken.google.com/{ProjectId}");

        Assert.Null(await verifier.VerifyAsync(token, CancellationToken.None));
    }

    private static string UnsignedToken(string audience, string issuer, DateTimeOffset? expiresAt = null)
    {
        var now = DateTimeOffset.UtcNow;
        var exp = expiresAt ?? now.AddHours(1);
        var header = new { alg = "RS256", kid = "test-key", typ = "JWT" };
        var payload = new
        {
            aud = audience,
            iss = issuer,
            sub = "uid-123",
            iat = exp.AddHours(-1).ToUnixTimeSeconds(),
            exp = exp.ToUnixTimeSeconds(),
            auth_time = exp.AddHours(-1).ToUnixTimeSeconds()
        };

        return $"{Encode(header)}.{Encode(payload)}.{Base64Url(Encoding.UTF8.GetBytes("invalid-signature"))}";
    }

    private static string Encode(object value) => Base64Url(JsonSerializer.SerializeToUtf8Bytes(value));

    private static string Base64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
