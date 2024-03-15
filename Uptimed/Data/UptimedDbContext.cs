using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uptimed.Models;
using Monitor = Uptimed.Models.Monitor;

namespace Uptimed.Data;

public class UptimedDbContext : IdentityDbContext<ApplicationUser, UptimedRole, string>
{
    public UptimedDbContext(DbContextOptions<UptimedDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }

    public DbSet<Monitor> Monitors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    // public DbSet<MonitorLog> MonitorLogs { get; set; }
}