using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using InLoox.ODataClient.Extensions;
using InLooxInvoiceWebservice.Helpers;
using IQmedialab.InLoox.Data.Api.Model.OData;
using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InLooxInvoiceWebservice.Services
{
    public class InvoiceService : Service, IInvoiceService
    {
        private const string SEE_INVOICE_HINT = "siehe Rechnung: {0}";

        #region Budget
        private async Task<BudgetExtend> GetBudget(Guid budgetId)
        {
            // create new query
            var budgetQuery = _context.budgetextend
                .Where(b => b.BudgetId == budgetId)
                .ToDataServiceQuery();

            // load from service, return
            var budget = await budgetQuery.ExecuteAsync();
            return budget?.FirstOrDefault();
        }

        private async Task<BudgetExtend> GetBudget(string name, int type, Guid projectId)
        {
            // create new query
            var budgetQuery = _context.budgetextend
                .Where(b => b.Name == name && b.BudgetType == type && b.ProjectId == projectId)
                .ToDataServiceQuery();

            // load from service, return
            var budget = await budgetQuery.ExecuteAsync();
            return budget?.FirstOrDefault();
        }

        public async Task<BudgetPositionExtend> GetBudgetPosition(Guid budgetPositionId)
        {
            // create new query
            var budgetPositionQuery = _context.budgetpositionextend
                .Where(b => b.BudgetPositionId == budgetPositionId)
                .ToDataServiceQuery();

            // load from service, return
            var budgetPosition = await budgetPositionQuery.ExecuteAsync();
            return budgetPosition?.FirstOrDefault();
        }

        public async Task<BudgetExtend> EnsureActualExpenseBudget(Guid plannedBudgetId)
        {
            var plannedBudget = await GetBudget(plannedBudgetId);
            var actualExpense = await GetBudget(plannedBudget?.Name, ConstantsHelper.BUDGET_TYPE_ACTUAL_EXPENSE, plannedBudget.ProjectId);

            if (actualExpense == null)
            {
                actualExpense = await CreateActualExpense(plannedBudget);
            }

            return actualExpense;
        }

        private async Task<BudgetExtend> CreateActualExpense(BudgetExtend plannedExpense)
        {
            if (plannedExpense == null)
                return null;

            // create new query
            var query = ODataBasics.GetDSCollection<BudgetExtend>(_context);
            var budget = new BudgetExtend();
            query.Add(budget);

            // set values
            budget.Name = plannedExpense.Name;
            budget.BudgetType = ConstantsHelper.BUDGET_TYPE_ACTUAL_EXPENSE;
            budget.ProjectId = plannedExpense.ProjectId;

            // save
            await _context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            return budget;
        }

        public async Task<BudgetPositionExtend> CreateNewBudgetPositionWithInvoiceNumber(BudgetExtend actualExpenseBudget,
            BudgetPositionExtend plannedExpenseBudgetPosition,
            double amount, CustomExpandDefaultExtend invoiceField, string invoiceFieldValue)
        {
            var position = await CreateNewBudgetPosition(actualExpenseBudget, plannedExpenseBudgetPosition, amount, invoiceFieldValue);
            await AttachInvoiceNumberToBudgetPosition(position, invoiceField, invoiceFieldValue);

            return position;
        }

        private async Task<BudgetPositionExtend> CreateNewBudgetPosition(BudgetExtend actualExpenseBudget,
            BudgetPositionExtend plannedExpenseBudgetPosition, double amount, string invoiceNumber)
        {
            // create new query
            var query = ODataBasics.GetDSCollection<BudgetPositionExtend>(_context);
            var actualExpenseBudgetPosition = new BudgetPositionExtend();
            query.Add(actualExpenseBudgetPosition);

            // set values
            actualExpenseBudgetPosition.BudgetId = actualExpenseBudget.BudgetId;
            actualExpenseBudgetPosition.ShortDescription = plannedExpenseBudgetPosition.ShortDescription;
            actualExpenseBudgetPosition.GroupId = plannedExpenseBudgetPosition.GroupId;
            actualExpenseBudgetPosition.PricePerUnit = amount;
            actualExpenseBudgetPosition.Quantity = 1;
            actualExpenseBudgetPosition.Unit = string.Format(SEE_INVOICE_HINT, invoiceNumber);
            actualExpenseBudgetPosition.IsBilled = true;

            // save
            await _context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            return actualExpenseBudgetPosition;
        }

        private async Task<CustomExpandExtend> AttachInvoiceNumberToBudgetPosition(BudgetPositionExtend position, CustomExpandDefaultExtend invoiceField, string invoiceFieldValue)
        {
            // create new queries
            var customExpands = await _context.customexpandextend.GetCustomExpand(position.BudgetPositionId);
            var customExpand = customExpands
                .FirstOrDefault(k => k.CustomExpandDefaultId == invoiceField.CustomExpandDefaultId);

            // save
            await _context.customexpandextend.PatchCustomExpand(customExpand.CustomExpandId, ConstantsHelper.CUSTOM_FIELD_TYPE_STRING_VALUE, invoiceFieldValue);
                        
            return customExpand;
        }
        #endregion

        #region Document
        public async Task<DocumentView> CreateNewInvoiceDocumentLink(Uri path, Guid projectId)
        {
            // create new query
            var query = ODataBasics.GetDSCollection<DocumentView>(_context);
            var document = new DocumentView();
            query.Add(document);

            // set values
            document.FileName = path.ToString();
            document.IsFileLink = false;
            document.IsInternetLink = true;
            document.ProjectId = projectId;

            // save
            await _context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            return document;
        }

        public async Task<DocumentRelationExtend> CreateNewDocumentRelation(BudgetPositionExtend actualExpenseBudgetPosition, DocumentView document)
        {
            // create new query
            var query = ODataBasics.GetDSCollection<DocumentRelationExtend>(_context);
            var relation = new DocumentRelationExtend();
            query.Add(relation);

            // set values
            relation.DocumentId = document.DocumentId;
            relation.ProjectId = document.ProjectId;
            relation.ObjectId = actualExpenseBudgetPosition.BudgetPositionId;

            // save
            await _context.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            return relation;
        }
        #endregion
    }
}
