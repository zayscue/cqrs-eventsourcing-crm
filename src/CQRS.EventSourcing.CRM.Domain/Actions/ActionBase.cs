using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Actions
{
    public abstract class ActionBase<TEntity> : IAction where TEntity : class
    {
        public abstract string EventName { get; }

        public string EntityType => typeof(TEntity).FullName;

        public abstract Dictionary<string, string> SerializeData();
    }
}