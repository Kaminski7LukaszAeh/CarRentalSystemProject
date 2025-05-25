using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.Presentation.Models;
using Microsoft.AspNetCore.Mvc;

public class VehiclesController : Controller
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }
    public async Task<IActionResult> Index()
    {
        var vehiclesResult = await _vehicleService.GetAllVehiclesAsync();

        var model = DtoToViewModelMapper.MapList(vehiclesResult.Value);
        if (!vehiclesResult.IsSuccess)
        {
            TempData["Error"] = vehiclesResult.Error;
            return View(new List<VehicleViewModel>());
        }
        return View(model);
    }

}
