namespace GymManagement.DAL.Data.Models
{
    public class Booking :BaseEntity
    {

        // BookingDate = CreatedAt Of BaseEntity
        public bool IsAttended { get; set; } = false;

        #region Relationships
        public Member Member { get; set; } = default!;
        public int MemberId { get; set; }

        public Session Session { get; set; } = default!;
        public int SessionId { get; set; }
        #endregion

    }
}
