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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
