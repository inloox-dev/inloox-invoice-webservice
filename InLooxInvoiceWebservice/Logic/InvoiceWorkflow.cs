using InLoox.ODataClient.Data.BusinessObjects;
using InLooxInvoiceWebservice.Helpers;
using InLooxInvoiceWebservice.Models;
using InLooxInvoiceWebservice.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Logic
{
    public class InvoiceWorkflow : IWorkflow
    {
        private const string CUSTOM_FIELD_INVOICE_NUMBER = "Rechnungsnummer"; // <- change this if you like

        public async Task<ActionResult> RunAsync(IService service, IEntityModel entity)
        {
            // check configuration
            if (!(entity is IInvoiceModel invoice)
                || !(service is IInvoiceService invoiceService))
            {
                return ActionResultHelper.ServerErrorResult("Incorrect workflow configuration");
            }

            // find budget position
            var plannedExpenseBudgetPosition = await invoiceService.GetBudgetPosition(invoice.BudgetPositionId);
            if (plannedExpenseBudgetPosition == null)
            {
                return new NotFoundObjectResult(new { message = $"{nameof(BudgetPosition)} not found" });
            }

            // check if budget type is correct
            if (plannedExpenseBudgetPosition.BudgetType != ConstantsHelper.BUDGET_TYPE_PLANNED_EXPENSE)
            {
                return new BadRequestObjectResult(new { message = $"{nameof(BudgetPosition)} is not a planned expense" });
            }

            // look for custom field
            var customFieldInvoiceNumber = await invoiceService.GetCustomField(nameof(BudgetPosition), ConstantsHelper.CUSTOM_FIELD_TYPE_TEXT, CUSTOM_FIELD_INVOICE_NUMBER);
            if (customFieldInvoiceNumber == null)
            {
                return ActionResultHelper.ServerErrorResult($"Custom field '{CUSTOM_FIELD_INVOICE_NUMBER}' missing in {nameof(BudgetPosition)} entity");
            }

            // create invoice hyperlink
            var document = await invoiceService.CreateNewInvoiceDocumentLink(invoice.AttachmentLink, (Guid)plannedExpenseBudgetPosition.ProjectId);
            if (document == null)
            {
                return ActionResultHelper.ServerErrorResult("Failed creating invoice document");
            }

            // load target budget
            var actualExpenseBudget = await invoiceService.EnsureActualExpenseBudget(plannedExpenseBudgetPosition.BudgetId);
            if (actualExpenseBudget == null)
            {
                return ActionResultHelper.ServerErrorResult($"Failed creating/getting target {nameof(Budget)}");
            }

            // create budget position copy
            var actualExpenseBudgetPosition = await invoiceService.CreateNewBudgetPositionWithInvoiceNumber(
                actualExpenseBudget, plannedExpenseBudgetPosition, invoice.AmountEur,
                customFieldInvoiceNumber, invoice.InvoiceNumber);
            if (actualExpenseBudgetPosition == null)
            {
                return ActionResultHelper.ServerErrorResult($"Failed creating {nameof(BudgetPosition)}");
            }

            // link invoice hyperlink
            var relation = await invoiceService.CreateNewDocumentRelation(actualExpenseBudgetPosition, document);
            if (relation == null)
            {
                return ActionResultHelper.ServerErrorResult("Failed creating document link");
            }

            // success
            var result = new OkObjectResult(actualExpenseBudgetPosition.BudgetPositionId);
            return result;
        }
    }
}
