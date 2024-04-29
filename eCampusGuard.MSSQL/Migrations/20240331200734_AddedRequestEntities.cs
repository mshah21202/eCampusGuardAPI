using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddedRequestEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AcademicYear",
                table: "PermitApplications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TransferRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<int>(type: "int", nullable: false),
                    ToUserId = table.Column<int>(type: "int", nullable: false),
                    PermitId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Areas SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Areas.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT;");

            migrationBuilder.DropTable(
                name: "TransferRequests");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicYear",
                table: "PermitApplications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
