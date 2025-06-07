using CarRentalSystem.BusinessLogic.Common;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.BusinessLogic.Mapping;

namespace CarRentalSystem.BusinessLogic.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReservationRepository _reservationRepository;

        public PaymentService(IPaymentRepository paymentRepository, IReservationRepository reservationRepository)
        {
            _paymentRepository = paymentRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<Result> ProcessPaymentAsync(int reservationId, decimal amount)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                return Result.Failure("Reservation not found.");

            var payment = reservation.Payment;
            if (payment == null)
                return Result.Failure("Payment not found for the reservation.");

            UpdatePayment(payment, amount);
            await _reservationRepository.SaveChangesAsync();

            return Result.Success("Payment processed successfully.");
        }

        public async Task<Result<PaymentDto>> GetPaymentByReservationIdAsync(int reservationId)
        {
            var payment = await _paymentRepository.GetByReservationIdAsync(reservationId);
            if (payment == null)
                return Result<PaymentDto>.Failure("Payment not found.");

            var dto = EntityToDtoMapper.Map(payment);

            return Result<PaymentDto>.Success(dto);
        }

        private void UpdatePayment(Payment payment, decimal amount)
        {
            payment.Amount = amount;
            payment.PaymentDate = DateTime.UtcNow.AddHours(2);
            payment.Status = PaymentStatus.Completed;
            payment.PaymentConfirmationNumber = Guid.NewGuid().ToString();
        }
    }
}
