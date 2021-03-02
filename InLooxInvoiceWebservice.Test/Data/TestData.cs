using InLooxInvoiceWebservice.Models;
using System;

namespace InLooxInvoiceWebservice.Test.Data
{
    public static class TestData
    {
        public static InvoiceModel GetTestInvoice_Good(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 1000.25,
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_MissingBudgetPosition()
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 1000.25,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_MissingAmount(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_MissingInvoiceNumber(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                AmountEur = 1000.25,
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_MissingAttachmentLink(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 1000.25,
                BudgetPositionId = budgetPositionId
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_NonexistentBudgetPosition()
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 1000.25,
                BudgetPositionId = new Guid("abcde012-abcd-abcd-abcd-abcde0123456"),
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_NoPlannedExpenseBudgetPosition(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 1000.25,
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_NegativeAmount(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = -1000.25,
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }

        public static InvoiceModel GetTestInvoice_ZeroAmount(Guid budgetPositionId)
        {
            var invoice = new InvoiceModel()
            {
                InvoiceNumber = "12345/2020",
                AmountEur = 0,
                BudgetPositionId = budgetPositionId,
                AttachmentLink = new Uri("https://www.inloox.com")
            };

            return invoice;
        }
    }
}
