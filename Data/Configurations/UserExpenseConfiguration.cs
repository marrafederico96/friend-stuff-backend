using System;
using FriendStuff.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendStuff.Data.Configurations;

public class UserExpenseConfiguration : IEntityTypeConfiguration<UserExpense>
{
    public void Configure(EntityTypeBuilder<UserExpense> builder)
    {

        builder.Property(p => p.AmountOwed).HasPrecision(18, 2);

        builder.HasOne(ue => ue.Expense)
            .WithMany(e => e.Participants)
            .HasForeignKey(ue => ue.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ue => ue.Debtor)
            .WithMany(d => d.OwedExpenses)
            .HasForeignKey(ue => ue.DebtorId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
