using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);

        Task<Result<PlanViewModel>> PlanDetailsAsync(int planid, CancellationToken ct = default);

        Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planid, CancellationToken ct = default);

        Task<Result> UpdatPlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);

        Task<Result> ToggleActivationAsync(int planid, CancellationToken ct = default);

    }
}
