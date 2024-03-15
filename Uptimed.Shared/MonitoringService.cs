using System.ComponentModel;
using Hangfire;

namespace Uptimed.Shared;

public class MonitoringService
{
    [Queue("default")]
    [DisplayName("JobId: {1}")]
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = [2, 5, 10])]
    public static async Task GetMonitoringJobDoneAsync(MonitoringJob job)
    {
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(job.RequestTimeout);

        var request = new HttpRequestMessage
        {
            Method = new HttpMethod(job.RequestMethod),
            RequestUri = new Uri(job.Url),
            Content = new StringContent(job.RequestBody)
        };

        var response = await client.SendAsync(request);

        Console.WriteLine($"{job.RequestMethod} {job.Url} => {response.StatusCode}");
    }
}