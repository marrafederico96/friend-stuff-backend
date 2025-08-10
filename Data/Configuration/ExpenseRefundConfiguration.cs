using FriendStuffBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendStuffBackend.Data.Configuration;

public class ExpenseRefundConfiguration : IEntityTypeConfiguration<ExpenseRefund>
{
    public void Configure(EntityTypeBuilder<ExpenseRefund> builder)
    {
        builder.ToTable(t => t.HasCheckConstraint("Expense_Refund_Amount_Positive", "amount_refund > 0"));

        builder.HasOne(ex => ex.Debtor)
            .WithMany(deb => deb.ExpenseAsDebtor)
            .HasForeignKey(ex => ex.DebtorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ex => ex.Payer)
            .WithMany(deb => deb.ExpenseAsPayer)
            .HasForeignKey(ex => ex.PayerId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}