using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NameId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 700, nullable: true),
                    Year = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IMDBId = table.Column<string>(type: "TEXT", maxLength: 12, nullable: true),
                    BaseFolderDir = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    FolderName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    VideoFilePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CoverFilePath = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BackdropFilePath = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PosterFilePath = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TMDbId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    VoteAverage = table.Column<float>(type: "REAL", nullable: false),
                    VoteCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IsAdult = table.Column<bool>(type: "INTEGER", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.UniqueConstraint("AK_Movies_FolderName", x => x.FolderName);
                    table.UniqueConstraint("AK_Movies_NameId", x => x.NameId);
                    table.UniqueConstraint("AK_Movies_Title", x => x.Title);
                });

            migrationBuilder.CreateTable(
                name: "StudySessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExerciseType = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CurrentCardPosition = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Text = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.UniqueConstraint("AK_Word_Text", x => x.Text);
                });

            migrationBuilder.CreateTable(
                name: "GenreMovie",
                columns: table => new
                {
                    GenresId = table.Column<int>(type: "INTEGER", nullable: false),
                    MoviesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenreMovie", x => new { x.GenresId, x.MoviesId });
                    table.ForeignKey(
                        name: "FK_GenreMovie_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenreMovie_Movies_MoviesId",
                        column: x => x.MoviesId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubtitlePhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 700, nullable: false),
                    VideoClipPath = table.Column<string>(type: "TEXT", maxLength: 700, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "StudySessionCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudySessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudyCardId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "PhraseWords",
                columns: table => new
                {
                    PhraseId = table.Column<int>(type: "INTEGER", nullable: false),
                    WordId = table.Column<int>(type: "INTEGER", nullable: false),
                    Index = table.Column<int>(type: "INTEGER", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "StudyMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PhraseId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartOfSpeech = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ContentArabicTranslation = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Origin = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Definition = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ArPhraseTranslation = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    IsDraft = table.Column<bool>(type: "INTEGER", nullable: false),
                    Examples = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Synonyms = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Level = table.Column<string>(type: "TEXT", maxLength: 2, nullable: false),
                    Pronunciation = table.Column<string>(type: "TEXT", maxLength: 70, nullable: false),
                    IsVulgar = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Tags = table.Column<string>(type: "TEXT", maxLength: 120, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "StudyCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudyMaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "StudyMaterialPhrase",
                columns: table => new
                {
                    StudyMaterialId = table.Column<int>(type: "INTEGER", nullable: false),
                    PhraseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequance = table.Column<int>(type: "INTEGER", nullable: false),
                    ArabicTranslation = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Content = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyMaterialPhrase", x => new { x.PhraseId, x.StudyMaterialId, x.Sequance });
                    table.ForeignKey(
                        name: "FK_StudyMaterialPhrase_StudyMaterials_StudyMaterialId",
                        column: x => x.StudyMaterialId,
                        principalTable: "StudyMaterials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudyMaterialPhrase_SubtitlePhrases_PhraseId",
                        column: x => x.PhraseId,
                        principalTable: "SubtitlePhrases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CardProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CardId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseType = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    LastReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    NextReviewAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Repetitions = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    IntervalDays = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    EaseFactor = table.Column<double>(type: "REAL", nullable: false, defaultValue: 2.5),
                    ReviewCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    LapseCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardProgresses", x => x.Id);
                    table.UniqueConstraint("AK_CardProgresses_CardId_ExerciseType", x => new { x.CardId, x.ExerciseType });
                    table.ForeignKey(
                        name: "FK_CardProgresses_StudyCards_CardId",
                        column: x => x.CardId,
                        principalTable: "StudyCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenreMovie_MoviesId",
                table: "GenreMovie",
                column: "MoviesId");

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_PhraseId_WordId_Index",
                table: "PhraseWords",
                columns: new[] { "PhraseId", "WordId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_WordId",
                table: "PhraseWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyCards_StudyMaterialId",
                table: "StudyCards",
                column: "StudyMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterialPhrase_StudyMaterialId",
                table: "StudyMaterialPhrase",
                column: "StudyMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_PhraseId",
                table: "StudyMaterials",
                column: "PhraseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionCards_StudySessionId_Order",
                table: "StudySessionCards",
                columns: new[] { "StudySessionId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_CompletedAt",
                table: "StudySessions",
                column: "CompletedAt");

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
                name: "CardProgresses");

            migrationBuilder.DropTable(
                name: "GenreMovie");

            migrationBuilder.DropTable(
                name: "PhraseWords");

            migrationBuilder.DropTable(
                name: "StudyMaterialPhrase");

            migrationBuilder.DropTable(
                name: "StudySessionCards");

            migrationBuilder.DropTable(
                name: "StudyCards");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Word");

            migrationBuilder.DropTable(
                name: "StudySessions");

            migrationBuilder.DropTable(
                name: "StudyMaterials");

            migrationBuilder.DropTable(
                name: "SubtitlePhrases");

            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
