using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.BookindViewModel;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using GymManagement.BLL.ViewModels.SessionViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(CancellationToken ct = default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMembersForSessionAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct);
        Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct);
        Task<Result>MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct);
        Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct);
     
        Task<IEnumerable<MemberForSessionViewModel>> GetMemberForUpcomingSessionIdAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<MemberForSessionViewModel>> GetMemberForongoingSessionIdAsync(int sessionId, CancellationToken ct = default);
    }
}
