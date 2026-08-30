SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Boards', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Boards
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Boards PRIMARY KEY,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Boards_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Boards_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Boards_IsActive  DEFAULT (1)
    );
END
GO

INSERT INTO dbo.Boards ([Name])
SELECT v.[Name]
FROM (VALUES
    (N'PECTA'),
    (N'KPTB'),
    (N'FEDERAL')
) AS v([Name])
WHERE NOT EXISTS (SELECT 1 FROM dbo.Boards b WHERE b.[Name] = v.[Name]);
GO
