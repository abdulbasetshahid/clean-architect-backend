using EShop.Application.Contracts;
using EShop.Application.Models.Authentication;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EShop.Infrastructure.Authentication;

public sealed class FirebaseTokenVerifier : IFirebaseTokenVerifier
{
    private const string AppName = "eshop-customer-auth";

    private readonly FirebaseSettings _settings;
    private readonly ILogger<FirebaseTokenVerifier> _logger;
    private readonly Func<FirebaseSettings, GoogleCredential> _credentialFactory;
    private readonly string _appName;
    private readonly Lazy<FirebaseAuth?> _auth;

    public FirebaseTokenVerifier(IOptions<FirebaseSettings> settings, ILogger<FirebaseTokenVerifier> logger)
        : this(settings, logger, LoadCredential, AppName)
    {
    }

    internal FirebaseTokenVerifier(
        IOptions<FirebaseSettings> settings,
        ILogger<FirebaseTokenVerifier> logger,
        Func<FirebaseSettings, GoogleCredential> credentialFactory,
        string appName)
    {
        _settings = settings.Value;
        _logger = logger;
        _credentialFactory = credentialFactory;
        _appName = appName;
        _auth = new Lazy<FirebaseAuth?>(CreateAuth, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public async Task<FirebaseTokenInfo?> VerifyAsync(string idToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            return null;

        var auth = _auth.Value;
        if (auth is null)
            return null;

        FirebaseToken decoded;
        try
        {
            decoded = await auth.VerifyIdTokenAsync(idToken, cancellationToken);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogInformation("Firebase ID token rejected ({ErrorCode}).", ex.AuthErrorCode);
            return null;
        }
        catch (ArgumentException)
        {
            _logger.LogInformation("Firebase ID token rejected (malformed).");
            return null;
        }

        var expectedIssuer = $"https://securetoken.google.com/{_settings.ProjectId}";
        if (!string.Equals(decoded.Audience, _settings.ProjectId, StringComparison.Ordinal) ||
            !string.Equals(decoded.Issuer, expectedIssuer, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(decoded.Uid))
        {
            _logger.LogInformation("Firebase ID token rejected (wrong project).");
            return null;
        }

        decoded.Claims.TryGetValue("email", out var email);
        return new FirebaseTokenInfo(decoded.Uid, email as string);
    }

    private FirebaseAuth? CreateAuth()
    {
        if (string.IsNullOrWhiteSpace(_settings.ProjectId))
        {
            _logger.LogError("Firebase:ProjectId is not configured; customer authentication is disabled.");
            return null;
        }

        try
        {
            var app = FirebaseApp.GetInstance(_appName) ?? FirebaseApp.Create(new AppOptions
            {
                Credential = _credentialFactory(_settings),
                ProjectId = _settings.ProjectId
            }, _appName);

            return FirebaseAuth.GetAuth(app);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or ArgumentException)
        {
            _logger.LogError("Firebase credentials could not be loaded; customer authentication is disabled. {Reason}", ex.GetType().Name);
            return null;
        }
    }

    private static GoogleCredential LoadCredential(FirebaseSettings settings) =>
        string.IsNullOrWhiteSpace(settings.CredentialsPath)
            ? GoogleCredential.GetApplicationDefault()
            : CredentialFactory.FromFile<ServiceAccountCredential>(settings.CredentialsPath).ToGoogleCredential();
}
