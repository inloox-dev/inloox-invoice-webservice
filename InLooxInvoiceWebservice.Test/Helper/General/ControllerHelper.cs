using InLooxInvoiceWebservice.Controllers;
using InLooxInvoiceWebservice.Helpers;
using InLooxInvoiceWebservice.Logic;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;

namespace InLooxInvoiceWebservice.Test.Helper.General
{
    public class ControllerHelper
    {
        public InvoiceController GetNewController(string token)
        {
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            var host = new TestServer(webHostBuilder);
            var service = (InvoiceService)host.Services.GetService(typeof(IService));
            var workflow = (InvoiceWorkflow)host.Services.GetService(typeof(IWorkflow));
            var controller = new InvoiceController(workflow, service);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.ControllerContext.HttpContext.Request.Headers[ConstantsHelper.ACCESS_TOKEN_PARAMETER_NAME] = token;
            return controller;
        }
    }
}
