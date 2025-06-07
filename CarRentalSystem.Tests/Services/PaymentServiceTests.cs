using CarRentalSystem.BusinessLogic.Services;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
using Moq;

namespace CarRentalSystem.Tests.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _mockPaymentRepo;
        private readonly Mock<IReservationRepository> _mockReservationRepo;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _mockPaymentRepo = new Mock<IPaymentRepository>();
            _mockReservationRepo = new Mock<IReservationRepository>();
            _service = new PaymentService(_mockPaymentRepo.Object, _mockReservationRepo.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnFailure_WhenReservationNotFound()
        {
            var mockPaymentRepo = new Mock<IPaymentRepository>();
            var mockReservationRepo = new Mock<IReservationRepository>();
            mockReservationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Reservation)null);

            var service = new PaymentService(mockPaymentRepo.Object, mockReservationRepo.Object);

            var result = await service.ProcessPaymentAsync(1, 100);

            Assert.False(result.IsSuccess);
            Assert.Equal("Reservation not found.", result.Error);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldUpdatePaymentAndMarkReservationPaid()
        {
            var payment = new Payment();
            var reservation = new Reservation { Payment = payment, Status = ReservationStatus.Active };

            var mockPaymentRepo = new Mock<IPaymentRepository>();
            var mockReservationRepo = new Mock<IReservationRepository>();
            mockReservationRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(reservation);
            mockReservationRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            var service = new PaymentService(mockPaymentRepo.Object, mockReservationRepo.Object);

            var result = await service.ProcessPaymentAsync(1, 200);

            Assert.True(result.IsSuccess);
            Assert.Equal(ReservationStatus.Completed, reservation.Status);
            Assert.Equal(200, reservation.Payment.Amount);
            Assert.Equal(PaymentStatus.Completed, reservation.Payment.Status);
            Assert.NotNull(reservation.Payment.PaymentConfirmationNumber);
            mockReservationRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPaymentByReservationIdAsync_ShouldReturnFailure_WhenPaymentNotFound()
        {
            var mockPaymentRepo = new Mock<IPaymentRepository>();
            mockPaymentRepo.Setup(p => p.GetByReservationIdAsync(1)).ReturnsAsync((Payment)null);

            var service = new PaymentService(mockPaymentRepo.Object, Mock.Of<IReservationRepository>());

            var result = await service.GetPaymentByReservationIdAsync(1);

            Assert.False(result.IsSuccess);
            Assert.Equal("Payment not found.", result.Error);
        }

        [Fact]
        public async Task GetPaymentByReservationIdAsync_ShouldReturnPaymentDto_WhenPaymentExists()
        {
            var payment = new Payment
            {
                Amount = 100,
                Status = PaymentStatus.Completed,
                PaymentDate = DateTime.UtcNow,
                PaymentConfirmationNumber = "CONF123"
            };

            var mockPaymentRepo = new Mock<IPaymentRepository>();
            mockPaymentRepo.Setup(p => p.GetByReservationIdAsync(1)).ReturnsAsync(payment);

            var service = new PaymentService(mockPaymentRepo.Object, Mock.Of<IReservationRepository>());

            var result = await service.GetPaymentByReservationIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(payment.Amount, result.Value.Amount);
        }
    }
}
