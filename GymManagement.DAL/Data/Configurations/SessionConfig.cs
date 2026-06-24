using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    internal class SessionConfig : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable(
             tb =>
             {
                 tb.HasCheckConstraint("SessionCapacityCheck", "Capacity Between 1 and 25");
                 tb.HasCheckConstraint("SessionEndDateCheck", "EndDate > StartDate");
             });
        }
    }
}
