using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SpacedSpacedRepetion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyPhraseProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StudyPhraseId = table.Column<int>(type: "int", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Repetition = table.Column<int>(type: "int", nullable: false),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    EaseFactor = table.Column<double>(type: "double", nullable: false, defaultValue: 2.5),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    LapseCount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPhraseProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPhraseProgress_StudyPhrases_StudyPhraseId",
                        column: x => x.StudyPhraseId,
                        principalTable: "StudyPhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPhraseProgress_StudyPhraseId",
                table: "StudyPhraseProgress",
                column: "StudyPhraseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyPhraseProgress");
        }
    }
}
