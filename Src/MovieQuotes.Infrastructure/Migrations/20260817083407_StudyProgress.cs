using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MovieQuotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudyProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CardProgresses");

            migrationBuilder.DropTable(
                name: "StudyCards");

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
                name: "IX_StudyCards_StudyMaterialId",
                table: "StudyCards",
                column: "StudyMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_CardId",
                table: "CardProgresses",
                column: "CardId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CardProgresses_CardId",
                table: "CardProgresses");

            migrationBuilder.CreateIndex(
                name: "IX_CardProgresses_CardId",
                table: "CardProgresses",
                column: "CardId");
        }
    }
}
