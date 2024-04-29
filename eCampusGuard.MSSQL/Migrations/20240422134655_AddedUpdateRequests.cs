using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedUpdateRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermits",
                table: "UserPermits");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserPermits",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermits",
                table: "UserPermits",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UpdateRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    NewPermitId = table.Column<int>(type: "int", nullable: false),
                    UpdatedVehicleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UpdateRequests_Permits_NewPermitId",
                        column: x => x.NewPermitId,
                        principalTable: "Permits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UpdateRequests_UserPermits_Id",
                        column: x => x.Id,
                        principalTable: "UserPermits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UpdateRequests_Vehicles_UpdatedVehicleId",
                        column: x => x.UpdatedVehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermits_UserId",
                table: "UserPermits",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateRequests_NewPermitId",
                table: "UpdateRequests",
                column: "NewPermitId");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateRequests_UpdatedVehicleId",
                table: "UpdateRequests",
                column: "UpdatedVehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UpdateRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPermits",
                table: "UserPermits");

            migrationBuilder.DropIndex(
                name: "IX_UserPermits_UserId",
                table: "UserPermits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPermits");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPermits",
                table: "UserPermits",
                columns: new[] { "UserId", "PermitId" });

            migrationBuilder.CreateTable(
                name: "TransferRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    PermitId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferRequests_AspNetUsers_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferRequests_AspNetUsers_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TransferRequests_Permits_PermitId",
                        column: x => x.PermitId,
                        principalTable: "Permits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_FromUserId",
                table: "TransferRequests",
                column: "FromUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_PermitId",
                table: "TransferRequests",
                column: "PermitId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_ToUserId",
                table: "TransferRequests",
                column: "ToUserId");
        }
    }
}
