SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.Teacher', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Teacher
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Teacher PRIMARY KEY,
        UserId    NVARCHAR(450)     NOT NULL,
        SubjectId INT               NOT NULL,
        GradeId   INT               NOT NULL,
        CreatedAt DATETIME2         NOT NULL CONSTRAINT DF_Teacher_CreatedAt DEFAULT (SYSUTCDATETIME()),
        UpdatedAt DATETIME2         NOT NULL CONSTRAINT DF_Teacher_UpdatedAt DEFAULT (SYSUTCDATETIME()),
        IsActive  BIT               NOT NULL CONSTRAINT DF_Teacher_IsActive  DEFAULT (1),
        CONSTRAINT FK_Teacher_AspNetUsers_UserId FOREIGN KEY (UserId)
            REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Teacher_Subjects_SubjectId FOREIGN KEY (SubjectId)
            REFERENCES dbo.Subjects (Id) ON DELETE CASCADE,
        CONSTRAINT FK_Teacher_Grades_GradeId FOREIGN KEY (GradeId)
            REFERENCES dbo.Grades (Id) ON DELETE NO ACTION,
        CONSTRAINT UQ_Teacher_User_Subject_Grade UNIQUE (UserId, SubjectId, GradeId)
    );

    CREATE INDEX IX_Teacher_SubjectId ON dbo.Teacher (SubjectId);
    CREATE INDEX IX_Teacher_GradeId   ON dbo.Teacher (GradeId);
END
GO

DECLARE @TeacherId NVARCHAR(450) = 'b7e4c2a1-1111-4d22-9c33-abcdef000001';
DECLARE @Email     NVARCHAR(256) = 'teacher@scholar.local';
DECLARE @Hash      NVARCHAR(MAX) = 'AQAAAAIAAYagAAAAEBXCLax6CzQnRMxcVaAcVh/+CCkqBEpSaNRpcKhm/3QDGgvDTPnySDwR+Udxww3P/A=='; /*Admin@123*/

-- Ensure the Teacher role exists (app also seeds it on startup).
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = N'TEACHER')
    INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (CONVERT(NVARCHAR(450), NEWID()), N'Teacher', N'TEACHER', CONVERT(NVARCHAR(MAX), NEWID()));

-- Sample teacher user.
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = UPPER(@Email))
BEGIN
    INSERT INTO AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName
    )
    VALUES
    (
        @TeacherId, @Email, UPPER(@Email), @Email, UPPER(@Email), 1,
        @Hash, CONVERT(NVARCHAR(MAX), NEWID()), CONVERT(NVARCHAR(MAX), NEWID()),
        0, 0, 1, 0, N'Sample Teacher'
    );
END
ELSE
    SELECT @TeacherId = Id FROM AspNetUsers WHERE NormalizedEmail = UPPER(@Email);

-- Map the sample user to the Teacher role.
IF NOT EXISTS (
    SELECT 1 FROM AspNetUserRoles ur
    JOIN AspNetRoles r ON r.Id = ur.RoleId
    WHERE ur.UserId = @TeacherId AND r.NormalizedName = N'TEACHER')
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @TeacherId, Id FROM AspNetRoles WHERE NormalizedName = N'TEACHER';
END

-- Assignments: Computer Science and Physics for PECTA -> 9th.
DECLARE @gradeId INT = (
    SELECT g.Id FROM Grades g
    WHERE g.[Name] = N'9th'
);
DECLARE @csId INT = (
    SELECT s.Id FROM Subjects s
    JOIN Grades g ON g.Id = s.GradeId
    JOIN Boards b ON b.Id = s.BoardId
    WHERE b.[Name] = N'PECTA' AND g.[Name] = N'9th' AND s.[Name] = N'Computer Science'
);
DECLARE @phyId INT = (
    SELECT s.Id FROM Subjects s
    JOIN Grades g ON g.Id = s.GradeId
    JOIN Boards b ON b.Id = s.BoardId
    WHERE b.[Name] = N'PECTA' AND g.[Name] = N'9th' AND s.[Name] = N'Physics'
);

IF @gradeId IS NOT NULL AND @csId IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.Teacher WHERE UserId = @TeacherId AND SubjectId = @csId AND GradeId = @gradeId)
    INSERT INTO dbo.Teacher (UserId, SubjectId, GradeId) VALUES (@TeacherId, @csId, @gradeId);

IF @gradeId IS NOT NULL AND @phyId IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.Teacher WHERE UserId = @TeacherId AND SubjectId = @phyId AND GradeId = @gradeId)
    INSERT INTO dbo.Teacher (UserId, SubjectId, GradeId) VALUES (@TeacherId, @phyId, @gradeId);
GO
