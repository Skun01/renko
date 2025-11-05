using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using project_z_backend.Entities;

namespace project_z_backend.Data;

public class ProjectZContext : DbContext
{
    public DbSet<User> Users { set; get; }
    public DbSet<Role> Roles { set; get; }
    public ProjectZContext(DbContextOptions<ProjectZContext> options) : base(options) { }
    
    // Custom SaveChangesAsync
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
