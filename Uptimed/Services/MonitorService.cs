using Hangfire;
using Microsoft.EntityFrameworkCore;
using Uptimed.Data;
using Uptimed.Models;
using Uptimed.Shared;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Services;

public class MonitorService(UptimedDbContext db, ILogger<MonitorService> logger, IBackgroundJobClient jobClient)
{
    public async Task<List<Monitor>> GetUserMonitorsAsync(ApplicationUser user, int page = 0, int pageSize = 10)
    {
        return await db.Monitors.Where(x => x.Owner == user)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Monitor?> GetUserMonitorAsync(ApplicationUser user, string id)
    {
        return await db.Monitors.FirstOrDefaultAsync(x => x.Owner == user && x.Id == id);
    }

    public async Task<int> GetUserMonitorsCountAsync(ApplicationUser user)
    {
        return await db.Monitors.CountAsync(x => x.Owner == user);
    }


    public async Task<Monitor> CreateMonitorAsync(Monitor monitor)
    {
        monitor.CreatedAt = DateTime.UtcNow;
        monitor.UpdatedAt = DateTime.UtcNow;
        await db.Monitors.AddAsync(monitor);
        await db.SaveChangesAsync();

        jobClient.Enqueue(() => MonitoringService.GetMonitoringJobDoneAsync(
            new MonitoringJob
            {
                Url = monitor.Url,
                RequestMethod = monitor.RequestMethod,
                RequestBody = monitor.RequestBody,
                RequestTimeout = monitor.RequestTimeout
            }
        ));

        return monitor;
    }

    public async Task UpdateMonitorAsync(Monitor monitor)
    {
        monitor.UpdatedAt = DateTime.UtcNow;
        db.Monitors.Update(monitor);
        await db.SaveChangesAsync();
    }
}