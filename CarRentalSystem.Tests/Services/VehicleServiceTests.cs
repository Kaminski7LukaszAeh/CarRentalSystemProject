using CarRentalSystem.BusinessLogic.DataTransferObjects;
using CarRentalSystem.BusinessLogic.Services;
using CarRentalSystem.DataAccess.Entities;
using CarRentalSystem.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using CarRentalSystem.DataAccess.Filters;

public class VehicleServiceTests
{
    private readonly Mock<IVehicleRepository> _mockRepo;
    private readonly Mock<ILogger<VehicleService>> _mockLogger;
    private readonly VehicleService _service;

    public VehicleServiceTests()
    {
        _mockRepo = new Mock<IVehicleRepository>();
        _mockLogger = new Mock<ILogger<VehicleService>>();
        _service = new VehicleService(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AddVehicleAsync_ShouldReturnSuccess_WhenRepositoryAddsVehicle()
    {
        var dto = new CreateVehicleDto { DailyRate = 100, VehicleModelId = 1, VehicleTypeId = 1 };
        _mockRepo.Setup(r => r.AddVehicleAsync(It.IsAny<Vehicle>())).Returns(Task.CompletedTask);

        var result = await _service.AddVehicleAsync(dto);

        Assert.True(result.IsSuccess);
        Assert.Equal("Vehicle added successfully.", result.Message);
    }

    [Fact]
    public async Task AddVehicleAsync_ShouldReturnFailureAndLogError_WhenRepositoryThrowsException()
    {
        var dto = new CreateVehicleDto { DailyRate = 100, VehicleModelId = 1, VehicleTypeId = 1 };
        _mockRepo.Setup(r => r.AddVehicleAsync(It.IsAny<Vehicle>())).ThrowsAsync(new Exception("DB error"));

        var result = await _service.AddVehicleAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("An unexpected error occurred.", result.Error);
        _mockLogger.Verify(x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to add vehicle.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllVehiclesAsync_ShouldReturnVehicles_WhenRepositoryReturnsData()
    {
        var vehicles = new List<Vehicle> { new Vehicle(), new Vehicle() };
        _mockRepo.Setup(r => r.GetAllVehiclesAsync()).ReturnsAsync(vehicles);

        var result = await _service.GetAllVehiclesAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllVehiclesAsync_ShouldReturnFailure_WhenRepositoryThrows()
    {
        _mockRepo.Setup(r => r.GetAllVehiclesAsync()).ThrowsAsync(new Exception());

        var result = await _service.GetAllVehiclesAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to retrieve vehicles.", result.Error);
    }

    [Fact]
    public async Task GetFilteredVehiclesAsync_ShouldReturnFilteredVehicles()
    {
        var filter = new VehicleFilterDto();
        var vehicles = new List<Vehicle> { new Vehicle() };
        _mockRepo.Setup(r => r.GetFilteredVehiclesAsync(It.IsAny<VehicleFilterDal>())).ReturnsAsync(vehicles);

        var result = await _service.GetFilteredVehiclesAsync(filter);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetVehicleByIdAsync_ShouldReturnVehicle_WhenFound()
    {
        var vehicle = new Vehicle { Id = 1 };
        _mockRepo.Setup(r => r.GetVehicleByIdAsync(1)).ReturnsAsync(vehicle);

        var result = await _service.GetVehicleByIdAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Id);
    }

    [Fact]
    public async Task GetVehicleByIdAsync_ShouldReturnFailure_WhenNotFound()
    {
        _mockRepo.Setup(r => r.GetVehicleByIdAsync(1)).ReturnsAsync((Vehicle)null);

        var result = await _service.GetVehicleByIdAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public async Task UpdateVehicleAsync_ShouldUpdate_WhenVehicleExists()
    {
        var dto = new VehicleDto { Id = 1, DailyRate = 150, IsAvailable = true };
        var vehicle = new Vehicle { Id = 1 };
        _mockRepo.Setup(r => r.GetVehicleByIdAsync(1)).ReturnsAsync(vehicle);

        var result = await _service.UpdateVehicleAsync(dto);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeleteVehicleAsync_ShouldReturnFailure_WhenVehicleNotFound()
    {
        _mockRepo.Setup(r => r.GetVehicleByIdAsync(1)).ReturnsAsync((Vehicle)null);

        var result = await _service.DeleteVehicleAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle not found.", result.Error);
    }

    [Fact]
    public async Task AddVehicleTypeAsync_ShouldReturnFailure_WhenTypeExists()
    {
        var dto = new CreateVehicleTypeDto { TypeName = "SUV" };
        _mockRepo.Setup(r => r.TypeExistsAsync("SUV")).ReturnsAsync(true);

        var result = await _service.AddVehicleTypeAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle type already exists.", result.Error);
    }

    [Fact]
    public async Task DeleteVehicleTypeAsync_ShouldReturnFailure_WhenTypeHasModels()
    {
        var type = new VehicleType { VehicleModels = new List<VehicleModel> { new VehicleModel() } };
        _mockRepo.Setup(r => r.GetTypeByIdAsync(1)).ReturnsAsync(type);

        var result = await _service.DeleteVehicleTypeAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete vehicle type because it has associated vehicle models.", result.Error);
    }

    [Fact]
    public async Task AddVehicleBrandAsync_ShouldReturnFailure_WhenBrandExists()
    {
        var dto = new CreateVehicleBrandDto { Name = "Toyota" };
        _mockRepo.Setup(r => r.BrandExistsAsync("Toyota")).ReturnsAsync(true);

        var result = await _service.AddVehicleBrandAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle brand already exists.", result.Error);
    }

    [Fact]
    public async Task DeleteVehicleBrandAsync_ShouldReturnFailure_WhenBrandHasModels()
    {
        var brand = new VehicleBrand { Models = new List<VehicleModel> { new VehicleModel() } };
        _mockRepo.Setup(r => r.GetBrandByIdAsync(1)).ReturnsAsync(brand);

        var result = await _service.DeleteVehicleBrandAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete vehicle brand because it has associated vehicle models.", result.Error);
    }

    [Fact]
    public async Task AddVehicleModelAsync_ShouldReturnFailure_WhenModelExists()
    {
        var dto = new CreateVehicleModelDto { Name = "Corolla", BrandId = 1 };
        _mockRepo.Setup(r => r.GetBrandByIdAsync(1)).ReturnsAsync(new VehicleBrand());
        _mockRepo.Setup(r => r.VehicleModelExistsAsync("Corolla", 1)).ReturnsAsync(true);

        var result = await _service.AddVehicleModelAsync(dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Vehicle model already exists.", result.Error);
    }

    [Fact]
    public async Task DeleteVehicleModelAsync_ShouldReturnFailure_WhenModelHasVehicles()
    {
        var model = new VehicleModel { Vehicles = new List<Vehicle> { new Vehicle() } };
        _mockRepo.Setup(r => r.GetVehicleModelByIdAsync(1)).ReturnsAsync(model);

        var result = await _service.DeleteVehicleModelAsync(1);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cannot delete vehicle model because it has associated vehicles.", result.Error);
    }

    [Fact]
    public async Task GetBrandsByTypeAsync_ShouldReturnBrands()
    {
        _mockRepo.Setup(r => r.GetBrandsByTypeAsync(1)).ReturnsAsync(new List<VehicleBrand> { new VehicleBrand() });

        var result = await _service.GetBrandsByTypeAsync(1);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetModelsByBrandAndTypeAsync_ShouldReturnModels()
    {
        _mockRepo.Setup(r => r.GetModelsByBrandAndTypeAsync(1, 2)).ReturnsAsync(new List<VehicleModel> { new VehicleModel() });

        var result = await _service.GetModelsByBrandAndTypeAsync(1, 2);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }
}
