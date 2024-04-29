using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserPermitTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT;");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT;");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Areas SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Areas.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND");
        }
    }
}
