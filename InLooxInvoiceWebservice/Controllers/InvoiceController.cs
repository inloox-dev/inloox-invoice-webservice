using Microsoft.AspNetCore.Mvc;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Logic;
using System.Threading.Tasks;
using InLooxInvoiceWebservice.Services;
using InLooxInvoiceWebservice.Helpers;

namespace InLooxInvoiceWebservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoiceController : Controller
    {
        private readonly IWorkflow _workflow;
        private readonly IService _service;

        public InvoiceController(IWorkflow workflow, IService service)
        {
            _workflow = workflow;
            _service = service;
        }

        // POST <controller>
        [HttpPost]
        public async Task<ActionResult> Post(InvoiceModel invoice)
        {
            // check for authorization
            Request?.Headers?.TryGetValue(ConstantsHelper.ACCESS_TOKEN_PARAMETER_NAME, out var token);
            if (string.IsNullOrWhiteSpace(token)
                || !await _service?.Connect(token))
            {
                return Unauthorized();
            }

            // execute
            return await _workflow?.RunAsync(_service, invoice);
        }
    }
}
