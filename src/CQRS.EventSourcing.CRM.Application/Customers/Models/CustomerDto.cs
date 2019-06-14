using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.EventSourcing.CRM.Application.Customers.Models
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
