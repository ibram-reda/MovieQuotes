using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManyStudyProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {            
            migrationBuilder.DropColumn(
                name: "Mode",
                table: "StudyCards");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseType",
                table: "CardProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_CardProgresses_CardId_ExerciseType",
                table: "CardProgresses",
                columns: new[] { "CardId", "ExerciseType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_CardProgresses_CardId_ExerciseType",
                table: "CardProgresses");

            migrationBuilder.DropColumn(
                name: "ExerciseType",
                table: "CardProgresses");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "StudyCards",
                type: "int",
                nullable: false,
                defaultValue: 0); 
        }
    }
}
