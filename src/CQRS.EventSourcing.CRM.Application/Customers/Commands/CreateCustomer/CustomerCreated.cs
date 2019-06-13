using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using CQRS.EventSourcing.CRM.Domain.Events.Customers;
using MediatR;

namespace CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer
{
    public class CustomerCreated : INotification
    {
        public Guid AggregateId { get; set; }
        public Guid EventId { get; set; }
        public CustomerCreatedEvent DomainEvent { get; set; }

        public class CustomerCreatedHandler : INotificationHandler<CustomerCreated>
        {
            private readonly ICRMDbContext _context;
            private readonly IEventStore _eventStore;

            public CustomerCreatedHandler(ICRMDbContext context, IEventStore eventStore)
            {
                _context = context;
                _eventStore = eventStore;
            }

            public async Task Handle(CustomerCreated notification, CancellationToken cancellationToken)
            {
                var @event = await _eventStore.GetEvent(notification.EventId);

                var entity = new Customer
                {
                    Id = notification.AggregateId,
                    Prefix = notification.DomainEvent.Prefix,
                    FirstName = notification.DomainEvent.FirstName,
                    LastName = notification.DomainEvent.LastName,
                    Title = notification.DomainEvent.Title,
                    Created = @event.TimeStamp
                };

                await _context.Customers.AddAsync(entity, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}