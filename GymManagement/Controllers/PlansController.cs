using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
        
        private readonly IPlanRepository planRepository;

        public PlansController(IPlanRepository _planRepository)
        {
            planRepository = _planRepository;
        }
        
        
        // GET: Plans
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await planRepository.GetAllAsync(ct: ct);
            return View(plans);
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await planRepository.GetByIdAsync(id, ct: ct);

            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }


    }
}
