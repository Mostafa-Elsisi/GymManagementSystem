using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.TrainerViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetTrainersAsync(CancellationToken ct);

        Task<Result<TrainerViewModel>> TrainerDetailsAsync(int trainerid, CancellationToken ct = default);

        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);

        Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdatAsync(int memberid, CancellationToken ct = default);

        Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);

        public Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default);

    }
}
