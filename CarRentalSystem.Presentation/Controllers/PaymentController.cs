using CarRentalSystem.BusinessLogic.Interfaces;
using CarRentalSystem.DataAccess.Entities.Enums;
using CarRentalSystem.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Presentation.Controllers
{
    [Authorize]
    public class PaymentsController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IReservationService _reservationService;

        public PaymentsController(IPaymentService paymentService, IReservationService reservationService)
        {
            _paymentService = paymentService;
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int reservationId)
        {
            var reservationResult = await _reservationService.GetReservationByIdAsync(reservationId);
            if (!reservationResult.IsSuccess)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("MyReservations", "Reservations");
            }

            var model = new PaymentViewModel
            {
                ReservationId = reservationId,
                Amount = reservationResult.Value.TotalCost,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var paymentResult = await _paymentService.ProcessPaymentAsync(model.ReservationId, model.Amount);

            if (!paymentResult.IsSuccess)
            {
                ModelState.AddModelError("", paymentResult.Error);
                return View(model);
            }

            TempData["Success"] = "Płatność zakończona sukcesem.";
            return RedirectToAction("MyReservations", "Reservations");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int reservationId)
        {
            var paymentResult = await _paymentService.GetPaymentByReservationIdAsync(reservationId);

            if (!paymentResult.IsSuccess)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction("MyReservations", "Reservations");
            }
            var paymentViewModel = DtoToViewModelMapper.Map(paymentResult.Value);

            return View(paymentViewModel);
        }


    }

}
