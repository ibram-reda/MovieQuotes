using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFolderName_ResetKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Movies_FolderName",
                table: "Movies",
                column: "FolderName");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Movies_NameId",
                table: "Movies",
                column: "NameId");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Movies_Title",
                table: "Movies",
                column: "Title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Movies_FolderName",
                table: "Movies");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Movies_NameId",
                table: "Movies");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Movies_Title",
                table: "Movies");
        }
    }
}
