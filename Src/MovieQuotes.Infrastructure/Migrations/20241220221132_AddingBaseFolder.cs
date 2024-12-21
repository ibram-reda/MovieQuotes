using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingBaseFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "StudyPhrases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BaseFolderDir",
                table: "Movies",
                type: "nvarchar(700)",
                maxLength: 700,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "StudyPhrases");

            migrationBuilder.DropColumn(
                name: "BaseFolderDir",
                table: "Movies");
        }
    }
}
