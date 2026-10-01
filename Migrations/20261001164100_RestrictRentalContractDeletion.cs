using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Migrations
{
    /// <inheritdoc />
    public partial class RestrictRentalContractDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalContracts_Clients_ClientId",
                table: "RentalContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalContracts_Vehicles_VehicleId",
                table: "RentalContracts");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalContracts_Clients_ClientId",
                table: "RentalContracts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RentalContracts_Vehicles_VehicleId",
                table: "RentalContracts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RentalContracts_Clients_ClientId",
                table: "RentalContracts");

            migrationBuilder.DropForeignKey(
                name: "FK_RentalContracts_Vehicles_VehicleId",
                table: "RentalContracts");

            migrationBuilder.AddForeignKey(
                name: "FK_RentalContracts_Clients_ClientId",
                table: "RentalContracts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RentalContracts_Vehicles_VehicleId",
                table: "RentalContracts",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
