using CarRentalSystem.DataAccess.Entities.Enums;
namespace CarRentalSystem.BusinessLogic.DataTransferObjects
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentConfirmationNumber { get; set; }
    }
}
