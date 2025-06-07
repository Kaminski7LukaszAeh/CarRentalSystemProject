using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Services;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
namespace CarRentalSystem.Tests.Services
{
    public class ReservationServiceTest
    {
        private readonly Mock<IReservationRepository> _mockReservationRepo;
        private readonly Mock<IVehicleRepository> _mockVehicleRepo;
        private readonly Mock<ILogger<ReservationService>> _mockLogger;
        private readonly ReservationService _service;

        public ReservationServiceTest()
        {
            _mockReservationRepo = new Mock<IReservationRepository>();
            _mockVehicleRepo = new Mock<IVehicleRepository>();
            _mockLogger = new Mock<ILogger<ReservationService>>();
            _service = new ReservationService(_mockReservationRepo.Object, _mockVehicleRepo.Object, _mockLogger.Object);
        }
  
    [Fact]
        public async Task GetReservationByIdAsync_ShouldReturnReservation_WhenFound()
        {
            var reservationId = 1;
            var reservation = new Reservation { Id = reservationId, UserId = "user123" };

            _mockReservationRepo.Setup(repo => repo.GetByIdAsync(reservationId))
                .ReturnsAsync(reservation);

            var result = await _service.GetReservationByIdAsync(reservationId);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(reservationId, result.Value.Id);
        }

        [Fact]
        public async Task GetReservationByIdAsync_ShouldReturnFailure_WhenReservationNotFound()
        {
            int reservationId = 42;
            _mockReservationRepo.Setup(repo => repo.GetByIdAsync(reservationId))
                                .ReturnsAsync((Reservation?)null);

            var result = await _service.GetReservationByIdAsync(reservationId);

            Assert.False(result.IsSuccess);
            Assert.Equal("Reservation not found.", result.Error);

            _mockReservationRepo.Verify(r => r.GetByIdAsync(reservationId), Times.Once);
        }

