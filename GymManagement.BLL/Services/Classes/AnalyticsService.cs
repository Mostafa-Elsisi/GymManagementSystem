using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetAnalyticsAsync(CancellationToken ct = default)
        {
           var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct:ct);
           var trainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct:ct);
           var members = await _unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
           var activeMembers = await _unitOfWork.GetRepository<Membership>().CountAsync(m => m.EndDate > DateTime.Now, ct:ct);

            return new AnalyticsViewModel
            {
                TotalMembers = members,
                TotalTrainers = trainers,
                ActiveMembers = activeMembers,
                UpcomingSessions = sessions.Count(s => s.StartDate > DateTime.Now),
                OngoingSessions = sessions.Count(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now),
                CompletedSessions = sessions.Count(s => s.EndDate < DateTime.Now)
            };
        }
    }
}
