using CarRentalSystem.BusinessLogic.DataTransferObjects;
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
    public async Task<IActionResult> Index([FromQuery] VehicleFilter filter)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Invalid filter parameters.");
            filter = new VehicleFilter();
        }
        var vehicleTypes = await _vehicleService.GetAllVehicleTypes();
        ViewData["VehicleTypes"] = vehicleTypes.Value?.ToList() ?? new List<VehicleTypeDto>();

        ViewData["Filter"] = filter ?? new VehicleFilter();

        var filterDto = FilterMapper.Map(filter);
        var vehiclesResult = await _vehicleService.GetFilteredVehiclesAsync(filterDto);

        if (!vehiclesResult.IsSuccess)
        {
            TempData["Error"] = vehiclesResult.Error;
            return View(new List<VehicleViewModel>());
        }

        var model = DtoToViewModelMapper.MapList(vehiclesResult.Value);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var vehicleResult = await _vehicleService.GetVehicleByIdAsync(id);
        if (!vehicleResult.IsSuccess)
        {
            return NotFound();
        }

        var model = DtoToViewModelMapper.Map(vehicleResult.Value);
        return View(model);
    }

}
