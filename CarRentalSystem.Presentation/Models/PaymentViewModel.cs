using CarRentalSystem.DataAccess.Entities.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Presentation.Models
{
    public class PaymentViewModel
    {
        public int ReservationId { get; set; }

        [Display(Name = "Kwota do zapłaty")]
        public decimal Amount { get; set; }

        public DateTime? PaymentDate { get; set; }
        public PaymentStatus? Status { get; set; }
        [ValidateNever]
        [BindNever]
        public string PaymentConfirmationNumber { get; set; }
    }
}
