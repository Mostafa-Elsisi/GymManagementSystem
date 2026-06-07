using GymManagement.BLL.ViewModels.PlanViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default);

        Task<PlanViewModel?> PlanDetailsAsync(int planid, CancellationToken ct = default);

        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planid, CancellationToken ct = default);

        Task<bool> UpdatPlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);

        Task<bool> ToggleActivationAsync(int planid, CancellationToken ct = default);

    }
}
