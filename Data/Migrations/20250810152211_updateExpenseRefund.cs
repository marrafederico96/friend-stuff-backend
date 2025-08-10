using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateExpenseRefund : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_refund_users_debtor_id",
                table: "expense_refund");

            migrationBuilder.AddColumn<Guid>(
                name: "payer_id",
                table: "expense_refund",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_expense_refund_payer_id",
                table: "expense_refund",
                column: "payer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_refund_users_debtor_id",
                table: "expense_refund",
                column: "debtor_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_expense_refund_users_payer_id",
                table: "expense_refund",
                column: "payer_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_expense_refund_users_debtor_id",
                table: "expense_refund");

            migrationBuilder.DropForeignKey(
                name: "FK_expense_refund_users_payer_id",
                table: "expense_refund");

            migrationBuilder.DropIndex(
                name: "IX_expense_refund_payer_id",
                table: "expense_refund");

            migrationBuilder.DropColumn(
                name: "payer_id",
                table: "expense_refund");

            migrationBuilder.AddForeignKey(
                name: "FK_expense_refund_users_debtor_id",
                table: "expense_refund",
                column: "debtor_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
