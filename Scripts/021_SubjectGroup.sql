SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.SubjectGroups', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubjectGroups
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SubjectGroups PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        Name         NVARCHAR(100)     NOT NULL,
        Description  NVARCHAR(500)     NULL,
        ClassId      INT               NOT NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_SubjectGroups_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_SubjectGroups_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_SubjectGroups_IsActive  DEFAULT (1),
        CONSTRAINT FK_SubjectGroups_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE,
        CONSTRAINT FK_SubjectGroups_Class_ClassId FOREIGN KEY (ClassId)
            REFERENCES dbo.Class (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_SubjectGroups_InstituteId ON dbo.SubjectGroups (InstituteId);
    CREATE INDEX IX_SubjectGroups_ClassId     ON dbo.SubjectGroups (ClassId);
END
GO

-- Join: subject group <-> sections (many-to-many). Group side cascades; the section
-- side is NO ACTION to avoid multiple cascade paths back to Institute.
IF OBJECT_ID(N'dbo.SubjectGroupSection', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubjectGroupSection
    (
        SectionId       INT NOT NULL,
        SubjectGroupId  INT NOT NULL,
        CONSTRAINT PK_SubjectGroupSection PRIMARY KEY (SectionId, SubjectGroupId),
        CONSTRAINT FK_SubjectGroupSection_Sections_SectionId FOREIGN KEY (SectionId)
            REFERENCES dbo.Sections (Id) ON DELETE NO ACTION,
        CONSTRAINT FK_SubjectGroupSection_SubjectGroups_SubjectGroupId FOREIGN KEY (SubjectGroupId)
            REFERENCES dbo.SubjectGroups (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SubjectGroupSection_SubjectGroupId ON dbo.SubjectGroupSection (SubjectGroupId);
END
GO

-- Join: subject group <-> institute subjects (many-to-many). Same cascade shape.
IF OBJECT_ID(N'dbo.SubjectGroupSubject', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SubjectGroupSubject
    (
        InstituteSubjectId  INT NOT NULL,
        SubjectGroupId      INT NOT NULL,
        CONSTRAINT PK_SubjectGroupSubject PRIMARY KEY (InstituteSubjectId, SubjectGroupId),
        CONSTRAINT FK_SubjectGroupSubject_InstituteSubjects_InstituteSubjectId FOREIGN KEY (InstituteSubjectId)
            REFERENCES dbo.InstituteSubjects (Id) ON DELETE NO ACTION,
        CONSTRAINT FK_SubjectGroupSubject_SubjectGroups_SubjectGroupId FOREIGN KEY (SubjectGroupId)
            REFERENCES dbo.SubjectGroups (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_SubjectGroupSubject_SubjectGroupId ON dbo.SubjectGroupSubject (SubjectGroupId);
END
GO
