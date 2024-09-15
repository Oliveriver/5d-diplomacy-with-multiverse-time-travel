using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _5dDiplomacyWithMultiverseTimeTravel.Migrations
{
    /// <inheritdoc />
    public partial class PersistAdvancingBoards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MightAdvance",
                table: "Boards",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MightAdvance",
                table: "Boards");
        }
    }
}
