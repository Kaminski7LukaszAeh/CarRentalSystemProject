using Microsoft.AspNetCore.Mvc;
using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.Presentation.Models;

public class ReservationCalendarViewComponent : ViewComponent
{
    private readonly IReservationService _reservationService;

    public ReservationCalendarViewComponent(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int vehicleId, decimal dailyRate)
    {
        var reservedDates = await _reservationService.GetReservedDatesAsync(vehicleId);

        var vm = new ReservationCalendarViewModel
        {
            VehicleId = vehicleId,
            DailyRate = dailyRate,
            ReservedDates = reservedDates.Value.ToList()
        };

        return View("Default", vm);
    }
}
