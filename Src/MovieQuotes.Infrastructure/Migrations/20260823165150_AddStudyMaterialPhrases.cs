using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyMaterialPhrases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyMaterialPhrase",
                columns: table => new
                {
                    StudyMaterialId = table.Column<int>(type: "int", nullable: false),
                    PhraseId = table.Column<int>(type: "int", nullable: false),
                    Sequance = table.Column<int>(type: "int", nullable: false),
                    ArabicTranslation = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
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
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterialPhrase_StudyMaterialId",
                table: "StudyMaterialPhrase",
                column: "StudyMaterialId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyMaterialPhrase");
        }
    }
}
