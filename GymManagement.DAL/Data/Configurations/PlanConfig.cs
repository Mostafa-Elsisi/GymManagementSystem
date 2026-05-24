using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.Data.Configurations
{
    public class PlanConfig : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(x => x.Name)
                    .HasColumnType("varchar")
                    .HasMaxLength(50);
           
            builder.Property(x => x.Description)
                    .HasMaxLength(100);
            
            builder.Property(x => x.Price)
                    .HasPrecision(10,2);

            builder.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Plan_DurationDays", "DurationDays Between 1 and 365");
            });


        }
    }
}
