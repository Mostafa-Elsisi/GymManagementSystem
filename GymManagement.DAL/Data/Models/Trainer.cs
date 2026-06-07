using GymManagement.DAL.Data.Models.Enums;

namespace GymManagement.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        //HireDate == CreatedAt of BaseEntity

        public Specialty Specialty { get; set; }

        #region Relationships
        public ICollection<Session> Sessions { get; set; } = default!;

        #endregion
    }
}
