using GymManagement.DAL.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Data.Models
{
    public abstract class GymUser : BaseEntity
    {
        public string Name { get; set; } =default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public DateOnly DateOfBirth { get; set; }
        public Address Address { get; set; }
        public Gender Gender { get; set; }
    }

    [Owned]
    public class Address
    {
        public int BuildingNumber { get; set; }
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;
    }
}
