using EShop.Application.Models.Authentication;

namespace EShop.Application.Contracts;

public interface IAuthenticationService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    Task<AuthenticationResponse> GoogleAuthenticateAsync(GoogleAuthRequest request);
    Task<RegistrationResponse> RegisterAsync(RegistrationRequest request);
}
