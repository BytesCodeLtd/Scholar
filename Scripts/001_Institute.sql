SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Institutes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Institutes
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Institutes PRIMARY KEY,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        [Address] NVARCHAR(MAX)     NULL,
        LogoUrl   NVARCHAR(MAX)     NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Institutes_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Institutes_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Institutes_IsActive  DEFAULT (1)
    );
END
GO

INSERT INTO dbo.Institutes ([Name])
SELECT v.[Name]
FROM (VALUES
    (N'Default Institute')
) AS v([Name])
WHERE NOT EXISTS (SELECT 1 FROM dbo.Institutes i WHERE i.[Name] = v.[Name]);
GO
