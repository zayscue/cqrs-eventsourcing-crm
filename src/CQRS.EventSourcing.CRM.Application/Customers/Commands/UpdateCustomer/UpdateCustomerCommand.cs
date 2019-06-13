using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Exceptions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using CQRS.EventSourcing.CRM.Domain.Events;
using CQRS.EventSourcing.CRM.Domain.Events.Customers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.EventSourcing.CRM.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public class Handler : IRequestHandler<UpdateCustomerCommand, Unit>
        {
            private readonly ICRMDbContext _context;
            private readonly IEventStore _eventStore;
            private readonly IMediator _mediator;

            public Handler(ICRMDbContext context, IEventStore eventStore, IMediator mediator)
            {
                _context = context;
                _eventStore = eventStore;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                var entity = await _context.Customers.SingleOrDefaultAsync(x => x.Id == request.Id);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Customer), request.Id);
                }

                var domainEvents = new List<IDomainEvent>();
                if (!string.Equals(entity.FirstName, request.FirstName))
                    domainEvents.Add(new CustomerFirstNameChangedEvent { FirstName = request.FirstName });
                if (!string.Equals(entity.LastName, request.LastName))
                    domainEvents.Add(new CustomerLastNameChangedEvent { LastName = request.LastName });
                if (!string.Equals(entity.Prefix, request.Prefix))
                    domainEvents.Add(new CustomerPrefixChangedEvent { Prefix = request.Prefix });
                if (!string.Equals(entity.Title, request.Title))
                    domainEvents.Add(new CustomerTitleChangedEvent { Title = request.Title });

                var eventIds = await _eventStore.SaveChanges(request.Id, domainEvents);

                await _mediator.Publish(new CustomerUpdated
                {
                    AggregateId = request.Id,
                    EventIds = eventIds,
                    DomainEvents = domainEvents
                });

                return Unit.Value;
            }
        }
    }
}