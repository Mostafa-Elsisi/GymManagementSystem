using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Data.Configurations
{
    internal class MembershipConfig : IEntityTypeConfiguration<Membership>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Membership> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("StartDate")
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}
