SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.TestQuestions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestQuestions
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TestQuestions PRIMARY KEY,
        TestId        INT               NOT NULL,
        QuestionId    INT               NOT NULL,
        TestSectionId INT               NULL,
        [Order]       INT               NOT NULL,
        Section       INT               NOT NULL,
        CreatedAt     DATETIME2         NOT NULL CONSTRAINT DF_TestQuestions_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt     DATETIME2         NOT NULL CONSTRAINT DF_TestQuestions_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive      BIT               NOT NULL CONSTRAINT DF_TestQuestions_IsActive  DEFAULT (1),
        CONSTRAINT FK_TestQuestions_Test_TestId FOREIGN KEY (TestId)
            REFERENCES dbo.Test (Id) ON DELETE CASCADE,
        CONSTRAINT FK_TestQuestions_Questions_QuestionId FOREIGN KEY (QuestionId)
            REFERENCES dbo.Questions (Id) ON DELETE NO ACTION,
        CONSTRAINT FK_TestQuestions_TestSections_TestSectionId FOREIGN KEY (TestSectionId)
            REFERENCES dbo.TestSections (Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_TestQuestions_TestId        ON dbo.TestQuestions (TestId);
    CREATE INDEX IX_TestQuestions_QuestionId    ON dbo.TestQuestions (QuestionId);
    CREATE INDEX IX_TestQuestions_TestSectionId ON dbo.TestQuestions (TestSectionId);
END
GO
