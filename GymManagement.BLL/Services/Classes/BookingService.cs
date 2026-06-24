using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookindViewModel;
using GymManagement.BLL.ViewModels.MemberShipViewModel;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionAsync(CancellationToken ct = default)

        {
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(s => s.EndDate >= DateTime.UtcNow, ct);

            IEnumerable<SessionViewModel>? mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
                // n + 1 Problem  
            }
            return mappedSessions;
        }
        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct)
        {
            // Get all bookings and members
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.SessionId == sessionId, false, ct);
            var allMembers = bookings.Select(b => b.MemberId);

            // Filter and map available members
            var availableMembers = await _unitOfWork.GetRepository<Member>().GetAllAsync(m => !allMembers.Contains(m.Id));

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(availableMembers);
        }
        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId, ct);
            if (session == null)
                return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can't Cancel Booking for Session Already Started ");

            var hasActiveMemberShip = await _unitOfWork.MemberShipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.UtcNow, ct);
            if (!hasActiveMemberShip)
                return Result.Fail("Member Does not Have an Active MemberShip");

            var alreadyBooked = await _unitOfWork.BookingRepository.AnyAsync(b => b.SessionId == model.SessionId && b.MemberId == model.MemberId, ct);
            if (alreadyBooked)
                return Result.Fail("Member is Already Booked For this Session");

            var booked = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (booked >= session.Capacity)
                return Result.Fail("Session Is Full");


            _unitOfWork.BookingRepository.Add(new Booking
            {
                MemberId = model.MemberId,
                SessionId = model.SessionId,
                IsAttended = false,
                CreatedAt = DateTime.UtcNow

            });
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Booking");


        }


        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingsBySessionIdAsync(sessionId, ct);

            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            return booking.Select(booking => new MemberForSessionViewModel
            {
                MemberId = booking.MemberId,
                SessionId = booking.SessionId,
                BookingDate = booking.CreatedAt,
                MemberName = booking.Member.Name,
                IsAttended = session.StartDate > DateTime.Now ? false : booking.IsAttended
            }).ToList();
        }
        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can't Cancel Booking for Session Already Started ");

            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.SessionId == sessionId && b.MemberId == memberId, tracking: true, ct: ct);

            if (booking == null) return Result.NotFound("Booking Not Found");

            _unitOfWork.BookingRepository.Delete(booking);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.OK() : Result.Fail("Failed To Cancel Booking");
        }
        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct)
        {
            var boooking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct: ct);
            if (boooking == null)

                return Result.NotFound("Booking Not Found");

            boooking.IsAttended = true;
            boooking.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.BookingRepository.Update(boooking);

            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.OK() : Result.Fail("Failed To Mark Attended");

        }



        public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForongoingSessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingsBySessionIdAsync(sessionId, ct);
            if (booking == null)
                return null!;
            return booking.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt

            }).ToList();
        }


        public async Task<IEnumerable<MemberForSessionViewModel>> GetMemberForUpcomingSessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingsBySessionIdAsync(sessionId, ct);
            if (booking == null)
                return null!;
            return booking.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = b.SessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt,
                IsAttended = b.IsAttended,

            }).ToList();
        }

    }
}
