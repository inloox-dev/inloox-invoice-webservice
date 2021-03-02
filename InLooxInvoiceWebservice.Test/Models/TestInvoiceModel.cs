using InLooxInvoiceWebservice.Models;
using System;

namespace InLooxInvoiceWebservice.Test.Models
{
    public class TestInvoiceModel : IInvoiceModel
    {
        public string InvoiceNumber { get; set; }
        public double AmountEur { get; set; }
        public Guid BudgetPositionId { get; set; }
        public Uri AttachmentLink { get; set; }
    }
}
