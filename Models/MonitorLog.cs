using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;

namespace Uptimed.Models;

[Index(nameof(Monitor.Id), nameof(CreatedAt), IsUnique = true)]
public class MonitorLog
{
    [MaxLength(32)]
    public string Id { get; set; } = Nanoid.Generate();
    
    public required Monitor Monitor { get; set; }
    public int StatusCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}