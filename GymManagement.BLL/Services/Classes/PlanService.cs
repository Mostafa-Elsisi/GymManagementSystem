using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System.Numerics;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<Membership> _memberShipRepo;

        public PlanService(IGenericRepository<Plan> planRepo,
                           IGenericRepository<Membership> memberShipRepo)
        {
            _planRepo = planRepo;
            _memberShipRepo = memberShipRepo;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _planRepo.GetAllAsync(ct: ct);
            if (!plans.Any())
                return [];
            var planViewModels = plans.Select(
                p => new PlanViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    DurationDays = p.DurationDays,
                    Description = p.Description,
                    IsActive = p.IsActive,
                }
                );
            return planViewModels;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(planid, ct);
            if (plan is null || !plan.IsActive) return null;
            if (await HasActiveMembershipAsyn(planid, ct))
                return null; 
            else
            {
                return new UpdatePlanViewModel
                {
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    Price = plan.Price

                };
            }
        }

        public async Task<PlanViewModel?> PlanDetailsAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(planid,ct);
            if (plan is null) return null;
            else
            {
                return new PlanViewModel
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    IsActive = plan.IsActive,
                    Price = plan.Price,
                    DurationDays = plan.DurationDays,
                };
            }
        }

        public async Task<bool> ToggleActivationAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(planid, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActiveMembershipAsyn(planid, ct)) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            var result = await _planRepo.UpdateAsync(plan, ct);
            return result > 0;
        }

        public async Task<bool> UpdatPlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _planRepo.GetByIdAsync(id, ct);
            if (plan is null) return false;
            if (await HasActiveMembershipAsyn(id, ct))
                return false;

            plan.Description = model.Description;
            plan.DurationDays = model.DurationDays;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;


            var result = await _planRepo.UpdateAsync(plan,ct);
            return result > 0;

        }

        #region HelperMethod
        private async Task<bool> HasActiveMembershipAsyn(int planId,CancellationToken ct)
        {
            return await _memberShipRepo.AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
            
        }
        #endregion
    }
}
