using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixExpenseRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_refund_expenses_expense_id",
                table: "expense_refund");

            migrationBuilder.DropIndex(
                name: "IX_expense_refund_expense_id",
                table: "expense_refund");

            migrationBuilder.DropColumn(
                name: "expense_id",
                table: "expense_refund");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "expense_id",
                table: "expense_refund",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_expense_refund_expense_id",
                table: "expense_refund",
                column: "expense_id");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_refund_expenses_expense_id",
                table: "expense_refund",
                column: "expense_id",
                principalTable: "expenses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
