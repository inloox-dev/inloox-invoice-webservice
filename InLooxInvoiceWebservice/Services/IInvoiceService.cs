using InLoox.ODataClient.Data.BusinessObjects;
using IQmedialab.InLoox.Data.Api.Model.OData;
using System;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Services
{
    public interface IInvoiceService
    {
        Task<BudgetPositionExtend> CreateNewBudgetPositionWithInvoiceNumber(
            BudgetExtend actualExpenseBudget,
            BudgetPositionExtend plannedExpenseBudgetPosition,
            double amount,
            CustomExpandDefaultExtend invoiceField,
            string invoiceFieldValue);
        Task<DocumentRelationExtend> CreateNewDocumentRelation(BudgetPositionExtend actualExpenseBudgetPosition, DocumentView document);
        Task<DocumentView> CreateNewInvoiceDocumentLink(Uri path, Guid projectId);
        Task<BudgetExtend> EnsureActualExpenseBudget(Guid plannedBudgetId);
        Task<BudgetPositionExtend> GetBudgetPosition(Guid budgetPositionId);
        Task<CustomExpandDefaultExtend> GetCustomField(string entity, int type, string name);
    }
}