using System.ComponentModel.DataAnnotations;

namespace EShop.Application.Models.Authentication;

public class GoogleAuthRequest
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
}
