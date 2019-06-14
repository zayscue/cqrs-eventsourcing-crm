using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Actions
{
    public abstract class CommandAction<TEntity> : ICommandAction where TEntity : class
    {
        public abstract string EventName { get; }

        public string EntityType => typeof(TEntity).FullName;

        public abstract Dictionary<string, string> SerializeData();
    }
}