using System;

namespace CQRS.EventSourcing.CRM.Domain.EventStore
{
    public class Aggregate
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public int Version { get; set; }
    }
}