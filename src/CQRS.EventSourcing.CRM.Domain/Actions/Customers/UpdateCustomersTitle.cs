using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public sealed class UpdateCustomersTitle : CommandAction<Customer>
    {
        public static readonly string Event = "CustomersTitleUpdated";
        public override string EventName => Event;
        public string Title { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Title", Title }
            };
        }
    }
}