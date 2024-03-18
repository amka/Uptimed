using Hangfire;
using Uptimed.Shared;
using Uptimed.Shared.Models;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Services;

public class MonitoringJobService(IRecurringJobManager recurringJobManager)
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
        recurringJobManager.AddOrUpdate<ICallMonitoringJob>(
            monitor.JobId,
            x => x.ExecuteAsync(job),
            Cron.Minutely
        );
    }

    public void RemoveMonitoringJob(string jobId)
    {
        recurringJobManager.RemoveIfExists(jobId);
    }
}