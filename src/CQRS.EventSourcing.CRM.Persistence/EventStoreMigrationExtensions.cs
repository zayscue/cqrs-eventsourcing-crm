using Microsoft.EntityFrameworkCore.Migrations;

namespace CQRS.EventSourcing.CRM.Persistence
{
    public static class EventStoreMigrationExtensions
    {
        public static MigrationBuilder MigrateEventStoreTables(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
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

            IF (NOT EXISTS (SELECT *
            FROM sys.procedures
            WHERE name = 'InsertEvent'))
            BEGIN
                EXECUTE('CREATE PROCEDURE dbo.InsertEvent
                    @AggregateId uniqueidentifier,
                    @Type nvarchar(200),
                    @EventName nvarchar(200),
                    @EventData nvarchar(max),
                    @TranName varchar(20) = ''InsertEvent''
                AS
                BEGIN
                    BEGIN TRANSACTION @TranName;

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
                END')
            END
            GO
            ");

            return migrationBuilder;
        }

        public static MigrationBuilder DropEventStoreTables(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            USE CRM
            GO

            IF (EXISTS (SELECT *
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Aggregates'))
            BEGIN
                DROP TABLE Aggregates
            END
            GO

            IF (EXISTS (SELECT *
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Events'))
            BEGIN
                DROP TABLE Events
            END
            GO

            IF (EXISTS (SELECT *
            FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Snapshots'))
            BEGIN
                DROP TABLE Snapshots
            END
            GO

            IF (EXISTS (SELECT *
            FROM sys.procedures
            WHERE name = 'InsertEvent'))
            BEGIN
                DROP PROCEDURE 'InsertEvent';
            END
            GO
            ");

            return migrationBuilder;
        }
    }
}