USE DotNetCourseDatabase;
GO

SELECT TOP(20)
    [UserId],
    [FirstName],
    [LastName],
    [Email],
    [Gender],
    [Active]
FROM TutorialAppSchema.Users
-- WHERE Active = 1
ORDER BY UserId DESC;
GO

SELECT [UserId],
    [Salary],
    [AvgSalary]
FROM TutorialAppSchema.UserSalary;
GO

SELECT [UserId],
    [JobTitle],
    [Department]
FROM TutorialAppSchema.UserJobInfo
-- WHERE UserId = 5;
ORDER BY UserId DESC;
GO

SELECT [ComputerId],
    [Motherboard],
    [CPUCores],
    [HasWifi],
    [HasLTE],
    [Price],
    [VideoCard],
    [ReleaseDate]
FROM TutorialAppSchema.Computers;

SELECT Users.UserId,
    Users.FirstName,
    Users.LastName,
    UserJobInfo.JobTitle,
    UserJobInfo.Department
FROM TutorialAppSchema.Users AS Users
    LEFT JOIN TutorialAppSchema.UserJobInfo AS UserJobInfo
    ON UserJobInfo.UserId = Users.UserId
ORDER BY Users.UserId DESC;
GO;

-- AUTH
CREATE TABLE TutorialAppSchema.Auth
(
    Email NVARCHAR(50) PRIMARY KEY,
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX)
)
GO;

-- This one is for testing with string based hash
CREATE TABLE TutorialAppSchema.Auth2
(
    Email NVARCHAR(50) PRIMARY KEY,
    PasswordHash NVARCHAR(512),
);
GO;

SELECT
    [Email],
    [PasswordHash],
    [PasswordSalt]
FROM TutorialAppSchema.Auth;
GO;

SELECT
    [Email],
    [PasswordHash]
FROM TutorialAppSchema.Auth2;

SELECT
    [PasswordHash],
    [Email]
FROM TutorialAppSchema.Auth2
WHERE Email = 'vickybecks@aol.com'

DELETE FROM TutorialAppSchema.Auth2
WHERE Auth2.Email = 'vickybecks@aol.com';

DELETE FROM TutorialAppSchema.Users
WHERE Users.Email = 'vickybecks@aol.com';



/* 

{
  "email": "vickybecks@aol.com",
  "password": "123456",
  "passwordConfirm": "123456",
  "firstName": "Victoria",
  "lastName": "Beckham",
  "gender": "Female"
}
 */