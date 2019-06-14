using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Exceptions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Actions;
using CQRS.EventSourcing.CRM.Domain.Actions.Customers;
using CQRS.EventSourcing.CRM.Domain.Entities;
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
                var entity = await _context.Customers.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Customer), request.Id);
                }

                var actions = new List<ICommandAction>();
                if (!string.Equals(entity.FirstName, request.FirstName))
                    actions.Add(new UpdateCustomersFirstName { FirstName = request.FirstName });
                if (!string.Equals(entity.LastName, request.LastName))
                    actions.Add(new UpdateCustomersLastName { LastName = request.LastName });
                if (!string.Equals(entity.Prefix, request.Prefix))
                    actions.Add(new UpdateCustomersPrefix { Prefix = request.Prefix });
                if (!string.Equals(entity.Title, request.Title))
                    actions.Add(new UpdateCustomersTitle { Title = request.Title });

                var eventIds = await _eventStore.SaveChanges(request.Id, actions);

                await _mediator.Publish(new CustomerUpdated
                {
                    AggregateId = request.Id,
                    EventIds = eventIds,
                    Actions = actions
                }, cancellationToken);

                return Unit.Value;
            }
        }
    }
}