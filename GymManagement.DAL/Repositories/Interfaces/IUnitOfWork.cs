using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        Task<int> SaveChangesAsync(CancellationToken ct);
        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MemberShipRepository { get; }
        public IBookingRepository BookingRepository { get; }
    }
}
