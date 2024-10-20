using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Homework7.EFMigrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    country_id = table.Column<string>(type: "char(3)", nullable: false),
                    country_name = table.Column<string>(type: "char(40)", nullable: false),
                    area_sqkm = table.Column<int>(type: "integer", nullable: false),
                    population = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.country_id);
                });

            migrationBuilder.CreateTable(
                name: "olympics",
                columns: table => new
                {
                    olympic_id = table.Column<string>(type: "char(7)", nullable: false),
                    country_id = table.Column<string>(type: "char(3)", nullable: false),
                    city = table.Column<string>(type: "char(50)", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    startdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    enddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_olympics", x => x.olympic_id);
                    table.ForeignKey(
                        name: "FK_olympics_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    player_id = table.Column<string>(type: "char(10)", nullable: false),
                    country_id = table.Column<string>(type: "char(3)", nullable: false),
                    player_name = table.Column<string>(type: "text", nullable: false),
                    birthdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.player_id);
                    table.ForeignKey(
                        name: "FK_players_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "country_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    event_id = table.Column<string>(type: "char(7)", nullable: false),
                    event_name = table.Column<string>(type: "char(40)", nullable: false),
                    eventtype = table.Column<string>(type: "char(20)", nullable: false),
                    olympic_id = table.Column<string>(type: "char(7)", nullable: false),
                    is_team_event = table.Column<int>(type: "integer", nullable: false),
                    num_players_in_team = table.Column<int>(type: "integer", nullable: false),
                    result_noted_in = table.Column<string>(type: "char(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.event_id);
                    table.ForeignKey(
                        name: "FK_events_olympics_olympic_id",
                        column: x => x.olympic_id,
                        principalTable: "olympics",
                        principalColumn: "olympic_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    event_id = table.Column<string>(type: "char(7)", nullable: false),
                    player_id = table.Column<string>(type: "char(10)", nullable: false),
                    medal = table.Column<string>(type: "char(7)", nullable: false),
                    result = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => new { x.event_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_results_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_results_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "player_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_olympic_id",
                table: "events",
                column: "olympic_id");

            migrationBuilder.CreateIndex(
                name: "IX_olympics_country_id",
                table: "olympics",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_country_id",
                table: "players",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_results_player_id",
                table: "results",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "results");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "olympics");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
