using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionAsync(ct:ct);
            return View(sessions);
        }

        #region Create

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropDownLisAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownLisAsync();
                return View(model);
            }
            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Created Successfuly";
                return RedirectToAction(nameof(Index));

            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownLisAsync();
            return View(model);
        }

        private async Task PopulateDropDownLisAsync()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GeCategoryForDropDownAsync(), "Id", "CategoryName");
        }

        #endregion
        #region SessionDetails

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion
        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]

        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(model);
            }

            var result = await _sessionService.UpdateSesionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDownAsync(), "Id", "Name");
                return View(model);
            }
        }

        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);
            if (result.success)
                TempData["SuccessMessage"] = "Session Deleted";
            else
                TempData["ErrorMessage"] = result.error;
            
            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
