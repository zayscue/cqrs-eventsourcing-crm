using System;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Events;
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

        public async Task SaveChange(Guid aggregateId, IDomainEvent @event)
        {
            var sql = @"InsertEvent";
            using (var db = _dbExecutorFactory.CreateExecutor())
            {
                await db.ExecuteAsync(sql, new
                {
                    AggregatId = aggregateId,
                    Type = @event.EntityType,
                    EventName = @event.EventName,
                    EventData = JsonConvert.SerializeObject(@event.SerializeData())
                }, null, null, CommandType.StoredProcedure);
            }
        }
    }
}