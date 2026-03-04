using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_ChatLogs_ChatLogChatID",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Message_Users_SenderID",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Message",
                table: "Message");

            migrationBuilder.RenameTable(
                name: "Message",
                newName: "Messages");

            migrationBuilder.RenameIndex(
                name: "IX_Message_SenderID",
                table: "Messages",
                newName: "IX_Messages_SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ChatLogChatID",
                table: "Messages",
                newName: "IX_Messages_ChatLogChatID");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Messages",
                table: "Messages",
                column: "MessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatLogs_ChatLogChatID",
                table: "Messages",
                column: "ChatLogChatID",
                principalTable: "ChatLogs",
                principalColumn: "ChatID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderID",
                table: "Messages",
                column: "SenderID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatLogs_ChatLogChatID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderID",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Messages",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Messages",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderID",
                table: "Message",
                newName: "IX_Message_SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChatLogChatID",
                table: "Message",
                newName: "IX_Message_ChatLogChatID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Message",
                table: "Message",
                column: "MessageID");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_ChatLogs_ChatLogChatID",
                table: "Message",
                column: "ChatLogChatID",
                principalTable: "ChatLogs",
                principalColumn: "ChatID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Users_SenderID",
                table: "Message",
                column: "SenderID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
