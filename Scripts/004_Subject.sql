SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Subjects
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Subjects PRIMARY KEY,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        BoardId   INT               NOT NULL,
        GradeId   INT               NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Subjects_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Subjects_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Subjects_IsActive  DEFAULT (1),
        CONSTRAINT FK_Subjects_Boards_BoardId FOREIGN KEY (BoardId)
            REFERENCES dbo.Boards (Id),
        CONSTRAINT FK_Subjects_Grades_GradeId FOREIGN KEY (GradeId)
            REFERENCES dbo.Grades (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Subjects_BoardId ON dbo.Subjects (BoardId);
    CREATE INDEX IX_Subjects_GradeId ON dbo.Subjects (GradeId);
END
GO


INSERT INTO dbo.Subjects ([Name], BoardId, GradeId)
SELECT v.[Name], b.Id, g.Id
FROM dbo.Boards b
CROSS JOIN dbo.Grades g
CROSS JOIN (VALUES
    (N'Physics'),
    (N'Chemistry'),
    (N'Biology'),
    (N'Mathematics'),
    (N'Computer Science'),
    (N'English'),
    (N'Urdu'),
    (N'Islamiat'),
    (N'Pakistan Studies')
) AS v([Name])
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Subjects s
    WHERE s.BoardId = b.Id AND s.GradeId = g.Id AND s.[Name] = v.[Name]
);
GO
