using GymManagement.DAL.Data.Models;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetMembershipsWithMemberAndPlansAsync(Expression<Func<Membership,bool>>?filter =null,CancellationToken ct =default);
    }
}
