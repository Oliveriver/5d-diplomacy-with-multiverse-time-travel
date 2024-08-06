using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _5dDiplomacyWithMultiverseTimeTravel.Migrations
{
    /// <inheritdoc />
    public partial class SpecifySandboxAndAdjacencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasStrictAdjacencies",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSandbox",
                table: "Games",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasStrictAdjacencies",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "IsSandbox",
                table: "Games");
        }
    }
}
