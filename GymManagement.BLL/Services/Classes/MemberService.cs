using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
            if (await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct))
                return Result.Validation("Email Already Exist !!");


            //Check Phone
            if (await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct))
                return Result.Validation("Phone Already Exist !!");

            // else Return True Add Member
            var member = _mapper.Map<Member>(model);


            _unitOfWork.GetRepository<Member>().Add(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Member");

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!members.Any())
                return [];

           return _mapper.Map<IEnumerable<Member>, IEnumerable<MemberViewModel>>(members);
        }

        public async Task<Result<HealthRecordViewModel>> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            // id is a Member id; fetch the health record using MemberId
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(m => m.MemberId == id, ct: ct);
            if (record == null)
                return Result<HealthRecordViewModel>.NotFound("Health Record Not Found");
            else
            {
                var model = _mapper.Map<HealthRecord, HealthRecordViewModel>(record);
                return Result<HealthRecordViewModel>.OK(model);
            }
        }

        public async Task<Result<MemberToUpdateViewModel>> GetMemberToUpdatAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberid, ct);
            if (member is null)
                return Result<MemberToUpdateViewModel>.NotFound("Member Not Found");
            else
            {
                var model = _mapper.Map<Member, MemberToUpdateViewModel>(member);
                return Result<MemberToUpdateViewModel>.OK(model);
            }
        }

        public async Task<Result<MemberViewModel>> MemberDetailsAsync(int memberid, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberid, ct);
            if (member is null)
                return Result<MemberViewModel>.NotFound("Member Not Found");
            var model = _mapper.Map<Member, MemberViewModel>(member);

            var ActtiveMemberShip = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(m => m.Id == memberid && m.EndDate > DateTime.Now, ct: ct);

            if (ActtiveMemberShip is not null)
            {
                var ActivePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(ActtiveMemberShip.PlanId, ct);
                model.PlanName = ActivePlan?.Name;
                model.MemberShipStartDate = ActtiveMemberShip.CreatedAt.ToShortDateString();
                model.MemberShipEndtDate = ActtiveMemberShip.EndDate.ToShortDateString();
            }
            return Result<MemberViewModel>.OK(model);

        }

        public async Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return Result.NotFound("Member Not Found");

            var hasFutureSession = await _unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == member.Id && b.Session.StartDate > DateTime.Now);
            if (hasFutureSession)
                return Result.Validation("Member Already Has Future Session");


            _unitOfWork.GetRepository<Member>().Delete(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Remove Member");
        }

        public async Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member is null) return Result.NotFound("Member Not Found");

            //Check Email
            if (await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return Result.Validation("Email Already Exist !!");
            //Check Phone
            if (await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return Result.Validation("Phone Already Exist !!");

            // else Update Member 
            _mapper.Map(model, member);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().Update(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Member");
        }
    }
}
