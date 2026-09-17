SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.InstituteSubjects', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InstituteSubjects
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InstituteSubjects PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        Name         NVARCHAR(100)     NOT NULL,
        Code         NVARCHAR(30)      NOT NULL,
        Type         NVARCHAR(20)      NOT NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_InstituteSubjects_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_InstituteSubjects_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_InstituteSubjects_IsActive  DEFAULT (1),
        CONSTRAINT FK_InstituteSubjects_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE
    );

    -- Fast institute-scoped subject lookups.
    CREATE INDEX IX_InstituteSubjects_InstituteId ON dbo.InstituteSubjects (InstituteId);
END
GO
