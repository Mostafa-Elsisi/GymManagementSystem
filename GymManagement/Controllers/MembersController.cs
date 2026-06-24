using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize ]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }


        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memberService.GetAllMembersAsync(ct);
            return View(members);
        }


        [HttpGet]
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var Member = await _memberService.MemberDetailsAsync(id, ct);
            if (!Member.success)
            {
                TempData["ErrorMessage"] = Member.error;
                return RedirectToAction(nameof(Index));
            }
            return View(Member.value);
        }



        [HttpGet]
        public async Task<IActionResult> MemberHealthRecordDetails(int id, CancellationToken ct)
        {
            var record = await _memberService.GetMemberHealthRecordAsync(id, ct);
            if (!record.success)
            {
                TempData["ErrorMessage"] = record.error;
                return RedirectToAction(nameof(Index));
            }
            return View(record.value);
        }

        #region Get Member Photo
        [HttpGet]
        public async Task<IActionResult> GetMemberPhoto(int id, CancellationToken ct)
        {
            var member = await _memberService.MemberDetailsAsync(id, ct);
            if (member is null || !member.success || string.IsNullOrWhiteSpace(member.value?.Photo))
                return NotFound();

            var result = _attachmentService.GetFile(member.value.Photo, "MembersPhoto");
            if(result is null) return NotFound();

            return File(result.Value.stream, result.Value.contentType);
        }
        #endregion


        #region Create


        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = result.error;


            return RedirectToAction(nameof(Index));

        }


        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {

            var member = await _memberService.GetMemberToUpdatAsync(id, ct);
            if (!member.success)
            {
                TempData["ErrorMessage"] = member.error;
                return RedirectToAction(nameof(Index));
            }
            return View(member.value);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Member Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            return View(model);
        }
        #endregion

        #region Delete

        [HttpGet]
        public IActionResult Delete(int id, CancellationToken ct)
        {
            var member = _memberService.MemberDetailsAsync(id, ct).Result;
            if (!member.success)
            {
                TempData["ErrorMessage"] = member.error;
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var member = await _memberService.RemoveMemberAsync(id, ct);


            TempData[member.success ? "SuccessMessage" : "ErrorMessage"] =
                                      member.success ? "Member Deleted Successfully" : member.error;
            //if (member.success)
            //{
            //    TempData["SuccessMessage"] = "Member Deleted Successfully";
            //}
            //else
            //{
            //    TempData["ErrorMessage"] = "Failed To Delete Member";
            //}

            return RedirectToAction(nameof(Index));

        }
        #endregion

    }
}
