SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Grades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Grades
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Grades PRIMARY KEY,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Grades_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Grades_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Grades_IsActive  DEFAULT (1)
    );
END
GO


INSERT INTO dbo.Grades ([Name])
SELECT v.[Name]
FROM (VALUES (N'9th'), (N'10th'), (N'1st Year'), (N'2nd Year')) AS v([Name])
WHERE NOT EXISTS (SELECT 1 FROM dbo.Grades g WHERE g.[Name] = v.[Name]);
GO
