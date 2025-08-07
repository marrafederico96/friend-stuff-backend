using FriendStuffBackend.Data.Configuration;
using FriendStuffBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendStuffBackend.Data;

public class FriendStuffDbContext(DbContextOptions<FriendStuffDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { set; get; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventUser> EventUsers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
    }
}