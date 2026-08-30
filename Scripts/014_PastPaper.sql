SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.PastPapers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PastPapers
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PastPapers PRIMARY KEY,
        SubjectId INT               NOT NULL,
        Title            NVARCHAR(200) NOT NULL,
        [Year]           INT           NULL,
        FileUrl          NVARCHAR(MAX) NOT NULL,
        OriginalFileName NVARCHAR(260) NOT NULL CONSTRAINT DF_PastPapers_OriginalFileName DEFAULT (N''),
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_PastPapers_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_PastPapers_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_PastPapers_IsActive  DEFAULT (1),

        CONSTRAINT FK_PastPapers_Subjects FOREIGN KEY (SubjectId)
            REFERENCES dbo.Subjects (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_PastPapers_SubjectId ON dbo.PastPapers (SubjectId);
END
GO
