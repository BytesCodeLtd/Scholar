SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Test', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Test
    (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Test PRIMARY KEY,
        Title           NVARCHAR(MAX)     NOT NULL,
        Type            NVARCHAR(MAX)     NOT NULL CONSTRAINT DF_Test_Type DEFAULT (N''),
        Date            DATE              NOT NULL CONSTRAINT DF_Test_Date DEFAULT (CAST(SYSUTCDATETIME() AS DATE)),
        InstituteId     INT               NOT NULL,
        SubjectId       INT               NOT NULL,
        TotalMarks      INT               NOT NULL,
        DurationMinutes INT               NOT NULL,
        HeaderNotes     NVARCHAR(MAX)     NULL,
        PaperSettingsSnapshot NVARCHAR(MAX) NULL,
        CreatedAt       DATETIME2         NOT NULL CONSTRAINT DF_Test_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt       DATETIME2         NOT NULL CONSTRAINT DF_Test_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive        BIT               NOT NULL CONSTRAINT DF_Test_IsActive  DEFAULT (1),
        CONSTRAINT FK_Test_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Test_Subjects_SubjectId FOREIGN KEY (SubjectId)
            REFERENCES dbo.Subjects (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_Test_InstituteId_IsActive ON dbo.Test (InstituteId, IsActive);
    CREATE INDEX IX_Test_SubjectId            ON dbo.Test (SubjectId);
END
GO
