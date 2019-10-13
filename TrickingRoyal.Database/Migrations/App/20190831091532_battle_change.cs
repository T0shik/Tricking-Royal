using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class battle_change : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnHours",
                table: "Matches");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurnHours",
                table: "Matches",
                nullable: false,
                defaultValue: 0);
        }
    }
}
