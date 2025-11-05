using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using project_z_backend.Entities;
using project_z_backend.Share.Constants;

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

        // Seeding Role data
        DateTime seedDate = new DateTime(2025, 11, 5, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Role>().HasData([
            new Role {
                Id = Guid.Parse(RoleConstants.UserRoleId),
                Name = RoleConstants.UserRoleName
            },
            new Role
            {
                Id = Guid.Parse(RoleConstants.ContributorRoleId), 
                Name = RoleConstants.ContributorRoleName,       
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role
            {
                Id = Guid.Parse(RoleConstants.TeacherRoleId),
                Name = RoleConstants.TeacherRoleName,         
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            },
            new Role
            {
                Id = Guid.Parse(RoleConstants.AdminRoleId), 
                Name = RoleConstants.AdminRoleName,        
                CreatedAt = seedDate,
                UpdatedAt = seedDate
            }
        ]);
    }
}
