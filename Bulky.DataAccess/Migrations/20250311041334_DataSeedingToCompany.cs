using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DataSeedingToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "companies",
                columns: new[] { "Id", "City", "Name", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "San Francisco", "Tech Innovators Inc.", "94103", "CA", "123 Innovation Drive" },
                    { 2, "New York", "Global Solutions Ltd.", "10001", "NY", "456 Enterprise Ave" },
                    { 3, "Austin", "Eco Future Corp.", "73301", "TX", "789 Greenway Blvd" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "companies",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
