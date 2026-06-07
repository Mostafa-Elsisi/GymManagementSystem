using GymManagement.BLL.ViewModels.MemberViewModel;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default);
    
        Task <bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> MemberDetailsAsync(int memberid , CancellationToken ct = default);

        Task<MemberToUpdateViewModel?> GetMemberToUpdatAsync(int memberid, CancellationToken ct = default);

        Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);
        
        Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default);
       
        Task<bool> RemoveMemberAsync(int id , CancellationToken ct = default);

    }
}
