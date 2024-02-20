namespace Uptimed.Models.Response;

public class AuthResponse
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
}