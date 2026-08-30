SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.TestSections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestSections
    (
        Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TestSections PRIMARY KEY,
        TestId           INT               NOT NULL,
        [Order]          INT               NOT NULL,
        [Type]           INT               NOT NULL,
        Instruction      NVARCHAR(MAX)     NULL,
        MarksPerQuestion INT               NOT NULL,
        CreatedAt        DATETIME2         NOT NULL CONSTRAINT DF_TestSections_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt        DATETIME2         NOT NULL CONSTRAINT DF_TestSections_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive         BIT               NOT NULL CONSTRAINT DF_TestSections_IsActive  DEFAULT (1),
        CONSTRAINT FK_TestSections_Test_TestId FOREIGN KEY (TestId)
            REFERENCES dbo.Test (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_TestSections_TestId ON dbo.TestSections (TestId);
END
GO
