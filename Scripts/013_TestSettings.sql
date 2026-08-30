SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.TestSettings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TestSettings
    (
        Id                          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TestSettings PRIMARY KEY,
        InstituteId                 INT           NOT NULL,

        -- Layout & typography
        HeaderLayout                INT           NOT NULL CONSTRAINT DF_TestSettings_HeaderLayout        DEFAULT (1),
        HeaderFontStyle             NVARCHAR(100) NOT NULL CONSTRAINT DF_TestSettings_HeaderFontStyle     DEFAULT (N'Default'),
        HeaderFontSize              INT           NOT NULL CONSTRAINT DF_TestSettings_HeaderFontSize       DEFAULT (40),
        HeadingFontSize             INT           NOT NULL CONSTRAINT DF_TestSettings_HeadingFontSize      DEFAULT (16),
        TextFontSize                INT           NOT NULL CONSTRAINT DF_TestSettings_TextFontSize         DEFAULT (12),
        TextFormatting              INT           NOT NULL CONSTRAINT DF_TestSettings_TextFormatting       DEFAULT (0),
        HeadingFormatting           INT           NOT NULL CONSTRAINT DF_TestSettings_HeadingFormatting    DEFAULT (1),
        LineHeight                  DECIMAL(3,1)  NOT NULL CONSTRAINT DF_TestSettings_LineHeight           DEFAULT (2.5),
        PaperFontColor              INT           NOT NULL CONSTRAINT DF_TestSettings_PaperFontColor       DEFAULT (0),
        EnglishTextFontStyle        NVARCHAR(100) NOT NULL CONSTRAINT DF_TestSettings_EnglishTextFontStyle DEFAULT (N'Default'),

        -- Watermark & branding
        Watermark                   INT           NOT NULL CONSTRAINT DF_TestSettings_Watermark            DEFAULT (0),
        PicWatermarkOpacity         DECIMAL(3,1)  NOT NULL CONSTRAINT DF_TestSettings_PicWatermarkOpacity  DEFAULT (0.4),
        PicWatermarkHeight          INT           NOT NULL CONSTRAINT DF_TestSettings_PicWatermarkHeight   DEFAULT (60),
        PicWatermarkWidth           INT           NOT NULL CONSTRAINT DF_TestSettings_PicWatermarkWidth    DEFAULT (70),
        LogoHeight                  INT           NOT NULL CONSTRAINT DF_TestSettings_LogoHeight           DEFAULT (100),
        LogoWidth                   INT           NOT NULL CONSTRAINT DF_TestSettings_LogoWidth            DEFAULT (100),

        -- Content options
        McqsLayout                  INT           NOT NULL CONSTRAINT DF_TestSettings_McqsLayout           DEFAULT (0),
        NumeralStyle                INT           NOT NULL CONSTRAINT DF_TestSettings_NumeralStyle         DEFAULT (0),
        ImportantNote               NVARCHAR(MAX) NULL,
        FooterText                  NVARCHAR(MAX) NULL,

        -- Toggles
        ShowConceptualQuestionMark  BIT           NOT NULL CONSTRAINT DF_TestSettings_ShowConceptualMark  DEFAULT (0),
        HidePhoneNumber             BIT           NOT NULL CONSTRAINT DF_TestSettings_HidePhoneNumber      DEFAULT (0),
        QuestionHeadingBottomBorder BIT           NOT NULL CONSTRAINT DF_TestSettings_HeadingBottomBorder DEFAULT (0),
        ShowChapterName             BIT           NOT NULL CONSTRAINT DF_TestSettings_ShowChapterName      DEFAULT (0),
        ShowGrammaticalTerms        BIT           NOT NULL CONSTRAINT DF_TestSettings_ShowGrammaticalTerms DEFAULT (0),

        -- Audit
        CreatedAt                   DATETIME2     NOT NULL CONSTRAINT DF_TestSettings_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt                   DATETIME2     NOT NULL CONSTRAINT DF_TestSettings_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive                    BIT           NOT NULL CONSTRAINT DF_TestSettings_IsActive  DEFAULT (1),

        CONSTRAINT FK_TestSettings_Institutes FOREIGN KEY (InstituteId)
            REFERENCES dbo.Institutes (Id) ON DELETE CASCADE
    );

    -- One settings row per institute.
    CREATE UNIQUE INDEX UX_TestSettings_InstituteId ON dbo.TestSettings (InstituteId);
END
GO
