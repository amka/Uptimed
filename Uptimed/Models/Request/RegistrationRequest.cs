using System.ComponentModel.DataAnnotations;

namespace Uptimed.Models;

public class RegistrationRequest
{
    [Required] public required string Email { get; set; }

    [Required] public required string Password { get; set; }
}