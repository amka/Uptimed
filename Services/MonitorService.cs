using Microsoft.EntityFrameworkCore;
using Uptimed.Data;
using Uptimed.Models;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Services;

public class MonitorService(UptimedDbContext db, ILogger<MonitorService> logger)
{
    public async Task<List<Monitor>> GetUserMonitorsAsync(ApplicationUser user, int page = 0, int pageSize = 10)
    {
        return await db.Monitors.Where(x => x.Owner == user)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetUserMonitorsCountAsync(ApplicationUser user)
    {
        return await db.Monitors.CountAsync(x => x.Owner == user);
    }


    public async Task<Monitor> CreateMonitorAsync(Monitor monitor)
    {
        await db.Monitors.AddAsync(monitor);
        await db.SaveChangesAsync();

        return monitor;
    }
}