        [Fact]
        public async Task GetReservationByIdAsync_ShouldReturnFailure_WhenExceptionThrown()
        {
            var reservationId = 999;
            _mockReservationRepo.Setup(repo => repo.GetByIdAsync(reservationId))
                                .ThrowsAsync(new Exception("Database down"));

            var result = await _service.GetReservationByIdAsync(reservationId);

            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred while retrieving reservation.", result.Error);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to retrieve reservation")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldCreateReservation_WhenDataIsValid()
        {
            // Arrange
            var dto = new CreateReservationDto
            {
                VehicleId = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                UserId = "user123"
            };

            var vehicle = new Vehicle
            {
                Id = 1,
                DailyRate = 50
            };

            _mockVehicleRepo.Setup(v => v.GetVehicleByIdAsync(dto.VehicleId))
                .ReturnsAsync(vehicle);

            _mockReservationRepo.Setup(r => r.IsVehicleAvailable(dto.VehicleId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(true);

            _mockReservationRepo.Setup(r => r.AddReservationAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask);

            _mockReservationRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateReservationAsync(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ReservationStatus.Active, result.Value.Status);
            Assert.Equal(dto.UserId, result.Value.UserId);
            Assert.Equal(dto.VehicleId, result.Value.VehicleId);
            Assert.Equal(100, result.Value.TotalCost); // 2 x 50 = 100
        }

        [Theory]
        [InlineData("2025-06-10", "2025-06-09")] // EndDate before StartDate
        [InlineData("2025-06-10", "2025-06-10")] // Same day 
        public async Task CreateReservationAsync_ShouldFail_WhenEndDateIsBeforeOrSameAsStartDate(string start, string end)
        {
            var dto = new CreateReservationDto
            {
                StartDate = DateTime.Parse(start),
                EndDate = DateTime.Parse(end),
                VehicleId = 1
            };

            var result = await _service.CreateReservationAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("End date must be after start date.", result.Error);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldFail_WhenVehicleNotFound()
        {
            var dto = new CreateReservationDto
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                VehicleId = 999
            };

            _mockVehicleRepo.Setup(r => r.GetVehicleByIdAsync(dto.VehicleId)).ReturnsAsync((Vehicle)null);

            var result = await _service.CreateReservationAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Vehicle not found.", result.Error);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldFail_WhenVehicleNotAvailable()
        {
            var dto = new CreateReservationDto
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                VehicleId = 1
            };

            _mockVehicleRepo.Setup(r => r.GetVehicleByIdAsync(dto.VehicleId))
                .ReturnsAsync(new Vehicle { Id = dto.VehicleId, DailyRate = 100 });

            _mockReservationRepo.Setup(r =>
                r.IsVehicleAvailable(dto.VehicleId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(false);

            var result = await _service.CreateReservationAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Vehicle is not available for the selected dates.", result.Error);
        }

        [Fact]
        public async Task CreateReservationAsync_ShouldFail_WhenDateRangeIsAlreadyReserved()
        {
            var dto = new CreateReservationDto
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(5),
                VehicleId = 1
            };

            _mockVehicleRepo.Setup(r => r.GetVehicleByIdAsync(dto.VehicleId))
                .ReturnsAsync(new Vehicle { Id = dto.VehicleId, DailyRate = 100 });

            _mockReservationRepo.Setup(r =>
                r.IsVehicleAvailable(dto.VehicleId, dto.StartDate, dto.EndDate))
                .ReturnsAsync(false);

            var result = await _service.CreateReservationAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Equal("Vehicle is not available for the selected dates.", result.Error);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldCancel_WhenValid()
        {
            // Arrange
            int reservationId = 1;
            string userId = "user123";
            var reservation = new Reservation
            {
                Id = reservationId,
                UserId = userId,
                Status = ReservationStatus.Active
            };

            _mockReservationRepo.Setup(r => r.GetByIdAsync(reservationId))
                .ReturnsAsync(reservation);

            _mockReservationRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CancelReservationAsync(reservationId, userId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Reservation canceled.", result.Message);
            Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldFail_WhenReservationNotFound()
        {
            // Arrange
            _mockReservationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Reservation)null);

            // Act
            var result = await _service.CancelReservationAsync(1, "wrongUser");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Reservation not found.", result.Error);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldFail_WhenAlreadyCancelled()
        {
            // Arrange
            int reservationId = 1;
            string userId = "user123";
            var reservation = new Reservation
            {
                Id = reservationId,
                UserId = userId,
                Status = ReservationStatus.Cancelled
            };

            _mockReservationRepo.Setup(r => r.GetByIdAsync(reservationId))
                .ReturnsAsync(reservation);

            // Act
            var result = await _service.CancelReservationAsync(reservationId, userId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Reservation is already canceled.", result.Error);
        }

        [Fact]
        public async Task CancelReservationAsync_ShouldFail_WhenExceptionOccurs()
        {
            // Arrange
            _mockReservationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _service.CancelReservationAsync(1, "user123");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("An error occurred while canceling the reservation.", result.Error);
        }

        [Fact]
        public async Task UpdateReservationStatusAsync_ShouldUpdateStatus_WhenValid()
        {
            // Arrange
            var reservation = new Reservation
            {
                Id = 1,
                Status = ReservationStatus.Active,
                Payment = new Payment { Status = PaymentStatus.Pending }
            };

            _mockReservationRepo.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(reservation);

            _mockReservationRepo.Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateReservationStatusAsync(1, ReservationStatus.Completed);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Reservation status updated.", result.Message);
            Assert.Equal(ReservationStatus.Completed, reservation.Status);
            Assert.Equal(PaymentStatus.Completed, reservation.Payment.Status);
        }

        [Fact]
        public async Task UpdateReservationStatusAsync_ShouldFail_WhenReservationNotFound()
        {
            // Arrange
            _mockReservationRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Reservation)null);

            // Act
            var result = await _service.UpdateReservationStatusAsync(1, ReservationStatus.Completed);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Reservation not found.", result.Error);
        }

        [Fact]
        public async Task GetReservedDatesAsync_ShouldReturnCorrectDates()
        {
            // Arrange:
            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    StartDate = new DateTime(2025, 6, 10),
                    EndDate = new DateTime(2025, 6, 12),
                    Status = ReservationStatus.Active
                },
                new Reservation
                {
                    StartDate = new DateTime(2025, 6, 15),
                    EndDate = new DateTime(2025, 6, 15),
                    Status = ReservationStatus.Overdue
                },
                new Reservation
                {
                    StartDate = new DateTime(2025, 6, 20),
                    EndDate = new DateTime(2025, 6, 22),
                    Status = ReservationStatus.Cancelled
                }
            };

            _mockReservationRepo.Setup(r => r.GetReservationsByVehicleIdAsync(1))
                .ReturnsAsync(reservations);

            // Act
            var result = await _service.GetReservedDatesAsync(1);

            // Assert
            Assert.True(result.IsSuccess);

            var expectedDates = new[]
            {
                new DateTime(2025, 6, 10),
                new DateTime(2025, 6, 11),
                new DateTime(2025, 6, 12),
                new DateTime(2025, 6, 15)
            };

            Assert.Equal(expectedDates.OrderBy(d => d), result.Value.OrderBy(d => d));
        }

        [Fact]
        public async Task GetReservedDatesAsync_ShouldReturnFailure_WhenNoReservations()
        {
            // Arrange: 
            _mockReservationRepo.Setup(r => r.GetReservationsByVehicleIdAsync(1))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _service.GetReservedDatesAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task GetReservedDatesAsync_ShouldReturnEmpty_WhenAllReservationsInactive()
        {
            // Arrange
            var reservations = new List<Reservation>
    {
        new Reservation
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(1),
            Status = ReservationStatus.Completed
        },
        new Reservation
        {
            StartDate = DateTime.Today.AddDays(2),
            EndDate = DateTime.Today.AddDays(3),
            Status = ReservationStatus.Cancelled
        }
    };

            _mockReservationRepo
                .Setup(r => r.GetReservationsByVehicleIdAsync(It.IsAny<int>()))
                .ReturnsAsync(reservations);

            // Act
            var result = await _service.GetReservedDatesAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);  
        }

        [Fact]
        public async Task UpdateOverdueReservationsAsync_ShouldNotMarkAsOverdue_IfCurrentDateIsStartDateOrBefore()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;
            var reservation = new Reservation
            {
                StartDate = today,
                Status = ReservationStatus.Active,
                TotalCost = 100m,
                Payment = new Payment { Status = PaymentStatus.Pending },
                Vehicle = new Vehicle { DailyRate = 50m }
            };

            var reservations = new List<Reservation> { reservation };

            var mockRepo = new Mock<IReservationRepository>();
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask).Verifiable();

