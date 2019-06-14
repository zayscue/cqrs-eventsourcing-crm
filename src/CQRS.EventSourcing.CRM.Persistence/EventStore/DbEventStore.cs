using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Actions;
using CQRS.EventSourcing.CRM.Domain.EventStore;
using Dapper;
using Dapper.Abstractions;
using Newtonsoft.Json;

namespace CQRS.EventSourcing.CRM.Persistence.EventStore
{
    public class DbEventStore : IEventStore
    {
        private readonly IDbExecutorFactory _dbExecutorFactory;

        public DbEventStore(IDbExecutorFactory dbExecutorFactory)
        {
            _dbExecutorFactory = dbExecutorFactory ?? throw new ArgumentNullException(nameof(dbExecutorFactory));
        }

        public async Task<IEnumerable<(Guid AggregateId, int LastSnapshotVersion, int CurrentVersion)>> QuerySnapshotDeltas(string aggregateType)
        {
            var sql = @"SELECT  aggregates.[Id] as AggregateId,
                                ISNULL(snapshots.[Version], 0) as LastSnapShotVersion,
                                aggregates.[Version] as CurrentVersion
                        FROM [CRM].[dbo].[Aggregates] as aggregates
                        LEFT JOIN (
                            SELECT [AggregateId], max([Version]) as Version
                            FROM [CRM].[dbo].[Snapshots]
                            GROUP BY [AggregateId]
                        ) as snapshots
                        ON snapshots.[AggregateId] = aggregates.[Id]
                        WHERE aggregates.[Type] = @AggregateType
                            AND (aggregates.[Version] - ISNULL(snapshots.[Version], 0)) >= 2";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                return await db.QueryAsync<(Guid AggregateId, int LastSnapShotVersion, int CurrentVersion)>(
                    sql, new { AggregateType = aggregateType });
            }
        }

        public async Task<Event> GetEvent(Guid eventId)
        {
            var sql = @"SELECT [Id]
                            ,[TimeStamp]
                            ,[Name]
                            ,[Version]
                            ,[AggregateId]
                            ,[Sequence]
                            ,[Data]
                        FROM [CRM].[dbo].[Events]
                        WHERE [Id] = @EventId";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                return await db.QuerySingleAsync<Event>(sql, new { EventId = eventId });
            }
        }

        public async Task<IEnumerable<Event>> GetEvents(Guid aggregateId, int startRange, int endRange)
        {
            var sql = @"SELECT  events.[Id],
                                events.[TimeStamp],
                                events.[Name],
                                events.[Version],
                                events.[AggregateId],
                                events.[Sequence],
                                events.[Data]
                        FROM [CRM].[dbo].[Events] as events
                        WHERE events.[AggregateId] = @AggregateId
                            AND events.[Version] > @StartRange
                            AND events.[Version] <= @EndRange";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                return await db.QueryAsync<Event>(sql, new
                {
                    AggregateId = aggregateId,
                    StartRange = startRange,
                    EndRange = endRange
                });
            }
        }

        public async Task<Guid> SaveChange(Guid aggregateId, ICommandAction action)
        {
            var sql = @"InsertEvent";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AggregateId", aggregateId, dbType: DbType.Guid, direction: ParameterDirection.Input);
                parameters.Add("@Type", action.EntityType, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventName", action.EventName, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventData", JsonConvert.SerializeObject(action.SerializeData()), dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventId", dbType: DbType.Guid, direction: ParameterDirection.Output);
                await db.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<Guid>("@EventId");
            }
        }

        public async Task<IEnumerable<Guid>> SaveChanges(Guid aggregateId, IEnumerable<ICommandAction> actions)
        {
            var eventIds = new List<Guid>();
            foreach (var action in actions)
            {
                var eventId = await SaveChange(aggregateId, action);
                eventIds.Add(eventId);
            }
            return eventIds;
        }

        public async Task<Snapshot> GetSnapshot(Guid aggregateId, int version)
        {
            var sql = @"SELECT  [Id]
                                ,[AggregateId]
                                ,[SerializedData]
                                ,[Version]
                        FROM [CRM].[dbo].[Snapshots]
                        WHERE [AggregateId] = @AggregateId
                            AND [Version] = @Version";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                return await db.QuerySingleOrDefaultAsync<Snapshot>(sql, new
                {
                    AggregateId = aggregateId,
                    Version = version
                });
            }
        }

        public async Task SaveSnapshot(Snapshot snapshot)
        {
            var sql = @"INSERT INTO [CRM].[dbo].[Snapshots] ([AggregateId] ,[SerializedData] ,[Version])
                        VALUES (@AggregateId, @SerializedData, @Version)";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                await db.ExecuteAsync(sql, new
                {
                    AggregateId = snapshot.AggregateId,
                    SerializedData = snapshot.SerializedData,
                    Version = snapshot.Version
                });
            }
        }
    }
}