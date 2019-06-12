using System;
using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Events.Customers
{
    public sealed class CreateNewCustomerEvent : DomainEvent<Customer>
    {
        public override string EventName => GetType().Name;

        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Id", Id.ToString() },
                { "Prefix", Prefix },
                { "FirstName", FirstName },
                { "LastName", LastName },
                { "Title", Title }
            };
        }
    }
}