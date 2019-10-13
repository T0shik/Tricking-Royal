using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class notificationUserIdFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NotificationConfigurations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "NotificationConfigurations",
                nullable: true);
        }
    }
}
