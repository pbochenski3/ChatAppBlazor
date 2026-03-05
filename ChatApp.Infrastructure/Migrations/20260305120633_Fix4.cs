using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Fix4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_ChatLogs_ChatLogChatID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_SenderUserID",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "ChatLogUsers");

            migrationBuilder.DropTable(
                name: "ChatLogs");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatLogChatID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatLogChatID",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderUserID",
                table: "Messages",
                newName: "SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderUserID",
                table: "Messages",
                newName: "IX_Messages_SenderID");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
                name: "FK_Messages_Users_SenderID",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "Messages",
                newName: "SenderUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_SenderID",
                table: "Messages",
                newName: "IX_Messages_SenderUserID");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<int>(
                name: "ChatLogChatID",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatLogs",
                columns: table => new
                {
                    ChatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLogs", x => x.ChatID);
                });

            migrationBuilder.CreateTable(
                name: "ChatLogUsers",
                columns: table => new
                {
                    ChatLogsChatID = table.Column<int>(type: "int", nullable: false),
                    UsersUserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatLogUsers", x => new { x.ChatLogsChatID, x.UsersUserID });
                    table.ForeignKey(
                        name: "FK_ChatLogUsers_ChatLogs_ChatLogsChatID",
                        column: x => x.ChatLogsChatID,
                        principalTable: "ChatLogs",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatLogUsers_Users_UsersUserID",
                        column: x => x.UsersUserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatLogChatID",
                table: "Messages",
                column: "ChatLogChatID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatLogUsers_UsersUserID",
                table: "ChatLogUsers",
                column: "UsersUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_ChatLogs_ChatLogChatID",
                table: "Messages",
                column: "ChatLogChatID",
                principalTable: "ChatLogs",
                principalColumn: "ChatID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_SenderUserID",
                table: "Messages",
                column: "SenderUserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
