using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    expense_date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expenses", x => x.Id);
                    table.CheckConstraint("Expense_Amount_Positive", "amount > 0");
                    table.ForeignKey(
                        name: "FK_expenses_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expenses_users_payer_id",
                        column: x => x.payer_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "expense_participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    participant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expense_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_expense_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_expense_participants_expenses_expense_id",
                        column: x => x.expense_id,
                        principalTable: "expenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_expense_participants_users_participant_id",
                        column: x => x.participant_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_expense_participants_expense_id",
                table: "expense_participants",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "IX_expense_participants_participant_id",
                table: "expense_participants",
                column: "participant_id");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_event_id",
                table: "expenses",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_expenses_payer_id",
                table: "expenses",
                column: "payer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "expense_participants");

            migrationBuilder.DropTable(
                name: "expenses");
        }
    }
}
