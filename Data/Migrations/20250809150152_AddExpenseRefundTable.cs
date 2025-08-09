using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseRefundTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expense_refund",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    refund_date = table.Column<DateOnly>(type: "date", nullable: false),
                    debtor_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_refund = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_refund", x => x.Id);
                    table.CheckConstraint("Expense_Refund_Amount_Positive", "amount_refund > 0");
                    table.ForeignKey(
                        name: "FK_expense_refund_expenses_expense_id",
                        column: x => x.expense_id,
                        principalTable: "expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expense_refund_users_debtor_id",
                        column: x => x.debtor_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_expense_refund_debtor_id",
                table: "expense_refund",
                column: "debtor_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_refund_expense_id",
                table: "expense_refund",
                column: "expense_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_refund");
        }
    }
}
