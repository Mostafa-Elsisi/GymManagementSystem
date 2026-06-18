using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {

        private readonly IPlanService _planService;

        public PlansController(IPlanService planSerrvice)
        {

            _planService = planSerrvice;
        }


       
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct: ct);
            return View(plans);
        }

        
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.PlanDetailsAsync(id, ct: ct);

            if (!plan.success)
            {
                TempData["ErrorMessage"] = plan.error;
                return RedirectToAction(nameof(Index));

            }
            else
                return View(plan.value);
        }

        #region Edit

        
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);
            if (!plan.success)
            {
                TempData["ErrorMessage"] = plan.error;
                return RedirectToAction(nameof(Index));
            }

            return View(plan.value);
        }

        //Post : Plans/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _planService.UpdatPlanDetailsAsync(id, model, ct);
            if (result.success)
                TempData["SuccesMessage"] = "Plan Updatd Successfully";
            else
                TempData["ErrorMessage"] = result.error;


            return RedirectToAction(nameof(Index));
        }

        #endregion

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id, ct);
            if (result.success)
                TempData["SuccesMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = result.error;


            return RedirectToAction(nameof(Index));


        }
    }
}
