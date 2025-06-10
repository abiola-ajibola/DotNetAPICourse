USE DotNetCourseDatabase;
GO

SELECT TOP(20) [UserId],
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
FROM TutorialAppSchema.UserJobInfo;
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