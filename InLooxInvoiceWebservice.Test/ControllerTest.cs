using InLooxInvoiceWebservice.Controllers;
using InLooxInvoiceWebservice.Logic;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Test
{
    [TestClass]
    public class ControllerTest
    {
        [TestMethod]
        public async Task Post_ReturnsUnauthorized_WhenNoAccessToken()
        {
            // Arrange
            var mockWorkflow = new Mock<IWorkflow>();
            var mockService = new Mock<IService>();
            var mockEntity = new Mock<InvoiceModel>();
            var controller = new InvoiceController(mockWorkflow.Object, mockService.Object);

            // Act
            var result = await controller.Post(mockEntity.Object);

            // Assert
            Assert.IsInstanceOfType(result, typeof(UnauthorizedResult), "Missing authorization not blocked correctly");
        }
    }
}
