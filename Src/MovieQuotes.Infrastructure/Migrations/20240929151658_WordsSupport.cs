using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WordsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Word",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Word", x => x.Id);
                    table.UniqueConstraint("AK_Word_Text", x => x.Text);
                });

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
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_PhraseId_WordId_Index",
                table: "PhraseWords",
                columns: new[] { "PhraseId", "WordId", "Index" });

            migrationBuilder.CreateIndex(
                name: "IX_PhraseWords_WordId",
                table: "PhraseWords",
                column: "WordId");

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
                name: "Word");
        }
    }
}
