using GymManagement.DAL.Data;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class MemberShipRepository : GenericRepository<Membership> ,IMemberShipRepository
    {
        private readonly GymDbContext _dbContext;

        public MemberShipRepository(GymDbContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
        async Task<IEnumerable<Membership>> IMemberShipRepository.GetMembershipsWithMemberAndPlansAsync(Expression<Func<Membership, bool>>? filter, CancellationToken ct)
        {
            var query = _dbContext.Memberships
                                        .Include(m => m.Member)
                                        .Include(m => m.Plan)
                                        .AsNoTracking();
            if(filter is not null)
                query =query.Where(filter);

            return await query.ToListAsync(ct);
        }
    }
}
