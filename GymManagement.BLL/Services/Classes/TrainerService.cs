using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.BLL.ViewModels.TrainerViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            //Check Email
            if( await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email, ct))
            return Result.Validation("Email Already Exist !!");
           
            //Check Phone
            if( await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone, ct))
            return Result.Validation("Phone Already Exist !!");


            var trainer = _mapper.Map<CreateTrainerViewModel, Trainer>(model);

            _unitOfWork.GetRepository<Trainer>().Add(trainer);

            var result = await _unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.OK() : Result.Fail("Failed To Create Trainer");

        }

        public async Task<IEnumerable<TrainerViewModel>> GetTrainersAsync(CancellationToken ct)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            if (!trainers.Any())
                return [];
          
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdatAsync(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid, ct);
            if (trainer is null)
                return Result<TrainerToUpdateViewModel>.NotFound("Trainer Not Found");
            else
            {
                var model = _mapper.Map<TrainerToUpdateViewModel>(trainer);
                return Result<TrainerToUpdateViewModel>.OK(model);
            }

        }

        public async Task<Result<TrainerViewModel>> TrainerDetailsAsync(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerid,ct);
            if (trainer == null) 
                return Result<TrainerViewModel>.NotFound("Trainer Not Found");
            else
            {
                var model = _mapper.Map<TrainerViewModel>(trainer);
                return Result<TrainerViewModel>.OK(model);
            }

        }

        public async Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default) 
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null)
                return Result.NotFound("Trainer Not Found"); 

            //Check Email
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return Result.Validation("Email Already Exists !!");
            //Check Phone
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return Result.Validation("Phone Already Exists !!");


            _mapper.Map(model, trainer);

            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var reslt = await _unitOfWork.SaveChangesAsync(ct); ;

            return reslt > 0 ? Result.OK() :Result.Fail("Failed To Update Trainer");
        }

        public async Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null)
                return Result.NotFound("Trainer Not Found");
            ;

            var hasFutureSession = await _unitOfWork.GetRepository<Trainer>().AnyAsync(s => s.Id == trainer.Id && s.CreatedAt > DateTime.Now,ct);
            if (hasFutureSession)
                return Result.Validation("Trainer Has Future Session");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct); ;

            return result > 0 ? Result.OK() : Result.Fail("Failed To Remove Trainer");
        }
    }
}
