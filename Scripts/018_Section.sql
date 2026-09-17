SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Sections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Sections
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sections PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        Name         NVARCHAR(50)      NOT NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Sections_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Sections_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_Sections_IsActive  DEFAULT (1),
        CONSTRAINT FK_Sections_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE
    );

    -- Fast institute-scoped section lookups.
    CREATE INDEX IX_Sections_InstituteId ON dbo.Sections (InstituteId);
END
GO
