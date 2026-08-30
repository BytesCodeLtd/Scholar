SET NOCOUNT ON;

-------------------------------------------------------------------------------
-- Seed the SuperAdmin role + user.
-- Requires the ASP.NET Identity tables (AspNetRoles, AspNetUsers, AspNetUserRoles).
-- Password: Admin@123
-------------------------------------------------------------------------------
DECLARE @UserId        NVARCHAR(450) = '2056d6c1-8bdc-46e5-a7ba-69e8369f63e3';
DECLARE @RoleId        NVARCHAR(450) = 'e0a21c9d-55ac-4c0d-a2f2-2a99199d91ea';
DECLARE @Email         NVARCHAR(256) = 'superadmin@scholar.local';
DECLARE @RoleName      NVARCHAR(256) = 'SuperAdmin';
DECLARE @PasswordHash  NVARCHAR(MAX) = 'AQAAAAIAAYagAAAAEBXCLax6CzQnRMxcVaAcVh/+CCkqBEpSaNRpcKhm/3QDGgvDTPnySDwR+Udxww3P/A=='; /*Admin@123*/

/* 1) Role */
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = UPPER(@RoleName))
BEGIN
    INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (@RoleId, @RoleName, UPPER(@RoleName), NEWID());
END
ELSE
BEGIN
    SELECT @RoleId = Id FROM AspNetRoles WHERE NormalizedName = UPPER(@RoleName);
END

/* 2) User */
IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE NormalizedEmail = UPPER(@Email))
BEGIN
    INSERT INTO AspNetUsers
    (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
        PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        FullName
    )
    VALUES
    (
        @UserId, @Email, UPPER(@Email), @Email, UPPER(@Email), 1,
        @PasswordHash, 'EFE663A20BBA461A8DA21D6D8D6619DC', NEWID(),
        0, 0, 1, 0,
        'Super Admin'
    );
END
ELSE
BEGIN
    SELECT @UserId = Id FROM AspNetUsers WHERE NormalizedEmail = UPPER(@Email);
END

/* 3) Map user -> role */
IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
END
GO
