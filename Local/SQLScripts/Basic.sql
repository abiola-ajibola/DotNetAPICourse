CREATE DATABASE DotNetCourseDatabase
GO

USE DotNetCourseDatabase
GO

CREATE SCHEMA TutorialAppSchema
GO

CREATE TABLE TutorialAppSchema.Computer
(
    ComputerId INT IDENTITY(1,1) PRIMARY KEY,
    Motherboard NVARCHAR(50),
    CPUCores INT,
    HasWifi BIT,
    HasLTE BIT,
    Price Decimal
    (18,4),
    VideoCard NVARCHAR
    (50)
)
GO

ALTER TABLE TutorialAppSchema.Computer ADD ReleaseDate DATETIME;
GO

EXEC sp_rename 'TutorialAppSchema.Computer', 'Computers';
GO

SELECT *
FROM TutorialAppSchema.Computers;
GO

-- The square brackets is for when your column names contain SQL keywords and special characters like a space.
/* INSERT INTO TutorialAppSchema.Computer
    ([Motherboard],
    [CPUCores],
    [HasWifi],
    [HasLTE],
    [Price],
    [VideoCard],
    [ReleaseDate])
VALUES
    (
        'Sample Board_2',
        12,
        1,
        0,
        800,
        'GeForce',
        '2002-01-01'
        );
GO */

-- This is the same as the above. No need for square brackets because there are no special characters.
INSERT INTO TutorialAppSchema.Computer
    (
    Motherboard,
    CPUCores,
    HasWifi,
    HasLTE,
    Price,
    VideoCard,
    ReleaseDate
    )
VALUES
    (
        'Sample Board_3',
        4,
        1,
        1,
        600,
        'GeForce RTX 1060',
        '2002-01-10'
    );
GO

DELETE FROM TutorialAppSchema.Computer WHERE ComputerId = 4;
GO

UPDATE TutorialAppSchema.Computer SET VideoCard = 'GeFoce RTX 5080' WHERE ComputerId = 5;
GO