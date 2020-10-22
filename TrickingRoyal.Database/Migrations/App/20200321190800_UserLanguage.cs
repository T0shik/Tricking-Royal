using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class UserLanguage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "UserInformation",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "UserInformation");
        }
    }
}
