using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Services;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.Presentation.Mapping;
using CarRentalSystem.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IVehicleService _vehicleService;
    private readonly IReservationService _reservationService;

    public AdminController(IVehicleService vehicleService, IReservationService reservationService)
    {
        _vehicleService = vehicleService;
        _reservationService = reservationService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ManageVehicleTypes()
    {
        var result = await _vehicleService.GetAllVehicleTypes();

        var model = new ManageVehicleTypesViewModel
        {
            VehicleTypes = result.IsSuccess ? DtoToViewModelMapper.MapList(result.Value) : new List<VehicleTypeViewModel>()
        };

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVehicleType(CreateVehicleTypeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Type name is required.";
            return RedirectToAction(nameof(ManageVehicleTypes));
        }

        var dto = ViewModelToDtoMapper.Map(model);
        var result = await _vehicleService.AddVehicleTypeAsync(dto);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicleTypes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVehicleType(int id)
    {
        var result = await _vehicleService.DeleteVehicleTypeAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicleTypes));
    }

    [HttpGet]
    public async Task<IActionResult> ManageVehicleBrands()
    {
        var result = await _vehicleService.GetAllBrandsAsync();

        var model = new ManageVehicleBrandsViewModel
        {
            VehicleBrands = result.IsSuccess ? DtoToViewModelMapper.MapList(result.Value) : new List<VehicleBrandViewModel>()
        };

        if (!result.IsSuccess)
            TempData["Error"] = result.Error;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddVehicleBrand(CreateVehicleBrandViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Brand name is required.";
            return RedirectToAction(nameof(ManageVehicleBrands));
        }

        var dto = ViewModelToDtoMapper.Map(model);
        var result = await _vehicleService.AddVehicleBrandAsync(dto);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicleBrands));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVehicleBrand(int id)
    {
        var result = await _vehicleService.DeleteVehicleBrandAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicleBrands));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVehicleModel(CreateVehicleModelViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill all required fields correctly.";

            var brandsResult = await _vehicleService.GetAllBrandsAsync();
            if (brandsResult.IsSuccess)
            {
                model.AvailableBrands = brandsResult.Value
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToList();
            }

            var modelsResult = await _vehicleService.GetAllVehicleModelsAsync();
            var manageModel = new ManageVehicleModelsViewModel
            {
                NewVehicleModel = model,
                VehicleModels = modelsResult.IsSuccess ? DtoToViewModelMapper.MapList(modelsResult.Value) : new List<VehicleModelViewModel>()
            };

            return View("ManageVehicleModels", manageModel);
        }

        var dto = ViewModelToDtoMapper.Map(model);
        var result = await _vehicleService.AddVehicleModelAsync(dto);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;

        return RedirectToAction(nameof(ManageVehicleModels));
    }

    [HttpGet]
    public async Task<IActionResult> ManageVehicleModels()
    {
        var modelsResult = await _vehicleService.GetAllVehicleModelsAsync();
        var brandsResult = await _vehicleService.GetAllBrandsAsync();
        var typesResult = await _vehicleService.GetAllVehicleTypes();

        var model = new ManageVehicleModelsViewModel();

        if (modelsResult.IsSuccess)
        {
            model.VehicleModels = DtoToViewModelMapper.MapList(modelsResult.Value);
        }
        else
        {
            TempData["Error"] = modelsResult.Error;
        }

        if (brandsResult.IsSuccess)
        {
            model.NewVehicleModel = new CreateVehicleModelViewModel
            {
                AvailableBrands = brandsResult.Value
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToList()
            };
        }
        else
        {
            TempData["Error"] = brandsResult.Error;
        }

        if (typesResult.IsSuccess)
        {
            model.NewVehicleModel.AvailableVehicleTypes = DtoToViewModelMapper.MapTypesToSelectList(typesResult.Value);
        }

        return View(model);
    }

    public async Task<IActionResult> ManageReservations()
    {
        var result = await _reservationService.GetAllReservationsAsync();
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }

        var reservationViewModels = DtoToViewModelMapper.MapList(result.Value);

        return View(reservationViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReservationStatus(int reservationId, ReservationStatus status)
    {
        var result = await _reservationService.UpdateReservationStatusAsync(reservationId, status);

        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] = "Status updated successfully.";
        }

        return RedirectToAction("ManageReservations");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVehicleModel(int id)
    {
        var result = await _vehicleService.DeleteVehicleModelAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicleModels));
    }

    [HttpGet]
    public async Task<IActionResult> ManageVehicles()
    {
        var vehiclesResult = await _vehicleService.GetAllVehiclesAsync();
        var modelsResult = await _vehicleService.GetAllVehicleModelsAsync();
        var typesResult = await _vehicleService.GetAllVehicleTypes();

        var model = new ManageVehiclesViewModel
        {
            NewVehicle = new CreateVehicleViewModel()
        };

        model.Vehicles = DtoToViewModelMapper.MapList(vehiclesResult.Value);
        model.NewVehicle.AvailableModels = DtoToViewModelMapper.MapModelsToSelectList(modelsResult.Value);


        model.NewVehicle.AvailableTypes = typesResult.Value
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.TypeName
            }).ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVehicle(CreateVehicleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill all required fields correctly.";
            return RedirectToAction(nameof(ManageVehicles));
        }

        var dto = ViewModelToDtoMapper.Map(model);
        var result = await _vehicleService.AddVehicleAsync(dto);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicles));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVehicle(int id)
    {
        var result = await _vehicleService.DeleteVehicleAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] = result.Message ?? result.Error;
        return RedirectToAction(nameof(ManageVehicles));
    }

    [HttpGet]
    public async Task<IActionResult> GetBrandsByType(int typeId)
    {
        var result = await _vehicleService.GetBrandsByTypeAsync(typeId);
        if (!result.IsSuccess)
        {
            return Json(new List<SelectListItem>());
        }

        var brands = result.Value.Select(b => new { id = b.Id, name = b.Name });
        return Json(brands);
    }

    [HttpGet]
    public async Task<IActionResult> GetModelsByBrandAndType(int brandId, int typeId)
    {
        var result = await _vehicleService.GetModelsByBrandAndTypeAsync(brandId, typeId);
        if (!result.IsSuccess)
        {
            return Json(new List<SelectListItem>());
        }

        var models = result.Value.Select(m => new { id = m.Id, name = m.Name });
        return Json(models);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsReturned(int reservationId)
    {
        var result = await _reservationService.MarkAsReturnedAsync(reservationId);

        if (result.IsSuccess)
        {
            TempData["Success"] = result.Message ?? "Vehicle marked as returned.";
        }
        else
        {
            TempData["Error"] = result.Error ?? "Failed to mark vehicle as returned.";
        }

        return RedirectToAction(nameof(ManageReservations));
    }
}
