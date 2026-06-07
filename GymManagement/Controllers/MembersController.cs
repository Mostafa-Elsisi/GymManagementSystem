using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;

        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }



        // GET BaseUrl/Members/Index
        // Index - List all members

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        // GET BaseUrl/Members/Details/{id}
        // Details - Show details of a specific member
        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var Member = await _memberService.MemberDetailsAsync(id, ct);
            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }


        // GET BaseUrl/Members/HealthRecordDetails/{id}
        // HealthRecordDetails - Show health record details of a specific member
        [HttpGet]
        public async Task<IActionResult> MemberHealthRecordDetails(int id, CancellationToken ct)
        {
            var record = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (record is null)
            {
                TempData["ErrorMessage"] = "Data Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }


        #region Create
        // GET BaseUrl/Members/Create
        // Create - Show Empty Form to Create a new member

        [HttpGet]
        public IActionResult Create() => View();

        // Post BaseUrl/Members/Create
        // CreateMember - Submit the form to create a new member

        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";


            return RedirectToAction(nameof(Index));

        }


        #endregion

        #region Edit
        // GET BaseUrl/Members/Edit/{id}
        // Edit - Display Edit Form
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {

            var Member = await _memberService.GetMemberToUpdatAsync(id, ct);
            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }

        // Post BaseUrl/Members/Edit {Member}
        // EditMember - Submit Form to Edit the member
        [HttpPost]
        public async Task<IActionResult> Edit(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);

            if (result)
            {
                TempData["SuccesMessage"] = "Member Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Failed To Update Member";
            return View(model);
        }
        #endregion

        #region Delete
        // GET BaseUrl/Members/Delete/{id}
        // Delete - Show Form
        [HttpGet]
        public IActionResult Delete(int id, CancellationToken ct)
        {
            var member = _memberService.MemberDetailsAsync(id, ct).Result;
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View();

        }
        // Post BaseUrl/Members/DeleteConfirmed/{Id}
        // DeleteConfirmed - Submit the form to Delete member

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var member = await _memberService.RemoveMemberAsync(id, ct);
            if (member)
            {
                TempData["SuccesMessage"] = "Member Deleted Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Member";
            }

            return RedirectToAction(nameof(Index));

        }
        #endregion

    }
}