            var service = new ReservationService(mockRepo.Object, Mock.Of<IVehicleRepository>(), Mock.Of<ILogger<ReservationService>>());

            // Act
            await service.UpdateOverdueReservationsAsync(reservations);

            // Assert
            Assert.Equal(ReservationStatus.Active, reservation.Status);
            Assert.Equal(100m, reservation.TotalCost);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateOverdueReservationsAsync_ShouldMarkAsOverdue_AndIncreaseCost()
        {
            // Arrange
            var startDate = DateTime.UtcNow.Date.AddDays(-3); // 3 days overdue
            var initialCost = 300m;
            var dailyRate = 100m;
            var expectedAdditionalCost = 3 * dailyRate * 0.5m;
            var expectedTotalCost = initialCost + expectedAdditionalCost;

            var reservation = new Reservation
            {
                StartDate = startDate,
                Status = ReservationStatus.Active,
                TotalCost = initialCost,
                Payment = new Payment { Status = PaymentStatus.Pending },
                Vehicle = new Vehicle { DailyRate = dailyRate }
            };

            var reservations = new List<Reservation> { reservation };

            var mockRepo = new Mock<IReservationRepository>();
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Reservation>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var service = new ReservationService(mockRepo.Object, Mock.Of<IVehicleRepository>(), Mock.Of<ILogger<ReservationService>>());

            // Act
            await service.UpdateOverdueReservationsAsync(reservations);

            // Assert
            Assert.Equal(ReservationStatus.Overdue, reservation.Status);
            Assert.Equal(expectedTotalCost, reservation.TotalCost);
            mockRepo.Verify(r => r.UpdateAsync(It.Is<Reservation>(r =>
                r.Status == ReservationStatus.Overdue &&
                r.TotalCost == expectedTotalCost)), Times.Once);
        }

    }
}
