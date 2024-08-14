using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: true),
                    LocalPath = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.UniqueConstraint("AK_Movies_Title", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "SubtitlePhrases",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: false),
                    VideoClipPath = table.Column<string>(type: "nvarchar(700)", maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitlePhrases", x => new { x.MovieId, x.Sequence });
                    table.ForeignKey(
                        name: "FK_SubtitlePhrases_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePhrases_Text",
                table: "SubtitlePhrases",
                column: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubtitlePhrases");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
