using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public class UpdateCustomersLastName : CommandAction<Customer>
    {
        public string LastName { get; set; }

        public override string EventName => "UpdatedCustomersLastName";

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "LastName", LastName }
            };
        }
    }
}