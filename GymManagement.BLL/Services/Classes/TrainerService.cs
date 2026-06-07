using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModel;
using GymManagement.BLL.ViewModels.PlanViewModel;
using GymManagement.BLL.ViewModels.TrainerViewModel;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IGenericRepository<Trainer> _trainerRepo;

        public TrainerService(IGenericRepository<Trainer> trainerRepo)
        {
            _trainerRepo = trainerRepo;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            //Check Email
            if( await _trainerRepo.AnyAsync(m => m.Email == model.Email, ct))
            return false;
           
            //Check Phone
            if( await _trainerRepo.AnyAsync(m => m.Phone == model.Phone, ct))
            return false;
           
               

            // else Return True Add Trainer
            var trainer = new Trainer()
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
                Specialty = model.Specialty,
            };

            var reslt = await _trainerRepo.AddAsync(trainer);

            return reslt > 0;

        }

        public async Task<IEnumerable<TrainerViewModel>> GetTrainersAsync(CancellationToken ct)
        {
            var trainers = await _trainerRepo.GetAllAsync(ct: ct);
            if (!trainers.Any())
                return [];
            var trainersViewModels = trainers.Select(
                t => new TrainerViewModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Email = t.Email,
                    Phone = t.Phone,
                    Specialization = t.Specialty.ToString()
                }
                );
            return trainersViewModels;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdatAsync(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(trainerid, ct);
            if (trainer is null) return null;
            else
            {
                return new TrainerToUpdateViewModel()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    BuildingNumber = trainer.Address.BuildingNumber,
                    City = trainer.Address.City,
                    Street = trainer.Address.Street,
                    Specialty = trainer.Specialty
                };
            }

        }

        public async Task<TrainerViewModel?> TrainerDetailsAsync(int trainerid, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(trainerid,ct);
            if (trainer == null) return null;
            else
            {
                return new TrainerViewModel
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                    Specialization = trainer.Specialty.ToString(),
                    Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
                };
            }
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default) 
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            if (trainer is null) return false;

            //Check Email
            if (await _trainerRepo.AnyAsync(m => m.Email == model.Email && m.Id != id, ct))
                return false;
            //Check Phone
            if (await _trainerRepo.AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct))
                return false;

            // else Update Member 
            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.City = model.City;
            trainer.Address.Street = model.Street;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Specialty = model.Specialty;
            trainer.UpdatedAt = DateTime.Now;
            
            var reslt = await _trainerRepo.UpdateAsync(trainer);

            return reslt > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerRepo.GetByIdAsync(id, ct);
            if (trainer is null) return false;

            var hasFutureSession = await _trainerRepo.AnyAsync(s => s.Id == trainer.Id && s.CreatedAt > DateTime.Now,ct);
            if (hasFutureSession)
                return false;
           
            var result = await _trainerRepo.DeleteAsync(trainer, ct);

            return result > 0;
        }
    }
}
