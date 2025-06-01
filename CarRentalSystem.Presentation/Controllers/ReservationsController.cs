using CarRentalSystem.BusinessLogic.Interfaces;
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
        private readonly IVehicleService _vehicleService;

        public ReservationsController(IReservationService reservationService, IVehicleService VehicleService)
        {
            _reservationService = reservationService;
            _vehicleService = VehicleService;
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation(int vehicleId)
        {
            var vehicleDto = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            if (vehicleDto == null)
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
                DailyRate = vehicleDto.Value.DailyRate,
                TotalCost = _reservationService.CalculateTotalCost(startDate, endDate, vehicleDto.Value.DailyRate)
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservation(CreateReservationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(model.VehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }
                model.DailyRate = vehicle.Value.DailyRate;
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dto = ViewModelToDtoMapper.Map(model, userId);

            var result = await _reservationService.CreateReservationAsync(dto);

            if (!result.IsSuccess)
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(model.VehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }
                model.DailyRate = vehicle.Value.DailyRate;
                TempData["Error"] = result.Error ?? "Failed to create reservation.";
                return View(model);
            }

            var reservationId = result.Value.Id;

            if (model.PayNow)
            {
                return RedirectToAction("Payment", "Payments", new { reservationId });
            }
            else
            {
                TempData["Success"] = "Rezerwacja została utworzona. Możesz zapłacić później.";
                return RedirectToAction("MyReservations", "Reservations");
            }
        }


        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reservationService.GetActiveReservationsByUserAsync(userId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return View(new List<ReservationViewModel>());
            }

            var reservationViewModels = DtoToViewModelMapper.MapList(result.Value);
            return View(reservationViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> ReservationHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reservationService.GetHistoricalReservations(userId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
                return View(new List<ReservationViewModel>());
            }

            var viewModels = DtoToViewModelMapper.MapList(result.Value);
            return View(viewModels);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reservationService.CancelReservationAsync(id, userId);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.Error;
            }
            else
            {
                TempData["Success"] = "Rezerwacja została anulowana.";
            }

            return RedirectToAction("MyReservations");
        }


        //[HttpGet]
        //public async Task<IActionResult> GetReservedDates(int vehicleId)
        //{
        //    var result = await _reservationService.GetReservedDatesAsync(vehicleId);

        //    if (!result.IsSuccess)
        //    {
        //        return BadRequest(result.Error);
        //    }

        //    return Json(result.Value);
        //}


    }
}
