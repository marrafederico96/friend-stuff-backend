using FriendStuff.Data.Configurations;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Data;

public class FriendStuffDbContext(DbContextOptions<FriendStuffDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
