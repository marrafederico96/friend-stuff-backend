using FriendStuff.Data.Configurations;
using FriendStuff.Domain.Entities;
using FriendStuff.Features.Activities.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FriendStuff.Data;

public class FriendStuffDbContext(DbContextOptions<FriendStuffDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserActivity> UsersActivities { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<UserExpense> UsersExpenses { get; set; }
    public DbSet<ActivityType> ActivityTypes { get; set; }

    // View
    public DbSet<ActivityTypesResponse> ActivityTypesResponse { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserActivityConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityConfiguration());
        modelBuilder.ApplyConfiguration(new ExpenseConfguration());
        modelBuilder.ApplyConfiguration(new UserExpenseConfiguration());
        modelBuilder.ApplyConfiguration(new ActivityTypeConfiguration());

        modelBuilder.Entity<ActivityTypesResponse>(mb =>
        {
            mb.HasNoKey();
            mb.ToView("GetActivityTypes");
        });

    }
}
