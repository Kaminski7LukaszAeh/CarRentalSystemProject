using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addClassModelsToVehicle3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModel_VehicleBrand_BrandId",
                table: "VehicleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModel_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleModel",
                table: "VehicleModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBrand",
                table: "VehicleBrand");

            migrationBuilder.RenameTable(
                name: "VehicleModel",
                newName: "VehicleModels");

            migrationBuilder.RenameTable(
                name: "VehicleBrand",
                newName: "VehicleBrands");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleModel_BrandId",
                table: "VehicleModels",
                newName: "IX_VehicleModels_BrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels",
                column: "BrandId",
                principalTable: "VehicleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_VehicleModelId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands");

            migrationBuilder.RenameTable(
                name: "VehicleModels",
                newName: "VehicleModel");

            migrationBuilder.RenameTable(
                name: "VehicleBrands",
                newName: "VehicleBrand");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleModels_BrandId",
                table: "VehicleModel",
                newName: "IX_VehicleModel_BrandId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleModel",
                table: "VehicleModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBrand",
                table: "VehicleBrand",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModel_VehicleBrand_BrandId",
                table: "VehicleModel",
                column: "BrandId",
                principalTable: "VehicleBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModel_VehicleModelId",
                table: "Vehicles",
                column: "VehicleModelId",
                principalTable: "VehicleModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
