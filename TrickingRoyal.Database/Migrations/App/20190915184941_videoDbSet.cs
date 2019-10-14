using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class videoDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Video_Matches_MatchId",
                table: "Video");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Video",
                table: "Video");

            migrationBuilder.RenameTable(
                name: "Video",
                newName: "Videos");

            migrationBuilder.RenameIndex(
                name: "IX_Video_MatchId",
                table: "Videos",
                newName: "IX_Videos_MatchId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Videos",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MatchId",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_UserId",
                table: "Videos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Matches_MatchId",
                table: "Videos",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_UserInformation_UserId",
                table: "Videos",
                column: "UserId",
                principalTable: "UserInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Matches_MatchId",
                table: "Videos");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_UserInformation_UserId",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_UserId",
                table: "Videos");

            migrationBuilder.RenameTable(
                name: "Videos",
                newName: "Video");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_MatchId",
                table: "Video",
                newName: "IX_Video_MatchId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Video",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MatchId",
                table: "Video",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Video",
                table: "Video",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Video_Matches_MatchId",
                table: "Video",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
