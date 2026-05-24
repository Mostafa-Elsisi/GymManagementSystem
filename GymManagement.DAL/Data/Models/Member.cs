namespace GymManagement.DAL.Data.Models
{
    public class Member : GymUser
    {
        public string? Photo { get; set; }

        // JoinDate == CreatedAt of BaseEntity

        #region Relationships
        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<Membership> Memberships { get; set; } = default!;

        public ICollection<Booking> MemberSessions { get; set; } = default!;
        #endregion
    }
}
