using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberShipService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.MemberShipRepository.GetMembershipsWithMemberAndPlansAsync(m => m.EndDate >  DateTime.UtcNow,ct);
            return _mapper.Map<IEnumerable<MemberShipViewModel>>(memberships);
        }
      
        public async Task<Result> CreateMemberShipByIdAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
        {
            // 1 . Chek member Is Exist
            var memberExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, ct);
            if (! memberExists)
                return Result.NotFound("Member Is Not Found");

            // 2. Check if plan exists
            var planExists = await _unitOfWork.GetRepository<Plan>().AnyAsync(p => p.Id == model.PlanId, ct);
            if (!planExists)
                return Result.NotFound("Plan Is Not Found");

            // 3. Check if member has active membership
            var hasActiveMembership = await _unitOfWork.MemberShipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.UtcNow, ct);
            if(hasActiveMembership)
                return Result.Fail("Member already has an active membership", ResultKind.Conflict);

            // 4. Check if plan is active
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
            if(! plan.IsActive)
                return Result.NotFound("Plan Is Not Allow");

            
            var membership = _mapper.Map<Membership>(model);
            membership.EndDate = (model.StartDate ?? DateTime.UtcNow).AddDays(plan.DurationDays);


            _unitOfWork.MemberShipRepository.Add(membership);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create MemberShip");
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownList(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetPlansForDropDownList(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(plans);

        }
        public async Task<Result> DeleteActiveMemberShip(int memberId, CancellationToken ct = default)
        {
            var ActiveMembership = await _unitOfWork.MemberShipRepository.FirstOrDefaultAsync(m => m.MemberId == memberId && m.EndDate > DateTime.UtcNow, tracking: true);

            if (ActiveMembership is null)
                return Result.Fail("Active MemberShip Not Found For The Member", ResultKind.Conflict);

            _unitOfWork.MemberShipRepository.Delete(ActiveMembership);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Delete Active MemberShip");
        }
    }
}
