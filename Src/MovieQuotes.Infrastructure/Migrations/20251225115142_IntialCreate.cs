using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    NameId = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    FolderName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "varchar(700)", maxLength: 700, nullable: true),
                    BaseFolderDir = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    LocalPath = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false),
                    CoverUrl = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IMDBId = table.Column<string>(type: "varchar(12)", maxLength: 12, nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.UniqueConstraint("AK_Movies_FolderName", x => x.FolderName);
                    table.UniqueConstraint("AK_Movies_NameId", x => x.NameId);
                    table.UniqueConstraint("AK_Movies_Title", x => x.Title);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.UniqueConstraint("AK_Word_Text", x => x.Text);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubtitlePhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Text = table.Column<string>(type: "varchar(700)", maxLength: 700, nullable: false),
                    VideoClipPath = table.Column<string>(type: "varchar(700)", maxLength: 700, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubtitlePhrases", x => x.Id);
                    table.UniqueConstraint("AK_SubtitlePhrases_MovieId_Sequence", x => new { x.MovieId, x.Sequence });
                    table.ForeignKey(
                        name: "FK_SubtitlePhrases_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PhraseWords",
                columns: table => new
                {
                    PhraseId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    Index = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhraseWords", x => new { x.PhraseId, x.WordId, x.Index });
                    table.ForeignKey(
                        name: "FK_PhraseWords_SubtitlePhrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "SubtitlePhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhraseWords_Word_WordId",
                        column: x => x.WordId,
                        principalTable: "Word",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudyPhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    PhraseId = table.Column<int>(type: "int", nullable: false),
                    StudyType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Content = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Origin = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    ArContentTranslation = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    Translation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    ArPhraseTranslation = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
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

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_PhraseId_WordId_Index",
                table: "PhraseWords",
                columns: new[] { "PhraseId", "WordId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_WordId",
                table: "PhraseWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyPhrases_PhraseId",
                table: "StudyPhrases",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_SubtitlePhrases_Text",
                table: "SubtitlePhrases",
                column: "Text");

            migrationBuilder.CreateIndex(
                name: "IX_Word_Text",
                table: "Word",
                column: "Text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhraseWords");

            migrationBuilder.DropTable(
                name: "StudyPhrases");

            migrationBuilder.DropTable(
                name: "Word");

            migrationBuilder.DropTable(
                name: "SubtitlePhrases");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
