SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Chapters', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Chapters
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Chapters PRIMARY KEY,
        [Number]  INT               NOT NULL,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        SubjectId INT               NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Chapters_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Chapters_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Chapters_IsActive  DEFAULT (1),
        CONSTRAINT FK_Chapters_Subjects_SubjectId FOREIGN KEY (SubjectId)
            REFERENCES dbo.Subjects (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Chapters_SubjectId ON dbo.Chapters (SubjectId);
END
GO



DECLARE @subjectId INT = (
    SELECT s.Id
    FROM dbo.Subjects s
    JOIN dbo.Grades g ON g.Id = s.GradeId
    JOIN dbo.Boards b ON b.Id = s.BoardId
    WHERE b.[Name] = N'PECTA' AND g.[Name] = N'9th' AND s.[Name] = N'Computer Science'
);

IF @subjectId IS NOT NULL
BEGIN
    INSERT INTO dbo.Chapters ([Number], [Name], SubjectId)
    SELECT v.[Number], v.[Name], @subjectId
    FROM (VALUES
        (1, N'Introduction to Computational Systems'),
        (2, N'System Design and Troubleshooting'),
        (3, N'Introduction to Computer Networks'),
        (4, N'Computational Thinking'),
        (5, N'Web Development with HTML, CSS and Javascript'),
        (6, N'Data Science and Data Gathering'),
        (7, N'Emerging Technologies in Computer Science'),
        (8, N'Ethical, Social, and Legal Concerns in Computer Usage'),
        (9, N'Entrepreneurship in Digital Age')
    ) AS v([Number], [Name])
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.Chapters c
        WHERE c.SubjectId = @subjectId AND c.[Number] = v.[Number]
    );
END
GO
