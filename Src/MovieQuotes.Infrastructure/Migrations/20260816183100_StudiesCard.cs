using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudiesCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PhraseId = table.Column<int>(type: "int", nullable: false),
                    PartOfSpeech = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ContentArabicTranslation = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Origin = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Definition = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ArPhraseTranslation = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true),
                    IsDraft = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Examples = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Synonyms = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false),
                    Level = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false),
                    Pronunciation = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false),
                    IsVulgar = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyMaterials_SubtitlePhrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "SubtitlePhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudyCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    StudyMaterialId = table.Column<int>(type: "int", nullable: false),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.ComputedColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyCards_StudyMaterials_StudyMaterialId",
                        column: x => x.StudyMaterialId,
                        principalTable: "StudyMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CardProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NextReviewAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IntervalDays = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    EaseFactor = table.Column<double>(type: "double", nullable: false, defaultValue: 2.5),
                    ReviewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LapseCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CardProgresses_StudyCards_CardId",
                        column: x => x.CardId,
                        principalTable: "StudyCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_CardId",
                table: "CardProgresses",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_StudyMaterialId",
                table: "StudyCards",
                column: "StudyMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_PhraseId",
                table: "StudyMaterials",
                column: "PhraseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardProgresses");

            migrationBuilder.DropTable(
                name: "StudyCards");

            migrationBuilder.DropTable(
                name: "StudyMaterials");
        }
    }
}
