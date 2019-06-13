using System;
using System.Collections.Generic;
using CQRS.EventSourcing.CRM.Domain.Events.Customers;
using ActionType = System.Object;

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

        public static Customer Reducer(Customer state, ActionType action)
        {
            switch (action)
            {
                case DateTime createdTimestamp:
                    state.Created = createdTimestamp;
                    return state;
                case CustomerCreatedEvent created:
                    state.FirstName = created.FirstName;
                    state.LastName = created.LastName;
                    state.Prefix = created.Prefix;
                    state.Title = created.Title;
                    return state;
                case CustomerFirstNameChangedEvent firstNameChanged:
                    state.FirstName = firstNameChanged.FirstName;
                    return state;
                case CustomerLastNameChangedEvent lastNameChanged:
                    state.LastName = lastNameChanged.LastName;
                    return state;
                case CustomerPrefixChangedEvent prefixChanged:
                    state.Prefix = prefixChanged.Prefix;
                    return state;
                case CustomerTitleChangedEvent titleChanged:
                    state.Title = titleChanged.Title;
                    return state;
                default:
                    return state;
            }
        }
    }
}