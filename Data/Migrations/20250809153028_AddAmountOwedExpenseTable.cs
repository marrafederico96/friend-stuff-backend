using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAmountOwedExpenseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "amount_owed",
                table: "expense_participants",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount_owed",
                table: "expense_participants");
        }
    }
}
