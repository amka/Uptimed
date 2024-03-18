using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using NanoidDotNet;

namespace Uptimed.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(32)]
    public string Id { get; set; } = Nanoid.Generate();
    
    public IEnumerable<Todo> Todos { get; set; }
}