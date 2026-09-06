SET NOCOUNT ON;

/*
    Seed: MCQ question bank for Computer Science (9th, PECTA) — Chapter 1
    "Introduction to Computational Systems" (topics 1.1 - 1.7).

    Idempotent: questions are matched by Topic + Text, options by Question + Text,
    so re-running inserts nothing new. Shared bank questions (OwnerId = NULL).

    Keep this as a SINGLE batch (no GO until the end): the @Mcqs table variable
    must stay in scope across all statements below.
*/

DECLARE @subjectId INT = (
    SELECT s.Id
    FROM dbo.Subjects s
    JOIN dbo.Grades g ON g.Id = s.GradeId
    JOIN dbo.Boards b ON b.Id = s.BoardId
    WHERE b.[Name] = N'PECTA' AND g.[Name] = N'9th' AND s.[Name] = N'Computer Science'
);

IF @subjectId IS NULL
BEGIN
    PRINT 'Computer Science 9th (PECTA) subject not found — skipping Chapter 1 MCQ seed.';
    RETURN;
END

-- Type: 0 = Mcq | Difficulty: 0 = Easy, 1 = Medium, 2 = Hard | Category: 0 = Exercise
DECLARE @Mcqs TABLE
(
    TopicNumber NVARCHAR(10)  NOT NULL,
    QText       NVARCHAR(MAX) NOT NULL,
    Difficulty  INT           NOT NULL,
    OptA        NVARCHAR(400) NOT NULL,
    OptB        NVARCHAR(400) NOT NULL,
    OptC        NVARCHAR(400) NOT NULL,
    OptD        NVARCHAR(400) NOT NULL,
    CorrectOpt  CHAR(1)       NOT NULL
);

INSERT INTO @Mcqs (TopicNumber, QText, Difficulty, OptA, OptB, OptC, OptD, CorrectOpt) VALUES
-- 1.1 Theory of System
(N'1.1', N'Which of the following is NOT one of the three basic operations of a computer system?', 0,
    N'Input', N'Processing', N'Output', N'Compilation', N'D'),
(N'1.1', N'The information about the output that is returned to a system to control or improve it is called:', 1,
    N'Feedback', N'Booting', N'Processing', N'Storage', N'A'),

-- 1.2 Software
(N'1.2', N'Which of the following is an example of system software?', 0,
    N'MS Excel', N'Operating System', N'Adobe Photoshop', N'Google Chrome', N'B'),
(N'1.2', N'Software designed to perform a specific task for the user is called:', 0,
    N'System software', N'Firmware', N'Application software', N'Device driver', N'C'),
(N'1.2', N'Which software acts as an interface between the user and the computer hardware?', 1,
    N'Application software', N'Operating system', N'Spreadsheet', N'Web browser', N'B'),

-- 1.3 The Architecture of Von Neumann Computers
(N'1.3', N'In the Von Neumann architecture, data and instructions are stored in:', 1,
    N'Separate memories', N'The same memory', N'The ALU', N'Input devices', N'B'),
(N'1.3', N'Which component of the CPU performs arithmetic and logical operations?', 0,
    N'Control Unit', N'Arithmetic Logic Unit', N'Register', N'Memory Unit', N'B'),
(N'1.3', N'The part of the CPU that directs and coordinates all the operations of the computer is the:', 1,
    N'ALU', N'Register', N'Control Unit', N'Cache', N'C'),
(N'1.3', N'Small, high-speed storage locations located inside the CPU are called:', 1,
    N'Registers', N'Sectors', N'Buses', N'Ports', N'A'),

-- 1.4 Number System
(N'1.4', N'The base (radix) of the binary number system is:', 0,
    N'2', N'8', N'10', N'16', N'A'),
(N'1.4', N'The base of the hexadecimal number system is:', 0,
    N'8', N'10', N'16', N'2', N'C'),
(N'1.4', N'Which set of digits is used in the octal number system?', 1,
    N'0 - 1', N'0 - 7', N'0 - 9', N'0 - F', N'B'),
(N'1.4', N'The binary equivalent of the decimal number 10 is:', 1,
    N'1010', N'1100', N'1001', N'1000', N'A'),
