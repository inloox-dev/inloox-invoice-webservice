using System;

namespace InLooxInvoiceWebservice.Models
{
    public interface IInvoiceModel : IEntityModel
    {
        double AmountEur { get; set; }
        Uri AttachmentLink { get; set; }
        Guid BudgetPositionId { get; set; }
        string InvoiceNumber { get; set; }
    }
}