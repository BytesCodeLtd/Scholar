SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Questions', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Questions
    (
        Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Questions PRIMARY KEY,
        TopicId    INT               NOT NULL,
        [Type]     INT               NOT NULL,
        [Text]     NVARCHAR(MAX)     NOT NULL,
        Marks      INT               NOT NULL,
        Difficulty INT               NOT NULL,
        Category   INT               NOT NULL,
        OwnerId    NVARCHAR(450)     NULL,
        CreatedAt  DATETIME2         NOT NULL CONSTRAINT DF_Questions_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt  DATETIME2         NOT NULL CONSTRAINT DF_Questions_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive   BIT               NOT NULL CONSTRAINT DF_Questions_IsActive  DEFAULT (1),
        CONSTRAINT FK_Questions_Topics_TopicId FOREIGN KEY (TopicId)
            REFERENCES dbo.Topics (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Questions_AspNetUsers_OwnerId FOREIGN KEY (OwnerId)
            REFERENCES dbo.AspNetUsers (Id) ON DELETE SET NULL
    );

    CREATE INDEX IX_Questions_TopicId ON dbo.Questions (TopicId);
    CREATE INDEX IX_Questions_OwnerId ON dbo.Questions (OwnerId);
END
GO
