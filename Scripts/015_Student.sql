SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Students PRIMARY KEY,
        InstituteId  INT               NOT NULL,
        GradeId      INT               NOT NULL,
        FullName     NVARCHAR(200)     NOT NULL,
        RollNumber   NVARCHAR(50)      NULL,
        Section      NVARCHAR(50)      NULL,
        Gender       NVARCHAR(20)      NULL,
        DateOfBirth  DATETIME2         NULL,
        GuardianName NVARCHAR(200)     NULL,
        PhoneNumber  NVARCHAR(50)      NULL,
        CreatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt    DATETIME2         NOT NULL CONSTRAINT DF_Students_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive     BIT               NOT NULL CONSTRAINT DF_Students_IsActive  DEFAULT (1),
        CONSTRAINT FK_Students_Institutes_InstituteId FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Students_Grades_GradeId FOREIGN KEY (GradeId)
            REFERENCES dbo.Grades (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_Students_InstituteId ON dbo.Students (InstituteId);
    CREATE INDEX IX_Students_GradeId     ON dbo.Students (GradeId);
END
GO
