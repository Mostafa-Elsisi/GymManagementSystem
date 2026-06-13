using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            if (!plans.Any())
                return [];

            return _mapper.Map<IEnumerable<PlanViewModel>>(plans);
           
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);

            if (plan is null || !plan.IsActive) 
                return Result<UpdatePlanViewModel>.NotFound("Plan Not Found Or Is Active");

            if (await HasActiveMembershipAsyn(planid, ct))
                return Result<UpdatePlanViewModel>.Validation("Plan Has Active MemberShip");
            else
            {
                var model = _mapper.Map<UpdatePlanViewModel>(plan);
                return Result<UpdatePlanViewModel>.OK(model);
            }

        }

        public async Task<Result<PlanViewModel>> PlanDetailsAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);
            if (plan is null)
                return Result<PlanViewModel>.NotFound("Plan Not Found");
            else
            {
                var model =  _mapper.Map<PlanViewModel>(plan);
                return Result<PlanViewModel>.OK(model);
            }
        }

        public async Task<Result> ToggleActivationAsync(int planid, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planid, ct);
            if (plan is null)
                return Result.NotFound("Plan Not Found"); ;
            if (plan.IsActive && await HasActiveMembershipAsyn(planid, ct))
                return Result.Validation("Plan Already Have Active MemberShip");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Toggle Activation ");
        }

        public async Task<Result> UpdatPlanDetailsAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null)
                return Result.NotFound("Plan Not Found");
            if (await HasActiveMembershipAsyn(id, ct))
                return Result.Validation("Plan Already Have Active MemberShip");

            _mapper.Map(model, plan);

            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Plan");

        }

        #region HelperMethod
        private async Task<bool> HasActiveMembershipAsyn(int planId, CancellationToken ct)
        {
            return await _unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
        }
        #endregion
    }
}