(N'1.4', N'The decimal equivalent of the binary number 1111 is:', 1,
    N'14', N'15', N'16', N'11', N'B'),

-- 1.5 Data Representation in Computing Systems
(N'1.5', N'A single binary digit (0 or 1) is known as a:', 0,
    N'Byte', N'Nibble', N'Bit', N'Word', N'C'),
(N'1.5', N'How many bits are there in one byte?', 0,
    N'2', N'4', N'8', N'16', N'C'),
(N'1.5', N'A group of four bits is called a:', 1,
    N'Bit', N'Nibble', N'Byte', N'Word', N'B'),
(N'1.5', N'One kilobyte (KB) is equal to:', 1,
    N'1000 bytes', N'1024 bytes', N'1024 bits', N'8 bytes', N'B'),

-- 1.6 Binary Arithmetic Operations
(N'1.6', N'In binary addition, 1 + 1 is equal to:', 1,
    N'0', N'1', N'10', N'11', N'C'),
(N'1.6', N'In binary addition, the sum of 1 + 1 + 1 is:', 2,
    N'10', N'11', N'01', N'100', N'B'),
(N'1.6', N'The result of the binary subtraction 10 - 1 is:', 1,
    N'0', N'1', N'10', N'11', N'B'),

-- 1.7 Common Text and Coding Schemes
(N'1.7', N'ASCII stands for:', 0,
    N'American Standard Code for Information Interchange', N'American System Code for Internet Interchange',
    N'Advanced Standard Code for Information Interchange', N'American Standard Computer for Information Interchange', N'A'),
(N'1.7', N'Standard ASCII uses how many bits to represent a single character?', 1,
    N'4', N'7', N'16', N'32', N'B'),
(N'1.7', N'Which coding scheme is designed to represent the characters of almost all the languages of the world?', 1,
    N'ASCII', N'BCD', N'Unicode', N'EBCDIC', N'C'),
(N'1.7', N'Unicode (UTF-16) commonly uses how many bits to represent a character?', 2,
    N'7', N'8', N'16', N'4', N'C');

-- Insert questions that do not already exist (matched by Topic + Text), scoped to this subject/chapter.
INSERT INTO dbo.Questions (TopicId, [Type], [Text], Marks, Difficulty, Category)
SELECT tp.Id, 0, m.QText, 1, m.Difficulty, 0
FROM @Mcqs m
JOIN dbo.Topics tp    ON tp.[Number] = m.TopicNumber
JOIN dbo.Chapters c   ON c.Id = tp.ChapterId AND c.SubjectId = @subjectId
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Questions q
    WHERE q.TopicId = tp.Id AND q.[Text] = m.QText
);

-- Insert the four options for each question (matched by Question + Text so re-runs are safe).
;WITH Options AS
(
    SELECT tp.Id AS TopicId, m.QText, v.OptText, v.IsCorrect
    FROM @Mcqs m
    JOIN dbo.Topics tp  ON tp.[Number] = m.TopicNumber
    JOIN dbo.Chapters c ON c.Id = tp.ChapterId AND c.SubjectId = @subjectId
    CROSS APPLY (VALUES
        (m.OptA, CASE WHEN m.CorrectOpt = 'A' THEN 1 ELSE 0 END),
        (m.OptB, CASE WHEN m.CorrectOpt = 'B' THEN 1 ELSE 0 END),
        (m.OptC, CASE WHEN m.CorrectOpt = 'C' THEN 1 ELSE 0 END),
        (m.OptD, CASE WHEN m.CorrectOpt = 'D' THEN 1 ELSE 0 END)
    ) v (OptText, IsCorrect)
)
INSERT INTO dbo.McqOptions (QuestionId, [Text], IsCorrect)
SELECT q.Id, o.OptText, o.IsCorrect
FROM Options o
JOIN dbo.Questions q ON q.TopicId = o.TopicId AND q.[Text] = o.QText
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.McqOptions mo
    WHERE mo.QuestionId = q.Id AND mo.[Text] = o.OptText
);

PRINT 'Chapter 1 Computer Science MCQ seed applied.';
GO
