using System;

namespace CQRS.EventSourcing.CRM.Domain
{
    public class Snapshot
    {
        public int Id { get; set; }
        public Guid AggregateId { get; set; }
        public string SerializedData { get; set; }
        public int Version { get; set; }
    }
}