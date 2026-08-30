SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.McqOptions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.McqOptions
    (
        Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_McqOptions PRIMARY KEY,
        QuestionId INT               NOT NULL,
        [Text]     NVARCHAR(MAX)     NOT NULL,
        IsCorrect  BIT               NOT NULL,
        CreatedAt  DATETIME2         NOT NULL CONSTRAINT DF_McqOptions_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt  DATETIME2         NOT NULL CONSTRAINT DF_McqOptions_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive   BIT               NOT NULL CONSTRAINT DF_McqOptions_IsActive  DEFAULT (1),
        CONSTRAINT FK_McqOptions_Questions_QuestionId FOREIGN KEY (QuestionId)
            REFERENCES dbo.Questions (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_McqOptions_QuestionId ON dbo.McqOptions (QuestionId);
END
GO
