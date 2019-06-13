using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Events.Customers
{
    public class CustomerLastNameChangedEvent : DomainEvent<Customer>
    {
        public string LastName { get; set; }

        public override string EventName => GetType().Name;

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "LastName", LastName }
            };
        }
    }
}