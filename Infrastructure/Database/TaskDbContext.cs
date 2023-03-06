using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class TaskDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public TaskDbContext(DbContextOptions<TaskDbContext> options)
      : base(options)
    {
    }

    public TaskDbContext() { }

    public DbSet<UserTask>? Tasks { get; set; }
    public DbSet<TaskFile>? TaskFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<AppUser>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<AppRole>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<UserTask>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();

        modelBuilder
            .Entity<TaskFile>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd();
    }

}
