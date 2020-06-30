using Microsoft.EntityFrameworkCore.Migrations;

namespace GotIt.MSSQL.Migrations
{
    public partial class RequestTitleAndContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_UserId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_UserId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Requests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReceiverId",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SenderId",
                table: "Requests",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Requests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Notifications",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_SenderId",
                table: "Requests",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_ReceiverId",
                table: "Requests",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_SenderId",
                table: "Requests",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_ReceiverId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_SenderId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ReceiverId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_SenderId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Requests");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Requests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SenderId",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_UserId",
                table: "Requests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_UserId",
                table: "Requests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
