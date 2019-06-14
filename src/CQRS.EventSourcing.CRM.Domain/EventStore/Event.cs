using System;

namespace CQRS.EventSourcing.CRM.Domain.EventStore
{
    public class Event
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public Guid AggregateId { get; set; }
        public long Sequence { get; set; }
        public string Data { get; set; }
    }
}