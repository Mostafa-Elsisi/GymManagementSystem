using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookindViewModel;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class BookingController :Controller
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index( CancellationToken ct)
            => View(await _bookingService.GetAllSessionAsync(ct));


        [HttpGet]
        public async Task<IActionResult>GetMembersForOngoingSession(int id ,CancellationToken ct)

        {
            var members = await _bookingService.GetMembersForSessionAsync(id, ct);
            return View(members);
        }
        public async Task<IActionResult>GetMembersForUpcominingSession(int id ,CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForSessionAsync(id,ct);
            return View(members);
        }


        #region Create

        [HttpGet]
        public async Task<IActionResult> Create(int id, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropDownAsync(id, ct);

            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = id;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            var result  =await _bookingService.CreateNewBookingAsync(model, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking Created Succesfully " : result.error;
            
            return RedirectToAction(nameof(GetMembersForUpcominingSession),new {id = model.SessionId});

        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendedAsync(memberId, sessionId, ct);
            if (!result.success)
            {
                TempData["ErrorMessage"] = result.error;
            }
            else
            {
                TempData["SuccessMessage"] = "Attendance Recorded";
            }
            return RedirectToAction(nameof(GetMembersForOngoingSession),new {id = sessionId});
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
            if (!result.success)
            {
                TempData["Error"] = result.error;
            }
            else
            {
                TempData["Success"] = "Cancel Booking Successfully";
            }
            return RedirectToAction(nameof(GetMembersForUpcominingSession),new {id = sessionId});
        }




        private async Task<IEnumerable<MemberSelectViewModel>> GetMembrsforDropdown(int sessionId,CancellationToken ct)
        => await _bookingService.GetMembersForDropDownAsync(sessionId, ct);
          
    }
}
