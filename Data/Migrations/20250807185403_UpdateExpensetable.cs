using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateExpensetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "expense_date",
                table: "expenses",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "expense_name",
                table: "expenses",
                type: "text",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expense_name",
                table: "expenses");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "expense_date",
                table: "expenses",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
