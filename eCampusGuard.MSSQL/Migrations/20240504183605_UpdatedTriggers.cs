using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eCampusGuard.MSSQL.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG_ENTRY;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG_EXIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT_;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT_;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT_;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT_;");

            migrationBuilder.Sql("CREATE OR ALTER FUNCTION [dbo].[CURR_OCCUPIED] (\r\n@AreaId INT\r\n)\r\nRETURNS INT AS\r\nBEGIN\r\nDECLARE @Result INT;\r\nDECLARE @EntryCount INT;\r\nDECLARE @ExitCount INT;\r\nSELECT @EntryCount = COUNT(*)\r\nFROM dbo.AccessLogs\r\nWHERE [Type] = 0 AND AreaId = @AreaId AND CONVERT(date, [Timestamp]) = CONVERT(date, GETDATE())\r\nSELECT @ExitCount = COUNT(*)\r\nFROM dbo.AccessLogs\r\nWHERE [Type] = 1 AND AreaId = @AreaId AND CONVERT(date, [Timestamp]) = CONVERT(date, GETDATE())\r\nSET @Result = @EntryCount - @ExitCount;\r\nRETURN @Result;\r\nEND\n");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG ON \"AccessLogs\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewAreaId INT\r\n  DECLARE InsertedAccessLogCursor CURSOR LOCAL FOR SELECT AreaId FROM Inserted\r\n  OPEN InsertedAccessLogCursor\r\n  FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Areas SET CurrentOccupied = dbo.CURR_OCCUPIED(@NewAreaId) WHERE dbo.Areas.Id = @NewAreaId\r\n    FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId\r\n  END\r\n  CLOSE InsertedAccessLogCursor DEALLOCATE InsertedAccessLogCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @NewId) WHERE dbo.Areas.Id = @NewAreaId\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @OldId) WHERE dbo.Areas.Id = @OldAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT ON \"UserPermits\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldPermitId INT\r\n  DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT ON \"UserPermits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @OldPermitId INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Inserted\r\n  OPEN InsertedUserPermitCursor DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldPermitId <> @NewPermitId)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT;");

            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT;");

            migrationBuilder.Sql("DROP FUNCTION [dbo].[CURR_OCCUPIED];");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG_ENTRY ON \"AccessLogs\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewAreaId INT, @NewType INT\r\n  DECLARE InsertedAccessLogCursor CURSOR LOCAL FOR SELECT AreaId, Type FROM Inserted\r\n  OPEN InsertedAccessLogCursor\r\n  FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId, @NewType\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewType = 0)\r\n    UPDATE dbo.Areas SET CurrentOccupied = CurrentOccupied + 1 WHERE dbo.Areas.Id = @NewAreaId\r\n    FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId, @NewType\r\n  END\r\n  CLOSE InsertedAccessLogCursor DEALLOCATE InsertedAccessLogCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_ACCESSLOG_EXIT ON \"AccessLogs\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewAreaId INT, @NewType INT\r\n  DECLARE InsertedAccessLogCursor CURSOR LOCAL FOR SELECT AreaId, Type FROM Inserted\r\n  OPEN InsertedAccessLogCursor\r\n  FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId, @NewType\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewType = 1)\r\n    UPDATE dbo.Areas SET CurrentOccupied = CurrentOccupied - 1 WHERE dbo.Areas.Id = @NewAreaId\r\n    FETCH NEXT FROM InsertedAccessLogCursor INTO @NewAreaId, @NewType\r\n  END\r\n  CLOSE InsertedAccessLogCursor DEALLOCATE InsertedAccessLogCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT_ ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @NewId) WHERE dbo.Areas.Id = @NewAreaId\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @OldId) WHERE dbo.Areas.Id = @OldAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_DELETE_USERPERMIT_ ON \"UserPermits\" AFTER Delete AS\r\nBEGIN\r\n  DECLARE @OldPermitId INT\r\n  DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_INSERT_USERPERMIT_ ON \"UserPermits\" AFTER Insert AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @NewStatus INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId, Status FROM Inserted\r\n  OPEN InsertedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@NewStatus = 0)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId, @NewStatus\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor\r\nEND");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_USERPERMIT_ ON \"UserPermits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewPermitId INT, @OldPermitId INT\r\n  DECLARE InsertedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Inserted\r\n  OPEN InsertedUserPermitCursor DECLARE DeletedUserPermitCursor CURSOR LOCAL FOR SELECT PermitId FROM Deleted\r\n  OPEN DeletedUserPermitCursor\r\n  FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldPermitId <> @NewPermitId)\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @NewPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @NewPermitId\r\n    UPDATE dbo.Permits SET Occupied = (SELECT COUNT(*) FROM dbo.UserPermits WHERE PermitId = @OldPermitId AND [Status] = 0) WHERE dbo.Permits.Id = @OldPermitId\r\n    FETCH NEXT FROM InsertedUserPermitCursor INTO @NewPermitId FETCH NEXT FROM DeletedUserPermitCursor INTO @OldPermitId\r\n  END\r\n  CLOSE InsertedUserPermitCursor DEALLOCATE InsertedUserPermitCursor CLOSE DeletedUserPermitCursor DEALLOCATE DeletedUserPermitCursor\r\nEND");
        }
    }
}
