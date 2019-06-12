using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Events
{
    public interface IDomainEvent
    {
        string EventName { get; }
        string EntityType { get; }
        Dictionary<string, string> SerializeData();
    }
}