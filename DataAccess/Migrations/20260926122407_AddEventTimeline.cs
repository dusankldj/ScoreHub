using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddEventTimeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TimelineSyncedAt",
                table: "Matches",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventTimelines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Home = table.Column<bool>(type: "bit", nullable: false),
                    ApiFootballId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerId = table.Column<long>(type: "bigint", nullable: true),
                    AssistantId = table.Column<long>(type: "bigint", nullable: true),
                    TeamId = table.Column<long>(type: "bigint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Minute = table.Column<int>(type: "int", nullable: true),
                    Period = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExternalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTimelines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventTimelines_Matches_EventId",
                        column: x => x.EventId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTimelines_Players_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTimelines_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventTimelines_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTimelines_AssistantId",
                table: "EventTimelines",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTimelines_EventId",
                table: "EventTimelines",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTimelines_ExternalId",
                table: "EventTimelines",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventTimelines_PlayerId",
                table: "EventTimelines",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTimelines_TeamId",
                table: "EventTimelines",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTimelines");

            migrationBuilder.DropColumn(
                name: "TimelineSyncedAt",
                table: "Matches");
        }
    }
}
