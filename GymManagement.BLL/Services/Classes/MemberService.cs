using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberService : IMemberService
    {
        private readonly IGenericRepository<Member> _memberRepo;
        private readonly IGenericRepository<Membership> _membershipRepo;
        private readonly IGenericRepository<Plan> _planRepo;
        private readonly IGenericRepository<HealthRecord> _healthRecordRepo;
        private readonly IGenericRepository<Booking> _bookingRepo;

        public MemberService(IGenericRepository<Member> memberRepo,
            IGenericRepository<Membership> memberShipRepo,
            IGenericRepository<Plan> planRepo,
            IGenericRepository<HealthRecord> healthRecordRepo,
            IGenericRepository<Booking> bookingRepo)
        {
            _memberRepo = memberRepo;
            _membershipRepo = memberShipRepo;
            _planRepo = planRepo;
            _healthRecordRepo = healthRecordRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            //Check Email
             var emailExist = await _memberRepo.AnyAsync(m => m.Email == model.Email, ct);
            //Check Phone
             var PhoneExist = await _memberRepo.AnyAsync(m => m.Phone == model.Phone, ct);

            // Email or Phone Exist Return False
            if (emailExist || PhoneExist) 
                return false;

            // else Return True Add Member
            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                }, 
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                }
            };

           var reslt = await _memberRepo.AddAsync(member);

            return reslt > 0;

        }

        public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync(CancellationToken ct = default)
        {
            var members = await _memberRepo.GetAllAsync(ct: ct);

            if (!members.Any()) return [];

            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Phone = m.Phone,
                //Photo = m.Photo,
                Gender = m.Gender.ToString()
            });

            return memberViewModels;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordAsync(int id, CancellationToken ct = default)
        {
            var Record = await _healthRecordRepo.FirstOrDefaultAsync(m => m.Id == id);
            if (Record == null) return null;
            else
            {
                return new HealthRecordViewModel
                {
                    Weight = Record.Weight,
                    Height = Record.Height,
                    BloodType = Record.BloodType,
                    Note = Record.Note
                }; 
            }
        }

        public async Task<MemberToUpdateViewModel?> GetMemberToUpdatAsync(int memberid, CancellationToken ct = default)
        {
            var Member = await _memberRepo.GetByIdAsync(memberid, ct);
            if (Member is null) return null;
            else
            {
                return new MemberToUpdateViewModel
                {
                    Name = Member.Name,
                    Email = Member.Email,
                    Phone = Member.Phone,
                    Photo = Member.Photo,
                    BuildingNumber = Member.Address.BuildingNumber,
                    City = Member.Address.City,
                    Street = Member.Address.Street,
                };
            }

        }

        public async Task<MemberViewModel?> MemberDetailsAsync(int memberid, CancellationToken ct = default)
        {
            var Member = await _memberRepo.GetByIdAsync(memberid, ct);
            if(Member is null ) return null;
            var ViewModel = new MemberViewModel
            {
                Name = Member.Name,
                Email = Member.Email,
                Phone = Member.Phone,
                DateOfBirth = Member.DateOfBirth.ToShortDateString(),
                Address = $"{Member.Address.BuildingNumber}-{Member.Address.Street}-{Member.Address.City}"
            };


            var ActtiveMemberShip = await _membershipRepo.FirstOrDefaultAsync(m => m.Id == memberid && m.EndDate>DateTime.Now,ct:ct);
            
            if (ActtiveMemberShip is not null )
            {
                var ActivePlan = await _planRepo.GetByIdAsync(ActtiveMemberShip.PlanId, ct);
                ViewModel.PlanName = ActivePlan?.Name;
                ViewModel.MemberShipStartDate = ActtiveMemberShip.CreatedAt.ToShortDateString();
                ViewModel.MemberShipEndtDate = ActtiveMemberShip.EndDate.ToShortDateString();
            }
            return ViewModel;
        
        }

        public async Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(id, ct);
            if (member is null) return false;

            var hasFutureSession =await _bookingRepo.AnyAsync(b => b.MemberId == member.Id && b.Session.StartDate > DateTime.Now );
            if( hasFutureSession ) 
                return false;
            var result  =await _memberRepo.DeleteAsync(member, ct);

            return result > 0;
        }

        public async Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _memberRepo.GetByIdAsync(id, ct);
            if (member is null) return false;
           
            //Check Email
            if( await _memberRepo.AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return false;
            //Check Phone
            if( await _memberRepo.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return false;

            // else Update Member 
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.Address.BuildingNumber = model.BuildingNumber;
            member.UpdatedAt = DateTime.Now;
            var reslt = await _memberRepo.UpdateAsync(member);

            return reslt > 0;
        }
    }
}
