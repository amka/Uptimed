using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using NanoidDotNet;

namespace Uptimed.Models;

public class UptimedRole : IdentityRole
{
    [MaxLength(32)]
    public string Id { get; set; } = Nanoid.Generate();
}