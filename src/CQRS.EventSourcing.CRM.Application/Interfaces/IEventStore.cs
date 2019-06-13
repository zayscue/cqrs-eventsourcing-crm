using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Domain;
using CQRS.EventSourcing.CRM.Domain.Events;

namespace CQRS.EventSourcing.CRM.Application.Interfaces
{
    public interface IEventStore
    {
        Task<Guid> SaveChange(Guid aggregateId, IDomainEvent @event);
        Task<IEnumerable<Guid>> SaveChanges(Guid aggregateId, IEnumerable<IDomainEvent> @events);
        Task<Event> GetEvent(Guid eventId);
    }
}