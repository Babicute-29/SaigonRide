using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaigonRide.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHistoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalPrice",
                table: "RentingHistories",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateIndex(
                name: "IX_RentingHistories_PickupStationId",
                table: "RentingHistories",
                column: "PickupStationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentingHistories_ReturnStationId",
                table: "RentingHistories",
                column: "ReturnStationId");

            migrationBuilder.CreateIndex(
                name: "IX_RentingHistories_VehicleId",
                table: "RentingHistories",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RentingHistories_Stations_PickupStationId",
                table: "RentingHistories",
                column: "PickupStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentingHistories_Stations_ReturnStationId",
                table: "RentingHistories",
                column: "ReturnStationId",
                principalTable: "Stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RentingHistories_Vehicles_VehicleId",
                table: "RentingHistories",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentingHistories_Stations_PickupStationId",
                table: "RentingHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RentingHistories_Stations_ReturnStationId",
                table: "RentingHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_RentingHistories_Vehicles_VehicleId",
                table: "RentingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RentingHistories_PickupStationId",
                table: "RentingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RentingHistories_ReturnStationId",
                table: "RentingHistories");

            migrationBuilder.DropIndex(
                name: "IX_RentingHistories_VehicleId",
                table: "RentingHistories");

            migrationBuilder.AlterColumn<double>(
                name: "TotalPrice",
                table: "RentingHistories",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }
    }
}
