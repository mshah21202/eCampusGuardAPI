using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserPermitTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldAreaId <> @NewAreaId)\r\n    UPDATE dbo.Areas SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewId AND [Status] = 0) WHERE dbo.Areas.Id = @NewAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT ON \"UserPermits\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldPermitId INT\r\n  DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT ON \"UserPermits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @OldPermitId INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Inserted\r\n  OPEN InsertedUserPermitCursor DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldPermitId <> @NewPermitId)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT;");
        }
    }
}
