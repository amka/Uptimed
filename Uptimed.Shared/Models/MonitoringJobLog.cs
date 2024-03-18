using System.ComponentModel.DataAnnotations;

namespace Uptimed.Shared.Models;

public class MonitoringJobLog
{
    [Required] public string Url { get; set; }
    public string? Method { get; set; }
    public string? Body { get; set; }
    public int ResponseTime { get; set; }
    public string UserAgent { get; set; } = String.Empty;
    public int StatusCode { get; set; }
    public DateTime DateTime { get; set; }
}