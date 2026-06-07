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


        // GET: Plans
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct: ct);
            return View(plans);
        }

        // GET: Plans/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.PlanDetailsAsync(id, ct: ct);

            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));

            }
            else
                return View(plan);
        }

        //Get : Plans/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found, inactive, or has active membership).";
                return RedirectToAction(nameof(Index));

            }

            return View(plan);
        }

        //Post : Plans/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdatePlanViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =  await _planService.UpdatPlanDetailsAsync(id,model, ct);
            if (result)
                TempData["SuccesMessage"] = "Plan Updatd Successfully";
            else
                TempData["ErrorMessage"] = "Plan Failed To Update"; 


            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Activate(int id ,CancellationToken ct)
        {
            var result = await _planService.ToggleActivationAsync(id, ct);
            if (result)
                TempData["SuccesMessage"] = "Plan Status Changed";
            else
                TempData["ErrorMessage"] = "Failed To Toggle Plan Status";


            return RedirectToAction(nameof(Index));


        }
    }
}
