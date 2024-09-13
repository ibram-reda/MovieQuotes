using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdForPhrase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitlePhrases",
                table: "SubtitlePhrases");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "SubtitlePhrases",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_SubtitlePhrases_MovieId_Sequence",
                table: "SubtitlePhrases",
                columns: new[] { "MovieId", "Sequence" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitlePhrases",
                table: "SubtitlePhrases",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_SubtitlePhrases_MovieId_Sequence",
                table: "SubtitlePhrases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitlePhrases",
                table: "SubtitlePhrases");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubtitlePhrases");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitlePhrases",
                table: "SubtitlePhrases",
                columns: new[] { "MovieId", "Sequence" });
        }
    }
}
