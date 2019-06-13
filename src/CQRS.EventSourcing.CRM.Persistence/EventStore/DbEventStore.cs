using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Events;
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

        public async Task<Guid> SaveChange(Guid aggregateId, IDomainEvent @event)
        {
            var sql = @"InsertEvent";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AggregateId", aggregateId, dbType: DbType.Guid, direction: ParameterDirection.Input);
                parameters.Add("@Type", @event.EntityType, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventName", @event.EventName, dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventData", JsonConvert.SerializeObject(@event.SerializeData()), dbType: DbType.String, direction: ParameterDirection.Input);
                parameters.Add("@EventId", dbType: DbType.Guid, direction: ParameterDirection.Output);
                await db.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<Guid>("@EventId");
            }
        }

        public async Task<IEnumerable<Guid>> SaveChanges(Guid aggregateId, IEnumerable<IDomainEvent> @events)
        {
            var eventIds = new List<Guid>();
            foreach (var @event in @events)
            {
                var eventId = await SaveChange(aggregateId, @event);
                eventIds.Add(eventId);
            }
            return eventIds;
        }
    }
}