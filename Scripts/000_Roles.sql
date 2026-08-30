SET NOCOUNT ON;

INSERT INTO dbo.AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
SELECT CONVERT(NVARCHAR(450), NEWID()), v.[Name], UPPER(v.[Name]), CONVERT(NVARCHAR(MAX), NEWID())
FROM (VALUES
    (N'SuperAdmin'),
    (N'InstituteAdmin'),
    (N'Teacher')
) AS v([Name])
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.AspNetRoles r WHERE r.NormalizedName = UPPER(v.[Name])
);
GO
