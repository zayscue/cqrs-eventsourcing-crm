using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public class UpdateCustomersTitle : CommandAction<Customer>
    {
        public string Title { get; set; }

        public override string EventName => "UpdatedCustomersTitle";

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Title", Title }
            };
        }
    }
}