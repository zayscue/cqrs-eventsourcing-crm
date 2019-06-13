using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Exceptions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using CQRS.EventSourcing.CRM.Domain.Events;
using MediatR;

namespace CQRS.EventSourcing.CRM.Application.Customers.Commands.UpdateCustomer
{
    public class CustomerUpdated : INotification
    {
        public Guid AggregateId { get; set; }
        public IEnumerable<Guid> EventIds { get; set; }
        public IEnumerable<IDomainEvent> DomainEvents { get; set; }

        public class CustomerUpdatedHandler : INotificationHandler<CustomerUpdated>
        {
            private readonly ICRMDbContext _context;
            private readonly IEventStore _eventStore;

            public CustomerUpdatedHandler(ICRMDbContext context, IEventStore eventStore)
            {
                _context = context;
                _eventStore = eventStore;
            }

            public async Task Handle(CustomerUpdated notification, CancellationToken cancellationToken)
            {
                var entity = await _context.Customers.FindAsync(notification.AggregateId);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Customer), notification.AggregateId);
                }

                var redux = new ReduxStore<Customer>(Customer.Reducer, entity);
                foreach (var domainEvent in notification.DomainEvents)
                {
                    redux.Dispatch(domainEvent);
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

    }
}