using Hangfire;
using Uptimed.Shared;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Services;

public class MonitoringJobService
{
    public void AddOrUpdateJob(Monitor monitor)
    {
        var job = new MonitoringJob
        {
            Url = monitor.Url,
            RequestMethod = monitor.RequestMethod,
            RequestBody = monitor.RequestBody,
            RequestTimeout = monitor.RequestTimeout,
            UserAgent = monitor.UserAgent,
        };

        // // Create a new recurring job to call the monitoring service
        RecurringJob.AddOrUpdate(monitor.JobId, () => MonitoringService.GetMonitoringJobDoneAsync(job), Cron.Minutely);
    }

    public void RemoveMonitoringJob(string jobId)
    {
        RecurringJob.RemoveIfExists(jobId);
    }
}