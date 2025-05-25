using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.BusinessLogic.Mapping;
using CarRentalSystem.DataAccess.Interfaces;
using CarRentalSystem.Presentation.Mapping;
using CarRentalSystem.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRentalSystem.Presentation.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IVehicleRepository _vehicleRepository;

        public ReservationsController(IReservationService reservationService, IVehicleRepository vehicleRepository)
        {
            _reservationService = reservationService;
            _vehicleRepository = vehicleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation(int vehicleId)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(1);

            var model = new CreateReservationViewModel
            {
                VehicleId = vehicleId,
                StartDate = startDate,
                EndDate = endDate,
                DailyRate = vehicle.DailyRate,
                TotalCost = _reservationService.CalculateTotalCost(startDate, endDate, vehicle.DailyRate)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(model.VehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }
                model.DailyRate = vehicle.DailyRate;

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var dto = ViewModelToDtoMapper.Map(model, userId);
            var result = await _reservationService.CreateReservationAsync(dto);

            if (result.IsSuccess)
            {
                TempData["Success"] = "Reservation created successfully.";
                return RedirectToAction("MyReservations", "Reservations");
            }
            else
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(model.VehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }
                model.DailyRate = vehicle.DailyRate;

                TempData["Error"] = result.Error ?? "Failed to create reservation.";
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reservationService.GetReservationsByUserAsync(userId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return View(new List<ReservationViewModel>());
            }

            var reservationViewModels = DtoToViewModelMapper.MapList(result.Value);
            return View(reservationViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetReservedDates(int vehicleId)
        {
            var result = await _reservationService.GetReservationsByVehicleAsync(vehicleId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            var reservedDates = result.Value
                .SelectMany(r => Enumerable.Range(0, (r.EndDate.Date - r.StartDate.Date).Days + 1)
                .Select(d => r.StartDate.Date.AddDays(d).ToString("yyyy-MM-dd")))
                .Distinct()
                .ToList();

            return Json(reservedDates);
        }


    }
}
