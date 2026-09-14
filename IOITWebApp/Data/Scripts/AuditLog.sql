-- =============================================================
-- Bảng nhật ký thao tác người dùng (AuditLog)
-- Chạy trên DB CHÍNH (ConnectionStrings:DefaultConnection - 1MainDB).
-- Lưu ý: UserId dùng INT (khớp với bảng [User].UserId của hệ thống),
--        thay vì UNIQUEIDENTIFIER trong mẫu ban đầu.
-- =============================================================
IF OBJECT_ID(N'[dbo].[AuditLog]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AuditLog]
    (
        Id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        OccurredAt  DATETIME2        NOT NULL DEFAULT SYSUTCDATETIME(),

        UserId      INT              NULL,
        UserName    NVARCHAR(100)    NULL,

        Action      NVARCHAR(30)     NOT NULL,   -- CREATE / UPDATE / DELETE
        EntityType  NVARCHAR(100)    NOT NULL,   -- MacBeTong / HopDong / KhachHang / DuAn / NhanVien / Xe
        EntityId    NVARCHAR(100)    NULL,

        OldValues   NVARCHAR(MAX)    NULL,       -- JSON
        NewValues   NVARCHAR(MAX)    NULL,       -- JSON

        Success     BIT              NOT NULL,
        IpAddress   NVARCHAR(50)     NULL,
        TraceId     NVARCHAR(100)    NULL,
        Description NVARCHAR(500)    NULL
    );

    CREATE INDEX IX_AuditLog_OccurredAt ON [dbo].[AuditLog] (OccurredAt DESC);
    CREATE INDEX IX_AuditLog_Entity     ON [dbo].[AuditLog] (EntityType, EntityId);
    CREATE INDEX IX_AuditLog_UserId     ON [dbo].[AuditLog] (UserId);
END
GO

-- Nếu bảng đã được tạo trước đó với UserId UNIQUEIDENTIFIER (script mẫu ban đầu)
-- thì chuyển sang INT, nếu không sẽ lỗi "Operand type clash: int is incompatible with uniqueidentifier".
IF EXISTS (SELECT 1 FROM sys.columns c JOIN sys.types t ON t.user_type_id = c.user_type_id
           WHERE c.object_id = OBJECT_ID(N'[dbo].[AuditLog]') AND c.name = N'UserId' AND t.name = N'uniqueidentifier')
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_UserId' AND object_id = OBJECT_ID(N'[dbo].[AuditLog]'))
        DROP INDEX IX_AuditLog_UserId ON [dbo].[AuditLog];

    ALTER TABLE [dbo].[AuditLog] ALTER COLUMN UserId INT NULL;

    CREATE INDEX IX_AuditLog_UserId ON [dbo].[AuditLog] (UserId);
END
GO

-- Bổ sung index nếu bảng tạo bằng script mẫu (chưa có index)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_OccurredAt' AND object_id = OBJECT_ID(N'[dbo].[AuditLog]'))
    CREATE INDEX IX_AuditLog_OccurredAt ON [dbo].[AuditLog] (OccurredAt DESC);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_Entity' AND object_id = OBJECT_ID(N'[dbo].[AuditLog]'))
    CREATE INDEX IX_AuditLog_Entity ON [dbo].[AuditLog] (EntityType, EntityId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_AuditLog_UserId' AND object_id = OBJECT_ID(N'[dbo].[AuditLog]'))
    CREATE INDEX IX_AuditLog_UserId ON [dbo].[AuditLog] (UserId);
GO

-- =============================================================
-- Đăng ký chức năng "Nhật ký người dùng" vào menu Hệ thống
-- (có thể thay bằng thao tác trên màn hình Quản lý chức năng).
-- Code chức năng: NKND  |  Url: system/auditlog
-- =============================================================
IF NOT EXISTS (SELECT 1 FROM [dbo].[Function] WHERE Code = N'NKND')
BEGIN
    DECLARE @ParentId INT = (SELECT TOP 1 FunctionId FROM [dbo].[Function] WHERE Url = N'system');

    INSERT INTO [dbo].[Function] (Name, Code, FunctionParentId, Url, Note, Location, Icon, CreatedAt, Status)
    VALUES (N'Nhật ký người dùng', N'NKND', ISNULL(@ParentId, 0), N'auditlog', N'Tra cứu, theo dõi log thao tác người dùng', 99, N'fas fa-history', GETDATE(), 1);

    DECLARE @FunctionId INT = SCOPE_IDENTITY();

    -- Gán quyền xem (VIEW) cho nhóm quyền Administrator (RoleId = 1). Type = 2: Nhóm quyền - Chức năng
    INSERT INTO [dbo].[FunctionRole] (TargetId, FunctionId, ActiveKey, Type, CreatedAt, Status)
    VALUES (1, @FunctionId, N'100000000', 2, GETDATE(), 1);
END
GO
