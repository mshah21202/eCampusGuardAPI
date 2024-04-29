using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPermitApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermitApplications_Vehicles_VehicleId",
                table: "PermitApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_PermitApplications_Vehicles_VehicleId",
                table: "PermitApplications",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermitApplications_Vehicles_VehicleId",
                table: "PermitApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_PermitApplications_Vehicles_VehicleId",
                table: "PermitApplications",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
