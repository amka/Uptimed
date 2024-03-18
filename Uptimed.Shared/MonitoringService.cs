using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using Hangfire;
using Octonica.ClickHouseClient;

namespace Uptimed.Shared;

public static class MonitoringService
{
    public static string UserAgent =
        "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/ Mobile Safari/537.36 (compatible; UptimedBot/0.1; +https://uptimed.ru/bot.html)";

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
            Content = new StringContent(job.RequestBody),
            Headers = { { "user-agent", job.UserAgent ?? UserAgent} }
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
                "INSERT INTO monitoring_logs (`datetime`, `url`, `method`, `body`, `status_code`, `latency`, `user_agent`) VALUES" +
                " VALUES (NOW(), {url}, {method}, {body}, {status_code}, {latency}, {user_agent})");
        // cmd.Parameters.AddWithValue("dt", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("url", job.Url);
        cmd.Parameters.AddWithValue("method", job.RequestMethod);
        cmd.Parameters.AddWithValue("body", job.RequestBody);
        cmd.Parameters.AddWithValue("status_code", (int)response.StatusCode);
        cmd.Parameters.AddWithValue("latency", stopWatch.ElapsedMilliseconds);
        cmd.Parameters.AddWithValue("user_agent", job.UserAgent?? UserAgent);
        cmd.ExecuteNonQuery();

        Console.WriteLine($"{job.RequestMethod} {job.Url} => {response.StatusCode}");
    }
}