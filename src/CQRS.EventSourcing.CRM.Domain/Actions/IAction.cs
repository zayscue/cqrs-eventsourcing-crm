using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Actions
{
    public interface IAction
    {
        string EventName { get; }
        string EntityType { get; }
        Dictionary<string, string> SerializeData();
    }
}