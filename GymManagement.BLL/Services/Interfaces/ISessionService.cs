using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<Result<SessionViewModel>> GetSessionByIdAsync(int id, CancellationToken ct = default);

        Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(CancellationToken ct = default);

        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct);
         
        Task<IEnumerable<TrainerSelectViewModel>>  GetTrainerForDropDownAsync(CancellationToken ct = default);

        Task<IEnumerable<CategorySelectViewModel>>GeCategoryForDropDownAsync(CancellationToken ct = default);

        Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default);

        Task<Result> UpdateSesionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);

        Task<Result> RemoveSessionAsync(int id, CancellationToken ct);
    }
}
