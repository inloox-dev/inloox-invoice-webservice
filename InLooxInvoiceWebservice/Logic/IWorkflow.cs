using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Logic
{
    public interface IWorkflow
    {
        Task<ActionResult> RunAsync(IService service, IEntityModel entity);
    }
}