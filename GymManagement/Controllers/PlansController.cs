using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Controllers
{
    public class PlansController : Controller
    {
        private readonly GymDbContext _context;

        public PlansController()
        {
            _context = new GymDbContext();
           
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            return View(await _context.Plans.ToListAsync());
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var plan = await _context.Plans.FirstOrDefaultAsync(m => m.Id == id);
            if (plan is null)
                return RedirectToAction(nameof(Index));
            else
                return View(plan);
        }


    }
}
