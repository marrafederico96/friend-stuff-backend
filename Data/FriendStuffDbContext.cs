using FriendStuff.Data.Configurations;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Data;

public class FriendStuffDbContext(DbContextOptions<FriendStuffDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
