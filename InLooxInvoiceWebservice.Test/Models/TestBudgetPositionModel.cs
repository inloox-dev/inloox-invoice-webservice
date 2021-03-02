using System;

namespace InLooxInvoiceWebservice.Test.Models
{
    public class TestBudgetPositionModel
    {
        public string ShortDescription { get; set; }

        public Guid ProjectId { get; set; }

        public Guid BudgetId { get; set; }
        
        public int Quantity { get; set; }
    }
}
