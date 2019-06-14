using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public sealed class UpdateCustomersFirstName : ActionBase<Customer>
    {
        public static readonly string Event = "CustomersFirstNameUpdated";
        public override string EventName => Event;
        public string FirstName { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "FirstName", FirstName }
            };
        }
    }
}