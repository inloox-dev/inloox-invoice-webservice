using System;
using System.ComponentModel.DataAnnotations;

namespace InLooxInvoiceWebservice.Models
{
    public class InvoiceModel : IInvoiceModel
    {
        [Required]
        public string InvoiceNumber { get; set; }

        private double _amountEur;
        
        [Required]
        public double AmountEur
        {
            get { return _amountEur; }
            set {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(AmountEur), "Value must be greater than zero.");
                }

                _amountEur = value;
            }
        }

        private Guid _budgetPositionId;

        [Required]
        public Guid BudgetPositionId {
            get { return _budgetPositionId; }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new ArgumentOutOfRangeException(nameof(BudgetPositionId), "Value must not be empty.");
                }

                _budgetPositionId = value;
            }
        }

        [Required]
        public Uri AttachmentLink { get; set; }
    }
}
