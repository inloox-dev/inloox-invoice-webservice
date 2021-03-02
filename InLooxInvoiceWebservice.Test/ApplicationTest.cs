using InLooxInvoiceWebservice.Logic;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InLooxInvoiceWebservice.Test
{
    [TestClass]
    public class ApplicationTest
    {
        [TestMethod]
        public void Container_Configuration_Ok()
        {
            // Arrange
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            var host = new TestServer(webHostBuilder);

            // Act
            var service = host.Services.GetService(typeof(IService));
            var workflow = host.Services.GetService(typeof(IWorkflow));
            var entity = host.Services.GetService(typeof(IEntityModel));

            // Assert
            Assert.IsInstanceOfType(workflow, typeof(InvoiceWorkflow), $"Container config for {nameof(IWorkflow)} is not {nameof(InvoiceWorkflow)}");
            Assert.IsInstanceOfType(service, typeof(InvoiceService), $"Container config for {nameof(IService)} is not {nameof(InvoiceService)}");
            Assert.IsInstanceOfType(entity, typeof(InvoiceModel), $"Container config for {nameof(IEntityModel)} is not {nameof(InvoiceModel)}");
        }
    }
}
