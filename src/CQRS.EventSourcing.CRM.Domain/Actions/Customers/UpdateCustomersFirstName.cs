using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public class UpdateCustomersFirstName : CommandAction<Customer>
    {
        public string FirstName { get; set; }

        public override string EventName => "UpdatedCustomersFirstName";

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "FirstName", FirstName }
            };
        }
    }
}