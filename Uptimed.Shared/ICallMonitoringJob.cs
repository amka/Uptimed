using Uptimed.Shared.Models;

namespace Uptimed.Shared;

public interface ICallMonitoringJob
{
    Task ExecuteAsync(MonitoringJob job);
}