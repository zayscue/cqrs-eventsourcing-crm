using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public class UpdateCustomersPrefix : CommandAction<Customer>
    {
        public string Prefix { get; set; }

        public override string EventName => "UpdatedCustomersPrefix";

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Prefix", Prefix }
            };
        }
    }
}