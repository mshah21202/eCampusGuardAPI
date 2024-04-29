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
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT;");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @NewId) WHERE dbo.Areas.Id = @NewAreaId\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @OldId) WHERE dbo.Areas.Id = @OldAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT;");

            migrationBuilder.Sql("CREATE TRIGGER LC_TRIGGER_AFTER_UPDATE_PERMIT ON \"Permits\" AFTER Update AS\r\nBEGIN\r\n  DECLARE @NewId INT, @NewAreaId INT, @OldId INT, @OldAreaId INT\r\n  DECLARE InsertedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Inserted\r\n  OPEN InsertedPermitCursor DECLARE DeletedPermitCursor CURSOR LOCAL FOR SELECT Id, AreaId FROM Deleted\r\n  OPEN DeletedPermitCursor\r\n  FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  WHILE @@FETCH_STATUS = 0\r\n  BEGIN\r\n    IF (@OldAreaId <> @NewAreaId)\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @NewId) WHERE dbo.Areas.Id = @NewAreaId\r\n    UPDATE dbo.Areas SET Occupied = (SELECT SUM(Occupied) FROM dbo.Permits WHERE Id = @OldId) WHERE dbo.Areas.Id = @OldAreaId\r\n    FETCH NEXT FROM InsertedPermitCursor INTO @NewId, @NewAreaId FETCH NEXT FROM DeletedPermitCursor INTO @OldId, @OldAreaId\r\n  END\r\n  CLOSE InsertedPermitCursor DEALLOCATE InsertedPermitCursor CLOSE DeletedPermitCursor DEALLOCATE DeletedPermitCursor\r\nEND");
        }
    }
}
