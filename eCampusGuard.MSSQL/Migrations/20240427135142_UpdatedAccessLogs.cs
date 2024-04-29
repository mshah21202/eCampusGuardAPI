using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAccessLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_AspNetUsers_UserId",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Permits_PermitId",
                table: "AccessLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_Vehicles_VehicleId",
                table: "AccessLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessLogs_PermitId",
                table: "AccessLogs");

            migrationBuilder.DropIndex(
                name: "IX_AccessLogs_UserId",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "PermitId",
                table: "AccessLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AccessLogs");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "AccessLogs",
                newName: "UserPermitId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_VehicleId",
                table: "AccessLogs",
                newName: "IX_AccessLogs_UserPermitId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_UserPermits_UserPermitId",
                table: "AccessLogs",
                column: "UserPermitId",
                principalTable: "UserPermits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessLogs_UserPermits_UserPermitId",
                table: "AccessLogs");

            migrationBuilder.RenameColumn(
                name: "UserPermitId",
                table: "AccessLogs",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_AccessLogs_UserPermitId",
                table: "AccessLogs",
                newName: "IX_AccessLogs_VehicleId");

            migrationBuilder.AddColumn<int>(
                name: "PermitId",
                table: "AccessLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "AccessLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_PermitId",
                table: "AccessLogs",
                column: "PermitId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessLogs_UserId",
                table: "AccessLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_AspNetUsers_UserId",
                table: "AccessLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Permits_PermitId",
                table: "AccessLogs",
                column: "PermitId",
                principalTable: "Permits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessLogs_Vehicles_VehicleId",
                table: "AccessLogs",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }
    }
}
