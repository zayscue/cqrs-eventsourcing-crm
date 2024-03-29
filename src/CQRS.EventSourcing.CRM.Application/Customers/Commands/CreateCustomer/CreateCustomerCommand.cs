using System;
using System.Threading;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using MediatR;

namespace CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>
    {
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }

        public class Handler : IRequestHandler<CreateCustomerCommand, Guid>
        {
            private readonly IEventStore _eventStore;
            private readonly IMediator _mediator;

            public Handler(IEventStore eventStore, IMediator mediator)
            {
                _eventStore = eventStore;
                _mediator = mediator;
            }

            public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var aggregateId = Guid.NewGuid();

                var action = new Domain.Actions.Customers.CreateCustomer
                {
                    Prefix = request.Prefix,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Title = request.Title
                };

                var eventId = await _eventStore.SaveChange(aggregateId, action);

                await _mediator.Publish(new CustomerCreated
                {
                    AggregateId = aggregateId,
                    EventId = eventId,
                    Action = action
                }, cancellationToken);

                return aggregateId;
            }
        }
    }
}