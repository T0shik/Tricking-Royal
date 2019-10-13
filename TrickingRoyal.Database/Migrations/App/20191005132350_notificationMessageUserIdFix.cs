using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class notificationMessageUserIdFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "NotificationMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "NotificationMessages",
                nullable: true);
        }
    }
}
