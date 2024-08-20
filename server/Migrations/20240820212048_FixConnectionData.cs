using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _5dDiplomacyWithMultiverseTimeTravel.Migrations
{
    /// <inheritdoc />
    public partial class FixConnectionData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConnectionMappings",
                keyColumns: new[] { "ConnectionsId", "RegionsId" },
                keyValues: new object[] { "Gas-Spa_S", "Gas" });

            migrationBuilder.DeleteData(
                table: "ConnectionMappings",
                keyColumns: new[] { "ConnectionsId", "RegionsId" },
                keyValues: new object[] { "Gas-Spa_S", "Spa_S" });

            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "Id",
                keyValue: "Gas-Spa_S");

            migrationBuilder.InsertData(
                table: "Connections",
                columns: new[] { "Id", "Type" },
                values: new object[] { "MAO-NAO", 2 });

            migrationBuilder.InsertData(
                table: "ConnectionMappings",
                columns: new[] { "ConnectionsId", "RegionsId" },
                values: new object[,]
                {
                    { "MAO-NAO", "MAO" },
                    { "MAO-NAO", "NAO" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ConnectionMappings",
                keyColumns: new[] { "ConnectionsId", "RegionsId" },
                keyValues: new object[] { "MAO-NAO", "MAO" });

            migrationBuilder.DeleteData(
                table: "ConnectionMappings",
                keyColumns: new[] { "ConnectionsId", "RegionsId" },
                keyValues: new object[] { "MAO-NAO", "NAO" });

            migrationBuilder.DeleteData(
                table: "Connections",
                keyColumn: "Id",
                keyValue: "MAO-NAO");

            migrationBuilder.InsertData(
                table: "Connections",
                columns: new[] { "Id", "Type" },
                values: new object[] { "Gas-Spa_S", 2 });

            migrationBuilder.InsertData(
                table: "ConnectionMappings",
                columns: new[] { "ConnectionsId", "RegionsId" },
                values: new object[,]
                {
                    { "Gas-Spa_S", "Gas" },
                    { "Gas-Spa_S", "Spa_S" }
                });
        }
    }
}
