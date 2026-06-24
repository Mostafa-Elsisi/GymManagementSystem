using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

      
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainers = await _trainerService.GetTrainersAsync(ct: ct);
            return View(trainers);
        }

        [HttpGet]
        public async Task<IActionResult> TrainerDetails(int id, CancellationToken ct)
        {
            var trainer = await _trainerService.TrainerDetailsAsync(id, ct);
            if (! trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
            return View(trainer.value);
        }

        #region Create 
        //Get
        [HttpGet]
        public IActionResult Create() => View();

        //Post
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);


            var result = await _trainerService.CreateTrainerAsync(model, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = result.error;


            return RedirectToAction(nameof(Index));

        }

        #endregion

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {

            var trainer = await _trainerService.GetTrainerToUpdatAsync(id, ct);
            if (!trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
            return View(trainer.value);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
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
            var trainer = _trainerService.TrainerDetailsAsync(id, ct).Result;
            if (!trainer.success)
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
            return View();

        }
       
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var member = await _trainerService.RemoveTrainerAsync(id, ct);
            if (member.success)
            TempData["SuccessMessage"] = "Trainer Deleted Successfully";
            else
             TempData["ErrorMessage"] = "Failed To Delete Trainer";
        

            return RedirectToAction(nameof(Index));
        }
        #endregion



    }
}
