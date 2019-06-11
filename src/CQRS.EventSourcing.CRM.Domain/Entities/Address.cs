using System;

namespace CQRS.EventSourcing.CRM.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public string StreetAddress { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string StateOrProvince { get; set; }
        public string Country { get; set; }
        public string ISOCountryCode { get; set; }
        public string PostalCode { get; set; }

        public Customer Customer { get; set; }
    }
}