using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSportScoringRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SportScoringRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SportId = table.Column<long>(type: "bigint", nullable: false),
                    ExactPoints = table.Column<int>(type: "int", nullable: false),
                    ClosePoints = table.Column<int>(type: "int", nullable: false),
                    WinnerPoints = table.Column<int>(type: "int", nullable: false),
                    CloseMarginTolerance = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportScoringRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportScoringRules_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SportScoringRules_SportId",
                table: "SportScoringRules",
                column: "SportId",
                unique: true);

            // Pocetne vrednosti pravila bodovanja po sportu (izracunate iz istorijskih rezultata): Exact / Close / Winner /
            // tolerancija razlike (NULL = sport nema Close). Kasnije se menjaju obicnim UPDATE-om.
            migrationBuilder.Sql(@"
                INSERT INTO SportScoringRules (SportId, ExactPoints, ClosePoints, WinnerPoints, CloseMarginTolerance)
                SELECT s.Id, v.ExactPoints, v.ClosePoints, v.WinnerPoints, v.Tolerance
                FROM Sports s
                JOIN (VALUES
                    ('Soccer', 3, 2, 1, 0),
                    ('Ice Hockey', 4, 2, 1, 0),
                    ('Volleyball', 2, 0, 1, NULL),
                    ('Handball', 6, 2, 1, 1),
                    ('American Football', 7, 2, 1, 4),
                    ('Basketball', 10, 2, 1, 3)
                ) AS v(Name, ExactPoints, ClosePoints, WinnerPoints, Tolerance) ON s.Name = v.Name;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SportScoringRules");
        }
    }
}
