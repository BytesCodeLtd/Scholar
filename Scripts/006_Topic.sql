SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Topics', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Topics
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Topics PRIMARY KEY,
        [Number]  NVARCHAR(MAX)     NOT NULL,
        [Name]    NVARCHAR(MAX)     NOT NULL,
        ChapterId INT               NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Topics_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Topics_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Topics_IsActive  DEFAULT (1),
        CONSTRAINT FK_Topics_Chapters_ChapterId FOREIGN KEY (ChapterId)
            REFERENCES dbo.Chapters (Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_Topics_ChapterId ON dbo.Topics (ChapterId);
END
GO

DECLARE @subjectId INT = (
    SELECT s.Id
    FROM dbo.Subjects s
    JOIN dbo.Grades g ON g.Id = s.GradeId
    JOIN dbo.Boards b ON b.Id = s.BoardId
    WHERE b.[Name] = N'PECTA' AND g.[Name] = N'9th' AND s.[Name] = N'Computer Science'
);

IF @subjectId IS NOT NULL
BEGIN
    INSERT INTO dbo.Topics ([Number], [Name], ChapterId)
    SELECT t.TopicNumber, t.TopicName, c.Id
    FROM dbo.Chapters c
    JOIN (VALUES
        -- Chapter 1: Introduction to Computational Systems
        (1, N'1.1',  N'Theory of System'),
        (1, N'1.2',  N'Software'),
        (1, N'1.3',  N'The Architecture of Von Neumann Computers'),
        (1, N'1.4',  N'Number System'),
        (1, N'1.5',  N'Data Representation in Computing Systems'),
        (1, N'1.6',  N'Binary Arithmetic Operations'),
        (1, N'1.7',  N'Common Text and Coding Schemes'),

        -- Chapter 2: System Design and Troubleshooting
        (2, N'2.1',  N'Basics of Digital Systems'),
        (2, N'2.2',  N'Boolean Algebra and Logic Gates'),
        (2, N'2.3',  N'Boolean Functions'),
        (2, N'2.4',  N'Creating Logic Diagrams'),
        (2, N'2.5',  N'System Troubleshooting'),

        -- Chapter 3: Introduction to Computer Networks
        (3, N'3.1',  N'Network as a System'),
        (3, N'3.2',  N'Fundamental Concepts in Data Communication'),
        (3, N'3.3',  N'Networking Devices'),
        (3, N'3.4',  N'Network Topologies'),
        (3, N'3.5',  N'Transmission Modes'),
        (3, N'3.6',  N'The OSI Networking Model'),
        (3, N'3.7',  N'IPv4 and IPv6'),
        (3, N'3.8',  N'Network Protocols'),
        (3, N'3.9',  N'Network Security'),
        (3, N'3.10', N'Network Security Methods'),
        (3, N'3.11', N'Types of Networks'),
        (3, N'3.12', N'Real-World Applications of Computer Networks'),

        -- Chapter 4: Computational Thinking
        (4, N'4.1',  N'Computational Thinking'),
        (4, N'4.2',  N'Principles of Computational Thinking'),
        (4, N'4.3',  N'Algorithm Design Methods'),
        (4, N'4.4',  N'Algorithmic Evaluating Methods'),
        (4, N'4.5',  N'Flowchart'),
        (4, N'4.6',  N'Introduction to LARP (Logic of Algorithms for Resolution of Problems)'),
        (4, N'4.7',  N'Error Identification and Debugging'),

        -- Chapter 5: Web Development with HTML, CSS and Javascript
        (5, N'5.1',  N'Web Development'),
        (5, N'5.2',  N'Basic Components of Web Development'),
        (5, N'5.3',  N'Getting Started with HTML'),
        (5, N'5.4',  N'HTML Basic Structure'),
        (5, N'5.5',  N'Creating Content with HTML'),
        (5, N'5.6',  N'Styling with CSS'),
        (5, N'5.7',  N'Introduction to JavaScript'),

        -- Chapter 6: Data Science and Data Gathering
        (6, N'6.1',  N'Data'),
        (6, N'6.2',  N'Data Types'),
        (6, N'6.3',  N'Organizing and Analysing Data'),
        (6, N'6.4',  N'Data Repositories (Storage and Processing)'),
        (6, N'6.5',  N'Data Visualisation'),
        (6, N'6.6',  N'Data Pre-Processing and Analysis'),
        (6, N'6.7',  N'Cloud Storage and Data Backup'),
        (6, N'6.8',  N'Introduction to Data Science'),

        -- Chapter 7: Emerging Technologies in Computer Science
        (7, N'7.1',  N'Introduction to Artificial Intelligence (AI)'),
        (7, N'7.2',  N'AI Algorithms and Techniques'),
        (7, N'7.3',  N'Introduction to Internet of Things (IoT)'),

        -- Chapter 8: Ethical, Social, and Legal Concerns in Computer Usage
        (8, N'8.1',  N'Responsible Computer Usage'),
        (8, N'8.2',  N'Responsible Use of Internet'),
        (8, N'8.3',  N'Intellectual Property Concepts'),

        -- Chapter 9: Entrepreneurship in Digital Age
        (9, N'9.1',  N'Entrepreneurship'),
        (9, N'9.2',  N'Entrepreneurship in the Digital Landscape'),
        (9, N'9.3',  N'Business Idea Generation'),
        (9, N'9.4',  N'Ethical and Sustainable Entrepreneurship')
    ) AS t(ChapterNumber, TopicNumber, TopicName)
        ON c.[Number] = t.ChapterNumber
    WHERE c.SubjectId = @subjectId
      AND NOT EXISTS (
          SELECT 1 FROM dbo.Topics tp
          WHERE tp.ChapterId = c.Id AND tp.[Number] = t.TopicNumber
      );
END
GO
