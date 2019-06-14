using Microsoft.AspNetCore.Mvc;

namespace CQRS.EventSourcing.CRM.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}