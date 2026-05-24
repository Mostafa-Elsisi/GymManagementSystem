using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        Task <IEnumerable<Plan>> GetAllAsync(bool tracking =false , CancellationToken ct = default); 
        Task<Plan?>GetByIdAsync(int id ,CancellationToken ct = default);
        Task<int> AddAsync(Plan plan , CancellationToken ct = default);
        Task<int> UpdateAsync(Plan plan , CancellationToken ct = default);
        Task<int> DeleteAsync(Plan plan , CancellationToken ct = default);
             
    }
}
