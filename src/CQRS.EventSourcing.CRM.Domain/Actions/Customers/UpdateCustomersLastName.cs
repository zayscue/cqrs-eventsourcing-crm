using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public sealed class UpdateCustomersLastName : ActionBase<Customer>
    {
        public static readonly string Event = "CustomersLastNameUpdated";
        public override string EventName => Event;
        public string LastName { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "LastName", LastName }
            };
        }
    }
}