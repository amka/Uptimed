using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Uptimed.Models;

namespace Uptimed.Data;

public class UptimedDbContext : IdentityDbContext<ApplicationUser, UptimedRole, string>
{
    public UptimedDbContext(DbContextOptions<UptimedDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Todo> Todos { get; set; }
}