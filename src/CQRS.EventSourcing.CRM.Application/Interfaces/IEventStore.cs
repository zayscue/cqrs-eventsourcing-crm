using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Actions;
using CQRS.EventSourcing.CRM.Domain.EventStore;

namespace CQRS.EventSourcing.CRM.Application.Interfaces
{
    public interface IEventStore
    {
        Task<Guid> SaveChange(Guid aggregateId, ICommandAction action);
        Task<IEnumerable<Guid>> SaveChanges(Guid aggregateId, IEnumerable<ICommandAction> actions);
        Task<Event> GetEvent(Guid eventId);
        Task<IEnumerable<Event>> GetEvents(Guid aggregateId, int startRange, int endRange);
        Task<IEnumerable<(Guid AggregateId, int LastSnapshotVersion, int CurrentVersion)>> QuerySnapshotDeltas(string aggregateType);
        Task<Snapshot> GetSnapshot(Guid aggregateId, int version);
        Task SaveSnapshot(Snapshot snapshot);
    }
}