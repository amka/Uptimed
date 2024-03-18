using System.Data;
using System.Diagnostics;
using Octonica.ClickHouseClient;
using Uptimed.Shared;
using Uptimed.Shared.Models;

namespace Uptimed.Worker.Jobs;

internal class CallMonitoringJob(HttpClient client, ClickHouseConnection chConnection)
    : ICallMonitoringJob
{
    private const string UserAgent = "Mozilla/5.0 (compatible; UptimedBot/0.1; +https://uptimed.ru/bots)";

    public async Task ExecuteAsync(MonitoringJob job)
    {
        if (chConnection.State != ConnectionState.Open)
        {
            await chConnection.OpenAsync();
        }

        var (stopWatch, response) = await MakeRequest(job);

        // Store response data in ClickHouse as logs
        var jobLog = new MonitoringJobLog
        {
            DateTime = DateTime.UtcNow,
            Url = job.Url,
            UserAgent = UserAgent,
            Method = job.RequestMethod,
            Body = job.RequestBody,
            ResponseTime = stopWatch.Elapsed.Milliseconds,
            StatusCode = (int)response.StatusCode,
        };
        await SaveLogAsync(jobLog);

        Console.WriteLine($"{job.RequestMethod} {job.Url} => HTTP{response.StatusCode}: {stopWatch.Elapsed}ms");
    }

    private async Task SaveLogAsync(MonitoringJobLog jobLog)
    {
        await using var cmd = chConnection.CreateCommand(
            "INSERT INTO monitoring_logs (`datetime`, `url`, `method`, `body`, `status_code`, `latency`, `user_agent`)" +
            " VALUES (NOW(), {url}, {method}, {body}, {status_code}, {latency}, {user_agent})");
        cmd.Parameters.AddWithValue("dt", jobLog.DateTime);
        cmd.Parameters.AddWithValue("url", jobLog.Url);
        cmd.Parameters.AddWithValue("method", jobLog.Method);
        cmd.Parameters.AddWithValue("body", jobLog.Body);
        cmd.Parameters.AddWithValue("status_code", jobLog.StatusCode);
        cmd.Parameters.AddWithValue("latency", jobLog.ResponseTime);
        cmd.Parameters.AddWithValue("user_agent", jobLog.UserAgent ?? UserAgent);
        cmd.ExecuteNonQuery();
    }

    private async Task<(Stopwatch stopWatch, HttpResponseMessage response)> MakeRequest(MonitoringJob job)
    {
        client.Timeout = TimeSpan.FromSeconds(job.RequestTimeout);

        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(job.RequestMethod),
            RequestUri = new Uri(job.Url),
            Content = new StringContent(job.RequestBody),
            Headers = { { "user-agent", job.UserAgent ?? UserAgent } }
        };

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var response = await client.SendAsync(request);
        stopWatch.Stop();
        return (stopWatch, response);
    }
}