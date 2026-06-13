namespace GymManagement.DAL.Data.Models
{
    public class Membership : BaseEntity
    {
        // StartDate = CreatedAt of BaseEntity
        public DateTime EndDate { get; set; }

        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now;

        #region Relationships

        public Member Member { get; set; }
        public int MemberId { get; set; }

        public Plan Plan { get; set; }
        public int PlanId { get; set; }

        #endregion
    }
}
