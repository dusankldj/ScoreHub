using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSportScoringRuleMaxScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxScore",
                table: "SportScoringRules",
                type: "int",
                nullable: true);

            // Odbojka se igra do 3 seta - najveca dozvoljena prognoza po timu je 3 (ostali sportovi bez ogranicenja).
            migrationBuilder.Sql(@"
                UPDATE r SET r.MaxScore = 3
                FROM SportScoringRules r JOIN Sports s ON s.Id = r.SportId
                WHERE s.Name = 'Volleyball';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "SportScoringRules");
        }
    }
}
