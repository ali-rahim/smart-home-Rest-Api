using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHoe_dbcontex.Migrations
{
    /// <inheritdoc />
    public partial class three : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecurityAlarm_IsOn",
                table: "Devices",
                newName: "Fan_IsOn");

            migrationBuilder.CreateTable(
                name: "DeviceReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NumericValue = table.Column<double>(type: "float", nullable: true),
                    BoolValue = table.Column<bool>(type: "bit", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceReadings_DeviceId_RecordedAt",
                table: "DeviceReadings",
                columns: new[] { "DeviceId", "RecordedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceReadings");

            migrationBuilder.RenameColumn(
                name: "Fan_IsOn",
                table: "Devices",
                newName: "SecurityAlarm_IsOn");
        }
    }
}
