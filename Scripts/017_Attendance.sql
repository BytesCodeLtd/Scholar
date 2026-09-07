SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Attendances', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Attendances
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Attendances PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        StudentId    INT               NOT NULL,
        [Date]       DATE              NOT NULL,
        Status       NVARCHAR(20)      NOT NULL,
        Remarks      NVARCHAR(500)     NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Attendances_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Attendances_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_Attendances_IsActive  DEFAULT (1),
        CONSTRAINT FK_Attendances_Students_StudentId FOREIGN KEY (StudentId)
            REFERENCES dbo.Students (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Attendances_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE NO ACTION
    );

    -- At most one attendance mark per student per day.
    CREATE UNIQUE INDEX UX_Attendances_StudentId_Date ON dbo.Attendances (StudentId, [Date]);
    -- Fast institute-scoped roster lookups for a given day.
    CREATE INDEX IX_Attendances_InstituteId_Date ON dbo.Attendances (InstituteId, [Date]);
END
GO
