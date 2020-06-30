using Microsoft.EntityFrameworkCore.Migrations;

namespace GotIt.MSSQL.Migrations
{
    public partial class AddScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "ProbablyMatches",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "ProbablyMatches");
        }
    }
}
