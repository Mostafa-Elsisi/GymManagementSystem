using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IUnitOfWork
    {

        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        Task<int> SaveChangesAsync(CancellationToken ct);
        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MemberShipRepository { get; }
    }
}
