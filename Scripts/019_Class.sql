SET NOCOUNT ON;

-- Classes are institute-owned and span one or more sections (many-to-many via
-- ClassSection). The institute link cascades; section links live in the join table.
IF OBJECT_ID(N'dbo.Class', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Class
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Class PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        Name         NVARCHAR(50)      NOT NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Class_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Class_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_Class_IsActive  DEFAULT (1),
        CONSTRAINT FK_Class_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE
    );

    -- Fast institute-scoped class lookups.
    CREATE INDEX IX_Class_InstituteId ON dbo.Class (InstituteId);
END
GO

-- Class <-> sections many-to-many. Class side cascades; section side is NO ACTION to
-- avoid multiple cascade paths back to Institute (which already cascades via the class).
IF OBJECT_ID(N'dbo.ClassSection', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ClassSection
    (
        ClassId    INT NOT NULL,
        SectionId  INT NOT NULL,
        CONSTRAINT PK_ClassSection PRIMARY KEY (ClassId, SectionId),
        CONSTRAINT FK_ClassSection_Class_ClassId FOREIGN KEY (ClassId)
            REFERENCES dbo.Class (Id) ON DELETE CASCADE,
        CONSTRAINT FK_ClassSection_Sections_SectionId FOREIGN KEY (SectionId)
            REFERENCES dbo.Sections (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_ClassSection_SectionId ON dbo.ClassSection (SectionId);
END
GO

-- Students reference a class (added here because Students is created earlier, in
-- 015_Student.sql, before the Class table exists).
IF OBJECT_ID(N'dbo.FK_Students_Class_ClassId', N'F') IS NULL
   AND OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
   AND COL_LENGTH('dbo.Students', 'ClassId') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Students
        ADD CONSTRAINT FK_Students_Class_ClassId FOREIGN KEY (ClassId)
            REFERENCES dbo.Class (Id) ON DELETE NO ACTION;
END
GO
