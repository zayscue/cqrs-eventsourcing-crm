using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Events
{
    public abstract class DomainEvent<TEntity> : IDomainEvent where TEntity : class
    {
        public abstract string EventName { get; }

        public string EntityType => typeof(TEntity).FullName;

        public abstract Dictionary<string, string> SerializeData();
    }
}