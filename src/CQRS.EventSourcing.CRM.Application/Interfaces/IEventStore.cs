using System;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Domain.Events;

namespace CQRS.EventSourcing.CRM.Application.Interfaces
{
    public interface IEventStore
    {
        Task SaveChange(Guid aggregateId, IDomainEvent @event);
    }
}