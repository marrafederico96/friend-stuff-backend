using FriendStuff.Data.Configurations;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Data;

public class FriendStuffDbContext(DbContextOptions<FriendStuffDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserActivity> UsersActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserActivityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityConfiguration());
    }
}
