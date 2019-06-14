using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using MediatR;

namespace CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer
{
    public class CustomerCreated : INotification
    {
        public Guid AggregateId { get; set; }
        public Guid EventId { get; set; }
        public Domain.Actions.Customers.CreateCustomer Action { get; set; }

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
                    Created = @event.TimeStamp
                };
                var redux = new ReduxStore<Customer>(Customer.Reducer, entity);
                redux.Dispatch(notification.Action);

                await _context.Customers.AddAsync(entity, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}