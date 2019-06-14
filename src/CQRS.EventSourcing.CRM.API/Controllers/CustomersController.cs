using System;
using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer;
using CQRS.EventSourcing.CRM.Application.Customers.Commands.UpdateCustomer;
using CQRS.EventSourcing.CRM.Application.Customers.Models;
using CQRS.EventSourcing.CRM.Application.Customers.Queries.GetCustomer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.EventSourcing.CRM.API.Controllers
{
    public class CustomersController : BaseController
    {
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> Get([FromRoute]Guid id)
        {
            return Ok(await Mediator.Send(new GetCustomerQuery { CustomerId = id }));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody]CreateCustomerCommand command)
        {
            var customerId = await Mediator.Send(command);

            var redirectResult = new RedirectToActionResult("Get", "Customers", new { id = customerId });
            return redirectResult;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateCustomerCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}