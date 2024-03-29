namespace Uptimed.Models.Response;

public class UserResponse
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
}