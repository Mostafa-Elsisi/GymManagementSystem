using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class MembershipController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MembershipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }
        public async Task<IActionResult> Index()
        {
            var memberships = await _memberShipService.GetAllMemberShipsAsync();
            return View(memberships);
        }

        #region Create
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await PopulateDropdownAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMemberShipViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownAsync(ct);
                return View(model);
            }
            var result = await _memberShipService.CreateMemberShipByIdAsync(model, ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.error;
            await PopulateDropdownAsync(ct);
            return View(model);
        }

        private async Task PopulateDropdownAsync(CancellationToken ct)
        {
            ViewBag.Plans = new SelectList(await _memberShipService.GetPlansForDropDownList(), "Id", "Name");
            ViewBag.Members = new SelectList(await _memberShipService.GetMembersForDropDownList(), "Id", "Name");

        }

        #endregion

        #region Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberid,CancellationToken ct)
        {
            var result = await _memberShipService.DeleteActiveMemberShip(memberid, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"]
                = result.success ? "Membership Cancelled Successfully " : result.error;
          
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
