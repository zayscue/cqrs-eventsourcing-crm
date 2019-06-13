using System.Threading.Tasks;
using CQRS.EventSourcing.CRM.Application.Customers.Commands.CreateCustomer;
using CQRS.EventSourcing.CRM.Application.Customers.Commands.UpdateCustomer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.EventSourcing.CRM.API.Controllers
{
    public class CustomersController : BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromBody]CreateCustomerCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody]UpdateCustomerCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}