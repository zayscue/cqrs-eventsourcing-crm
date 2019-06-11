USE CRM
GO

IF (NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Aggregates'))
BEGIN
    CREATE TABLE Aggregates
    (
        Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        Type [nvarchar](255) NOT NULL,
        Version [int] NOT NULL DEFAULT 1,
    )
END
GO

IF (NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Events'))
BEGIN
    CREATE TABLE Events
    (
        Id UNIQUEIDENTIFIER DEFAULT NEWID() PRIMARY KEY,
        TimeStamp [datetime] NOT NULL,
        Name [varchar](max) NOT NULL,
        Version [int] NOT NULL,

        AggregateId [uniqueidentifier] NOT NULL,
        Sequence [bigint] NOT NULL IDENTITY,

        Data [nvarchar](max) NOT NULL,
        FOREIGN KEY (AggregateId) REFERENCES Aggregates(Id)
    )
END
GO

IF (NOT EXISTS (SELECT *
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Snapshots'))
BEGIN
    CREATE TABLE Snapshots
    (
        Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        AggregateId [uniqueidentifier] NOT NULL,
        SerializedData [varchar](max) NOT NULL,
        Version [int] NOT NULL,

        FOREIGN KEY (AggregateId) REFERENCES Aggregates(Id)
    )
END
GO
