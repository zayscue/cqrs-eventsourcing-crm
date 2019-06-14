using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CQRS.EventSourcing.CRM.Application.Customers.Models;
using CQRS.EventSourcing.CRM.Application.Exceptions;
using CQRS.EventSourcing.CRM.Application.Interfaces;
using CQRS.EventSourcing.CRM.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRS.EventSourcing.CRM.Application.Customers.Queries.GetCustomer
{
    public class GetCustomerQuery : IRequest<CustomerDto>
    {
        public Guid CustomerId { get; set; }

        public class Handler : IRequestHandler<GetCustomerQuery, CustomerDto>
        {
            private readonly ICRMDbContext _context;
            private readonly IMapper _mapper;

            public Handler(ICRMDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CustomerDto> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
            {
                var entity = await _context.Customers
                    .SingleOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken: cancellationToken);

                if (entity == null)
                {
                    throw new NotFoundException(nameof(Customer), request.CustomerId);
                }

                var dto = _mapper.Map<CustomerDto>(entity);

                return dto;
            }
        }
    }
}
