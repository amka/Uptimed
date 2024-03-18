using Hangfire;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using Uptimed.Data;
using Uptimed.Models;
using Uptimed.Shared;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Services;

public class MonitorService(UptimedDbContext db, ILogger<MonitorService> logger, MonitoringJobService jobService)
{
    /// <summary>
    /// Returns a list of monitors for the given user with pagination.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async Task<List<Monitor>> GetUserMonitorsAsync(ApplicationUser user, int page = 0, int pageSize = 10)
    {
        return await db.Monitors.Where(x => x.Owner == user)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Returns the monitor with the given id for the given user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Monitor?> GetUserMonitorAsync(ApplicationUser user, string id)
    {
        return await db.Monitors.FirstOrDefaultAsync(x => x.Owner == user && x.Id == id);
    }

    /// <summary>
    /// Returns the number of monitors for the given user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<int> GetUserMonitorsCountAsync(ApplicationUser user)
    {
        return await db.Monitors.CountAsync(x => x.Owner == user);
    }


    /// <summary>
    /// Creates a monitor asynchronously and adds it to the database.
    /// It will be added to the recurring job.
    /// If you need to pause the job, use <see cref="UpdateMonitorAsync"/>
    /// </summary>
    /// <param name="monitor"></param>
    /// <returns></returns>
    public async Task<Monitor> CreateMonitorAsync(Monitor monitor)
    {
        monitor.CreatedAt = DateTime.UtcNow;
        monitor.UpdatedAt = DateTime.UtcNow;
        await db.Monitors.AddAsync(monitor);

        monitor.JobId ??= await Nanoid.GenerateAsync(size: 32);

        jobService.AddOrUpdateJob(monitor);

        await db.SaveChangesAsync();
        return monitor;
    }

    /// <summary>
    /// Updates a monitor asynchronously.
    /// If the monitor is disabled, it will be removed from the recurring job.
    /// Otherwise, it will be added to the recurring job.
    /// </summary>
    /// <param name="monitor">The monitor to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateMonitorAsync(Monitor monitor)
    {
        monitor.UpdatedAt = DateTime.UtcNow;
        db.Monitors.Update(monitor);
        await db.SaveChangesAsync();

        monitor.JobId ??= await Nanoid.GenerateAsync(size: 32);

        if (monitor.IsEnabled)
        {
            jobService.AddOrUpdateJob(monitor);
        }
        else
        {
            jobService.RemoveMonitoringJob(monitor.JobId);
        }
    }

    /// <summary>
    /// Deletes a monitor asynchronously.
    /// Removes the monitor from the database.
    /// If the monitor is associated with a job, also removes the monitoring job.
    /// </summary>
    /// <param name="monitor"></param>
    public async Task DeleteMonitorAsync(Monitor monitor)
    {
        db.Monitors.Remove(monitor);
        await db.SaveChangesAsync();

        if (monitor.JobId != null)
        {
            jobService.RemoveMonitoringJob(monitor.JobId);
        }
    }
}