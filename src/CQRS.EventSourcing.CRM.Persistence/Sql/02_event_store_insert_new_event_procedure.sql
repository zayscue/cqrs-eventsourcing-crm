DECLARE @Type nvarchar(max) = 'CQRS.EventSourcing.CRM.Domain.Entities.Customer';
DECLARE @EventName nvarchar(max) = 'CustomerCreated';
DECLARE @EventData nvarchar(max) = '{
  "Id": "a8efbab0-bf2e-405e-96a6-bdf7ab56cfb4",
  "Prefix": "Mr.",
  "FirstName": "Zackery",
  "LastName": "Ayscue",
  "Title": "Software Engineer"
}';
DECLARE @AggregateIdString nvarchar(36) = N'a8efbab0-bf2e-405e-96a6-bdf7ab56cfb4';
DECLARE @AggregateId uniqueidentifier;
SET @AggregateId = CONVERT([uniqueidentifier], @AggregateIdString);

DECLARE @TranName VARCHAR(20);
SELECT @TranName = 'InsertNewEvent';

BEGIN TRANSACTION @TranName;
USE CRM;

DECLARE @Version int;

SELECT @Version = Version FROM dbo.Aggregates WHERE Id = @AggregateId

IF @Version IS NULL
BEGIN
    SET @Version = 1;
    INSERT INTO dbo.Aggregates ([Id], [Type], [Version]) VALUES (@AggregateId, @Type, @Version);
END
ELSE 
BEGIN
    SET @Version = @Version + 1;
END

INSERT INTO dbo.Events ([Id],[TimeStamp],[Name],[Version],[AggregateId],[Data])
VALUES (NEWID(), GETUTCDATE(), @EventName, @Version, @AggregateId, @EventData);

UPDATE dbo.Aggregates SET [Version] = @Version WHERE Id = @AggregateId

COMMIT TRANSACTION @TranName;
GO