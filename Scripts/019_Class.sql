SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Class', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Class
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Class PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        SectionId    INT               NOT NULL,
        Name         NVARCHAR(50)      NOT NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Class_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Class_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_Class_IsActive  DEFAULT (1),
        CONSTRAINT FK_Class_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Class_Sections_SectionId FOREIGN KEY (SectionId)
            REFERENCES dbo.Sections (Id) ON DELETE NO ACTION
    );

    -- Fast institute-scoped class lookups.
    CREATE INDEX IX_Class_InstituteId ON dbo.Class (InstituteId);
    CREATE INDEX IX_Class_SectionId   ON dbo.Class (SectionId);
END
GO
