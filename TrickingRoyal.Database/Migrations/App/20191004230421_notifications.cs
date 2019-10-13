using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropColumn(
                name: "BrowserNotifications",
                table: "UserInformation");

            migrationBuilder.DropColumn(
                name: "EmailNotifications",
                table: "UserInformation");

            migrationBuilder.DropColumn(
                name: "NotificationId",
                table: "UserInformation");

            migrationBuilder.CreateTable(
                name: "NotificationConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    NotificationId = table.Column<string>(nullable: true),
                    ConfigurationType = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    UserInformationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationConfigurations_UserInformation_UserInformationId",
                        column: x => x.UserInformationId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationMessages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Navigation = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    New = table.Column<bool>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    UserInformationId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationMessages_UserInformation_UserInformationId",
                        column: x => x.UserInformationId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationConfigurations_UserInformationId",
                table: "NotificationConfigurations",
                column: "UserInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationMessages_UserInformationId",
                table: "NotificationMessages",
                column: "UserInformationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationConfigurations");

            migrationBuilder.DropTable(
                name: "NotificationMessages");

            migrationBuilder.AddColumn<bool>(
                name: "BrowserNotifications",
                table: "UserInformation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotifications",
                table: "UserInformation",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NotificationId",
                table: "UserInformation",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommentId = table.Column<int>(nullable: false),
                    MatchId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    New = table.Column<bool>(nullable: false),
                    TimeStamp = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_UserInformation_UserId",
                        column: x => x.UserId,
                        principalTable: "UserInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }
    }
}
