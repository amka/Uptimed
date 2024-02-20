using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using Uptimed.Data;

namespace Uptimed.Models;

[Index(nameof(Alias), IsUnique = true)]
public class Monitor
{
    [MaxLength(32)] public string Id { get; set; } = Nanoid.Generate();

    [MaxLength(4096)] public required string Url { get; set; }

    [MaxLength(256)] public required string Alias { get; set; }

    public string RequestMethod { get; set; } = "GET";
    public string RequestBody { get; set; } = "";
    public int RequestTimeout { get; set; } = 30;

    public required ApplicationUser Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}