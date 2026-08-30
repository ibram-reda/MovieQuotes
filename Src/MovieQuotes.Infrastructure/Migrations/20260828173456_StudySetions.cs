using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudySetions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyPhraseProgress");

            migrationBuilder.DropTable(
                name: "StudyPhrases");

            migrationBuilder.CreateTable(
                name: "StudySessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ExerciseType = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CurrentCardPosition = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudySessionCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StudySessionId = table.Column<int>(type: "int", nullable: false),
                    StudyCardId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessionCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessionCards_StudySessions_StudySessionId",
                        column: x => x.StudySessionId,
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionCards_StudySessionId_Order",
                table: "StudySessionCards",
                columns: new[] { "StudySessionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_CompletedAt",
                table: "StudySessions",
                column: "CompletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudySessionCards");

            migrationBuilder.DropTable(
                name: "StudySessions");

            migrationBuilder.CreateTable(
                name: "StudyPhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PhraseId = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ArContentTranslation = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    ArPhraseTranslation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Examples = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    IsDraft = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Level = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Origin = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Pronunciation = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    StudyType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Synonyms = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    Translation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyPhrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyPhrases_SubtitlePhrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "SubtitlePhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudyPhraseProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StudyPhraseId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EaseFactor = table.Column<double>(type: "double", nullable: false, defaultValue: 2.5),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    LapseCount = table.Column<int>(type: "int", nullable: false),
                    LastReviewed = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NextReviewDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Repetition = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_StudyPhrases_PhraseId",
                table: "StudyPhrases",
                column: "PhraseId");
        }
    }
}
