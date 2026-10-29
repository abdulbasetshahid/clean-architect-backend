namespace EShop.Application.Contracts;

public interface IFirebaseTokenVerifier
{
    /// <summary>
    /// Verifies a Firebase ID token (signature, issuer, audience, expiry and project).
    /// Returns null when the token is not valid for the configured Firebase project.
    /// </summary>
    Task<FirebaseTokenInfo?> VerifyAsync(string idToken, CancellationToken cancellationToken);
}

public sealed record FirebaseTokenInfo(string Uid, string? Email);
