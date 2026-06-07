using GymManagement.DAL.Data.Models;

namespace GymManagement.BLL.ViewModels.MemberViewModel
{
    public class MemberViewModel
    {
        public int Id { get; set; }

        public string? Photo { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }

        //Member Details
        public string DateOfBirth { get;  set; }
        public string Address {  get; set; }
        public string? PlanName { get; set; }
        public string? MemberShipStartDate { get; set; } = null;
        public string? MemberShipEndtDate { get; set; } = null;

    }
}
