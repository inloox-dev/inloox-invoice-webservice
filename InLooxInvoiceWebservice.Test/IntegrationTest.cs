using InLoox.ODataClient.Data.BusinessObjects;
using InLooxInvoiceWebservice.Controllers;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Test.Data;
using InLooxInvoiceWebservice.Test.Helper.Account;
using InLooxInvoiceWebservice.Test.Helper.General;
using InLooxInvoiceWebservice.Test.Models;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Test
{
    [TestClass]
    public class IntegrationTest
    {
        // account token
        private string _token;
        private Guid _projectId;
        private Guid _budgetIdCorrect;
        private Guid _budgetIdWrongType;
        private Guid _budgetPositionIdCorrect;
        private Guid _budgetPositionIdWrongType;

        [TestInitialize]
        public async Task CreateAccountAndFillWithData() {
            // check if initialized
            if (_budgetPositionIdCorrect != Guid.Empty
                && _budgetPositionIdWrongType != Guid.Empty) { 
                return;
            }

            await CreateAccount();
            await CreateCustomFields();
            await CreateProject();
            await CreateBudgets();
            await CreateBudgetPositions();
        } 
        
        private async Task CreateAccount() {
            // Arrange
            var accountLogic = new CreateAccountLogic();
            var accountModel = new CreateAccountModel
            {
                Name = "Test User",
                Password = "PasswortPasswort1010" + new Random().Next(),
                Email = "noreply@inloox.com",
                Language = "de"
            };
            
            Debug.WriteLine(accountModel.Email);
            Debug.WriteLine(accountModel.Password);

            // Act
            _token = await accountLogic.GetApprovedAccountToken(accountModel);
            var request = $"https://app.inlooxnow.com/odata/projectview?{ParameterHelper.GetTokenParameter(_token)}";
            var projectList = await new HttpTestClient().Get(request);

            // Assert
            Assert.IsTrue(projectList.Contains("InLoox Testphase"), $"Test account not created correctly, message: {projectList}");
        }

        private async Task CreateCustomFields()
        {
            // Arrange
            var field = "Rechnungsnummer";
            var view = nameof(CustomExpandDefaultExtend).ToLower();
            var input = new CustomExpandModel()
            {
                TableName = nameof(BudgetPosition),
                DisplayName = field,
                ColumnType = 0
            };

            // Act
            var text = await new HttpTestClient().Post(_token, view, input);
            var output = JsonConvert.DeserializeObject<CustomExpandDefaultExtend>(text);

            // Assert
            Assert.AreEqual(output.DisplayName, field, "Test custom field not created correctly");
        }

        private async Task CreateProject()
        {
            // Arrange
            var view = nameof(ProjectView).ToLower();
            var input = new TestProjectModel() {
                Name = "Testproject"
            };

            // Act
            var text = await new HttpTestClient().Post(_token, view, input);
            var output = JsonConvert.DeserializeObject<ProjectView>(text);
            _projectId = (Guid)output?.ProjectId;

            // Assert
            Assert.AreEqual(output.Name, input.Name, "Testproject not created correctly");
        }

        private async Task CreateBudgets()
        {
            _budgetIdCorrect = await CreateBudgetSingle("Testbudget OK", 5);
            _budgetIdWrongType = await CreateBudgetSingle("Testbudget Wrong Type", 3);
        }

        private async Task<Guid> CreateBudgetSingle(string name, int type)
        {
            // Arrange
            var view = nameof(BudgetExtend).ToLower();
            var input = new TestBudgetModel()
            {
                Name = name,
                ProjectId = _projectId,
                BudgetType = type
            };

            // Act
            var text = await new HttpTestClient().Post(_token, view, input);
            var output = JsonConvert.DeserializeObject<BudgetExtend>(text);

            // Assert
            Assert.AreEqual(output.Name, input.Name, "Testbudget not created correctly");

            return output.BudgetId;
        }

        private async Task CreateBudgetPositions()
        {
            _budgetPositionIdCorrect = await CreateBudgetPositionSingle("Testposition OK", _budgetIdCorrect);
            _budgetPositionIdWrongType = await CreateBudgetPositionSingle("Testposition WRONG", _budgetIdWrongType);
        }

        private async Task<Guid> CreateBudgetPositionSingle(string name, Guid budgetId)
        {
            // Arrange
            var view = nameof(BudgetPositionExtend).ToLower();
            var input = new TestBudgetPositionModel()
            {
                ShortDescription = name,
                ProjectId = _projectId,
                BudgetId = budgetId,
                Quantity = 1
            };

            // Act
            var text = await new HttpTestClient().Post(_token, view, input);
            var output = JsonConvert.DeserializeObject<BudgetPositionExtend>(text);

            // Assert
            Assert.AreEqual(output.ShortDescription, input.ShortDescription, "Testposition not created correctly");

            return output.BudgetPositionId;
        }

        private async Task<Tuple<ObjectResult, BudgetPositionExtend>> PostInvoiceAndGetBudgetPosition(InvoiceController controller, InvoiceModel invoice)
        {
            var response = await controller.Post(invoice) as ObjectResult;
            var request = $"https://app.inlooxnow.com/odata/budgetpositionextend?$filter=(BudgetPositionId+eq+{response.Value})&{ParameterHelper.GetTokenParameter(_token)}";
            var answer = await new HttpTestClient().Get(request);
            var output = new ParserHelper<BudgetPositionExtend>().GetObjectSingle(answer);
            return new Tuple<ObjectResult, BudgetPositionExtend>(response, output);
        }

        [TestMethod]
        public async Task Post_ReturnsOk_WhenPostingGoodTestData()
        {
            // Arrange
            var controller = new ControllerHelper().GetNewController(_token);
            var invoice = TestData.GetTestInvoice_Good(_budgetPositionIdCorrect);

            // Act
            var output = await PostInvoiceAndGetBudgetPosition(controller, invoice);

            // Assert
            Assert.IsInstanceOfType(output.Item1, typeof(OkObjectResult), "Good test data produces error");
            Assert.IsTrue(output.Item2.Unit.Contains("siehe Rechnung"), "Target position does not have the correct unit");
            Assert.AreEqual(output.Item2.IsBilled, true, "Target position does not have the correct billing state");
            Assert.AreEqual(output.Item2.Quantity, 1, "Target position does not have the correct quantity");
            Assert.AreEqual(output.Item2.PricePerUnit, invoice.AmountEur, "Target position does not have the correct amount");
            //Assert.AreEqual(output.Item2.ProjectId, )
            //TODO: add: projectid, link, group, custom field
        }

        [TestMethod]
        public async Task Post_ReturnsNotFound_WhenPostingNonexistentBudgetPosition()
        {
            // Arrange
            var controller = new ControllerHelper().GetNewController(_token);
            var invoice = TestData.GetTestInvoice_NonexistentBudgetPosition();

            // Act
            var response = await controller.Post(invoice) as ObjectResult;

            // Assert
            Assert.IsInstanceOfType(response, typeof(NotFoundObjectResult), "Nonexistent BudgetPosition produces no error");
        }

        [TestMethod]
        public async Task Post_ReturnsBadRequest_WhenPostingNoPlannedExpenseBudgetPosition()
        {
            // Arrange
            var controller = new ControllerHelper().GetNewController(_token);
            var invoice = TestData.GetTestInvoice_NoPlannedExpenseBudgetPosition(_budgetPositionIdWrongType);

            // Act
            var response = await controller.Post(invoice) as ObjectResult;

            // Assert
            Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult), "Wrong type of BudgetPosition produces no error");
        }

        [TestMethod]
        public async Task Post_ReturnsError_WhenPostingMalformedData()
        {
            // Arrange
            var controller = new ControllerHelper().GetNewController(_token);
            var invoiceNoBudgetPosition = TestData.GetTestInvoice_MissingBudgetPosition();
            var invoiceNoInvoiceNumber = TestData.GetTestInvoice_MissingInvoiceNumber(_budgetPositionIdCorrect);
            var invoiceNoLink = TestData.GetTestInvoice_MissingAttachmentLink(_budgetPositionIdCorrect);
            //var invoiceNegativeAmount = TestData.GetTestInvoice_NegativeAmount(_budgetPositionIdCorrect);
            //var invoiceZeroAmount = TestData.GetTestInvoice_ZeroAmount(_budgetPositionIdCorrect);
            //var invoiceNoAmount = TestData.GetTestInvoice_MissingAmount(_budgetPositionIdCorrect);

            // Act
            var responseNoBudgetPosition = await controller.Post(invoiceNoBudgetPosition) as ObjectResult;
            var responseNoInvoiceNumber = await controller.Post(invoiceNoInvoiceNumber) as ObjectResult;
            var responseNoLink = await controller.Post(invoiceNoLink) as ObjectResult;
            //TODO: If only the interface would work on the controller, we could use TestInvoiceModel in the data...
            //var responseNegativeAmount = await controller.Post(invoiceNegativeAmount) as ObjectResult;
            //var responseZeroAmount = await controller.Post(invoiceZeroAmount) as ObjectResult;
            //var responseNoAmount = await controller.Post(invoiceNoAmount) as ObjectResult;

            // Assert
            Assert.IsInstanceOfType(responseNoBudgetPosition, typeof(NotFoundObjectResult), "Missing BudgetPosition produces no error");
            Assert.IsInstanceOfType(responseNoInvoiceNumber, typeof(BadRequestObjectResult), "Missing invoice number produces no error");
            Assert.IsInstanceOfType(responseNoLink, typeof(BadRequestObjectResult), "Missing link produces no error");
            //Assert.IsInstanceOfType(responseNegativeAmount, typeof(BadRequestObjectResult), "Negative amount produces no error");
            //Assert.IsInstanceOfType(responseZeroAmount, typeof(BadRequestObjectResult), "Zero amount test data produces no error");
            //Assert.IsInstanceOfType(responseNoAmount, typeof(BadRequestObjectResult), "Missing amount produces no error");
        }
    }
}
