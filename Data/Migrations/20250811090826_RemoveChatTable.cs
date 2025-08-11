using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FriendStuffBackend.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_chats_chat_id",
                table: "messages");

            migrationBuilder.DropTable(
                name: "chats");

            migrationBuilder.RenameColumn(
                name: "chat_id",
                table: "messages",
                newName: "event_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_chat_id",
                table: "messages",
                newName: "IX_messages_event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_events_event_id",
                table: "messages",
                column: "event_id",
                principalTable: "events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_messages_events_event_id",
                table: "messages");

            migrationBuilder.RenameColumn(
                name: "event_id",
                table: "messages",
                newName: "chat_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_event_id",
                table: "messages",
                newName: "IX_messages_chat_id");

            migrationBuilder.CreateTable(
                name: "chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chats_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chats_event_id",
                table: "chats",
                column: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_messages_chats_chat_id",
                table: "messages",
                column: "chat_id",
                principalTable: "chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
