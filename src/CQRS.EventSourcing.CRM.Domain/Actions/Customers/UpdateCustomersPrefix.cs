using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Domain.Actions.Customers
{
    public sealed class UpdateCustomersPrefix : ActionBase<Customer>
    {
        public static readonly string Event = "CustomersPrefixUpdated";
        public override string EventName => Event;
        public string Prefix { get; set; }

        public override Dictionary<string, string> SerializeData()
        {
            return new Dictionary<string, string>
            {
                { "Prefix", Prefix }
            };
        }
    }
}