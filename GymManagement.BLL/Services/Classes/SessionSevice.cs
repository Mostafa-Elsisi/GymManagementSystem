using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionSevice : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionSevice(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            // Start date must be earlier than end date
            if (model.StartDate >= model.EndDate)
                return Result.Validation("EndDate Must Be After StartDate");
            if (model.StartDate <= DateTime.Now)
                return Result.Validation("StartDate Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25)
                return Result.Validation("Capacity Must Be Between 1 : 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer == null)
                return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId, ct);
            if (category == null)
                return Result.NotFound("Category Not Found");

            var isvalid = Enum.TryParse<Specialty>(category.CategoryName, true, out var Categoryspecialty);
            if (!isvalid || trainer.Specialty != Categoryspecialty)
                return Result.Validation("Can Not Create This Session To This Trainer");

            var session = _mapper.Map<CreateSessionViewModel, Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);

            var result = await _unitOfWork.SaveChangesAsync(ct: ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Session");

        }


        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionAsync(CancellationToken ct = default)
        {
            
            var sessions = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory(ct:ct);
            if (sessions?.Any() != true)
                return null;

            sessions = sessions.OrderByDescending(x => x.StartDate);
            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
                // n + 1 Problem  
            }
            return mappedSessions;
        }


        public async Task<IEnumerable<CategorySelectViewModel>> GeCategoryForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);

        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategoryAsync(sessionId, ct: ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found");
            else
            {
                var mappedSession = _mapper.Map<Session, SessionViewModel>(session);
                mappedSession.AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
                return Result<SessionViewModel>.OK(mappedSession);
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Started");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session That Has Already Has Booking");

            var mappedSesion = _mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.OK(mappedSesion);

        }

        public async Task<Result> UpdateSesionAsync(int sessionId, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);

            if (session is null) return Result.NotFound("Session Not Found");

            if (model.StartDate >= model.EndDate)
                return Result.Validation("EndDate Must Be After StartDate");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("StartDate Must Be In The Future");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0)
                return Result.Fail("Can Not Update Session That Has Already Has Booking");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer == null)
                return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);

            var isvalid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var Categoryspecialty);
            if (!isvalid || trainer.Specialty != Categoryspecialty)
                return Result.Validation("Can Not Create This Session To This Trainer");

            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.Update(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.OK() : Result.Fail("Failed To Update Session");
        }

        public async Task<Result> RemoveSessionAsync(int id, CancellationToken ct)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);

            if (session == null)
                return Result.NotFound("Session Not Found");

            if (session.EndDate > DateTime.Now)
                return Result.Fail("Can Not Delete Session That Has Not Ended Yet");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id, ct);
            if (bookingCount > 0)
                return Result.Fail("Can Not Update Session That Has Already Has Booking");

            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Deletd Session");
        }
    }
}
