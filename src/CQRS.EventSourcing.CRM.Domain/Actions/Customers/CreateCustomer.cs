using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public sealed class CreateCustomer : ActionBase<Customer>
    {
        public static readonly string Event = "CustomerCreated";
        public override string EventName => Event;
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Prefix", Prefix },
                { "FirstName", FirstName },
                { "LastName", LastName },
                { "Title", Title }
            };
        }
    }
}