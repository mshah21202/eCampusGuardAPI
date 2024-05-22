using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexUpdateRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UpdateRequests_UserPermitId",
                table: "UpdateRequests");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateRequests_UserPermitId_Status",
                table: "UpdateRequests",
                columns: new[] { "UserPermitId", "Status" },
                unique: true,
                filter: "[Status] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UpdateRequests_UserPermitId_Status",
                table: "UpdateRequests");

            migrationBuilder.CreateIndex(
                name: "IX_UpdateRequests_UserPermitId",
                table: "UpdateRequests",
                column: "UserPermitId");
        }
    }
}
