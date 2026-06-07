using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.BLL.ViewModels.TrainerViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetTrainersAsync(CancellationToken ct);

        Task<TrainerViewModel?> TrainerDetailsAsync(int trainerid, CancellationToken ct = default);

        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);

        Task<TrainerToUpdateViewModel?> GetTrainerToUpdatAsync(int memberid, CancellationToken ct = default);

        Task<bool>UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
      
        public Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default);

    }
}
