using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CQRS.EventSourcing.CRM.Application.Customers.Models;
using CQRS.EventSourcing.CRM.Domain.Entities;

namespace CQRS.EventSourcing.CRM.Application.Infrastructure.Mapper.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
        }
    }
}
