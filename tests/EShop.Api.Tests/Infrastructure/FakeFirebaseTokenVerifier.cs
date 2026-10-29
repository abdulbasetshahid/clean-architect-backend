using EShop.Application.Contracts;

namespace EShop.Api.Tests.Infrastructure;

/// <summary>
/// Stands in for Firebase so tests never need real credentials. Only the listed tokens are "verified";
/// everything else (invalid, expired, wrong-project, admin JWTs) is rejected like the real verifier would.
/// </summary>
public sealed class FakeFirebaseTokenVerifier : IFirebaseTokenVerifier
{
    public const string CustomerAToken = "firebase-token-customer-a";
    public const string CustomerBToken = "firebase-token-customer-b";
    public const string CustomerAUid = "firebase-uid-a";
    public const string CustomerBUid = "firebase-uid-b";

    public Task<FirebaseTokenInfo?> VerifyAsync(string idToken, CancellationToken cancellationToken)
    {
        FirebaseTokenInfo? result = idToken switch
        {
            CustomerAToken => new FirebaseTokenInfo(CustomerAUid, "a@example.com"),
            CustomerBToken => new FirebaseTokenInfo(CustomerBUid, "b@example.com"),
            _ => null
        };
        return Task.FromResult(result);
    }
}
