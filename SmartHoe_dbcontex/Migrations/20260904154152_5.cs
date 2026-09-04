using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHoe_dbcontex.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Heater_IsOn",
                table: "Devices",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Heater_IsOn",
                table: "Devices");
        }
    }
}
