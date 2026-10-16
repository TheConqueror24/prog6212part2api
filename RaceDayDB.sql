USE RaceDayDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EVENT' and xtype='U')
BEGIN
    CREATE TABLE EVENT (
        EventID INT IDENTITY(1,1) PRIMARY KEY,
        OrganizerID INT NOT NULL,
        EventName VARCHAR(150) NOT NULL,
        EventDate DATETIME NOT NULL,
        Location VARCHAR(150) NOT NULL,
        Description VARCHAR(255)
    );
END
GO