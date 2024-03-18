using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using Hangfire;
using Octonica.ClickHouseClient;

namespace Uptimed.Shared;

public static class MonitoringService
{
    [Queue("default")]
    [DisplayName("JobId: {1}")]
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [2, 5, 10])]
    public static async Task GetMonitoringJobDoneAsync(MonitoringJob job)
    {
        HttpClient httpResourceClient = new();
        httpResourceClient.Timeout = TimeSpan.FromSeconds(job.RequestTimeout);

        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(job.RequestMethod),
            RequestUri = new Uri(job.Url),
            Content = new StringContent(job.RequestBody)
        };

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var response = await httpResourceClient.SendAsync(request);
        stopWatch.Stop();

        // Store response data in ClickHouse as logs
        ClickHouseConnection click = new("Host=localhost");
        await click.OpenAsync();
        await using var cmd =
            click.CreateCommand(
                "INSERT INTO monitoring_logs (`datetime`, `url`, `method`, `body`, `status_code`, `latency`)" +
                " VALUES (NOW(), {url}, {method}, {body}, {status_code}, {latency})");
        // cmd.Parameters.AddWithValue("dt", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("url", job.Url);
        cmd.Parameters.AddWithValue("method", job.RequestMethod);
        cmd.Parameters.AddWithValue("body", job.RequestBody);
        cmd.Parameters.AddWithValue("status_code", (int)response.StatusCode);
        cmd.Parameters.AddWithValue("latency", stopWatch.ElapsedMilliseconds);
        cmd.ExecuteNonQuery();

        Console.WriteLine($"{job.RequestMethod} {job.Url} => {response.StatusCode}");
    }
}