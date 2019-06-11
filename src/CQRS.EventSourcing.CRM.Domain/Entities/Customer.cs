using System;
using System.Collections.Generic;

namespace CQRS.EventSourcing.CRM.Domain.Entities
{
    public class Customer
    {
        public Customer()
        {
            Addresses = new HashSet<Address>();
        }

        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}