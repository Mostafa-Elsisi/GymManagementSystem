using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string,object> _repositories = [];

        public UnitOfWork(GymDbContext dbContext,
            ISessionRepository sessionRepository,
            IMemberShipRepository memberShipRepository,
            IBookingRepository bookingRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
            MemberShipRepository = memberShipRepository;
            BookingRepository = bookingRepository;
        }

        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MemberShipRepository { get; }
        public IBookingRepository BookingRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //chek TEntity == ??? //Plan , Trainer
            var typeName = typeof(TEntity).Name;

            //if Exist Return
            if (_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity>) value;
            else
            {
                //Else Create - Store - Return
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return repo;

            }
            

        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        => _dbContext.SaveChangesAsync(ct);
    }
}
