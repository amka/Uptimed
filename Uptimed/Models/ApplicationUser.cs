using Microsoft.AspNetCore.Identity;

namespace Uptimed.Models;

public class ApplicationUser : IdentityUser
{
    public IEnumerable<Todo> Todos { get; set; }
}