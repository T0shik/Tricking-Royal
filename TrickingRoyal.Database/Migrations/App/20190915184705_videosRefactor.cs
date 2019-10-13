using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrickingRoyal.Database.Migrations.App
{
    public partial class videosRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbs",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ThumbsHost",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "ThumbsOpponent",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Videos",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "VideosHost",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "VideosOpponent",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Winner",
                table: "Matches");

            migrationBuilder.CreateTable(
                name: "Video",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VideoIndex = table.Column<int>(nullable: false),
                    UserIndex = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    VideoPath = table.Column<string>(nullable: true),
                    ThumbPath = table.Column<string>(nullable: true),
                    Empty = table.Column<bool>(nullable: false),
                    MatchId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Video", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Video_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Video_MatchId",
                table: "Video",
                column: "MatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Video");

            migrationBuilder.AddColumn<string>(
                name: "Thumbs",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbsHost",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbsOpponent",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Videos",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideosHost",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideosOpponent",
                table: "Matches",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Winner",
                table: "Matches",
                nullable: true);
        }
    }
}
