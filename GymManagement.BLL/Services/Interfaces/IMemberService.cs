using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);

        Task<Result>CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<Result<MemberViewModel>> MemberDetailsAsync(int memberid, CancellationToken ct = default);

        Task<Result<MemberToUpdateViewModel>> GetMemberToUpdatAsync(int memberid, CancellationToken ct = default);

        Task<Result>UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);

        Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);

        Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default);

    }
}
