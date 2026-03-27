using System;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendStuff.Data.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.HasKey(ua => new { ua.UserId, ua.ActivityId });

        builder.HasOne(ua => ua.User)
            .WithMany(u => u.Activities)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(ua => ua.Activity)
        .WithMany(a => a.Participants)
        .HasForeignKey(ua => ua.ActivityId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}
