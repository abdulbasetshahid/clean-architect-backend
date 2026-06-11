namespace EShop.Application.Models.Authentication;

public class AuthenticationResponse
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
