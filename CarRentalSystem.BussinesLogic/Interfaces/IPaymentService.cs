using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.DataTransferObjects;

namespace CarRentalSystem.BusinessLogic.Interfaces
{
    public interface IPaymentService
    {
        Task<Result> ProcessPaymentAsync(int reservationId, decimal amount);
        Task<Result<PaymentDto>> GetPaymentByReservationIdAsync(int reservationId);
    }
}
