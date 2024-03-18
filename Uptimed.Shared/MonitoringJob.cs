namespace Uptimed.Shared;

public class MonitoringJob
{
    public string Url { get; set; }
    public string RequestMethod { get; set; }
    public string RequestBody { get; set; }
    public int RequestTimeout { get; set; }
    public string? UserAgent { get; set; } = null;
}