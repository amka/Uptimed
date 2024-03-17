using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace Uptimed.Models;

[Index(nameof(Alias), nameof(OwnerId), IsUnique = true)]
public class Monitor
{
    [MaxLength(32)] public string Id { get; set; } = Nanoid.Generate();

    [MaxLength(4096)] public required string Url { get; set; }

    [MaxLength(256)] public required string Alias { get; set; }

    public string RequestMethod { get; set; } = "GET";
    public string RequestBody { get; set; } = "";
    public int RequestTimeout { get; set; } = 30;

    public string OwnerId { get; set; }
    public required ApplicationUser Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public string? JobId { get; set; } = Nanoid.Generate(size: 32);
}