using System;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendStuff.Data.Configurations;

public class ExpenseConfguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(p => p.Amount).HasPrecision(18, 2);

        builder.HasOne(a => a.Payer)
        .WithMany(p => p.PaidExpenses)
        .HasForeignKey(a => a.PayerId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(a => a.Activity)
            .WithMany(a => a.Expenses)
            .HasForeignKey(a => a.ActivityId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
