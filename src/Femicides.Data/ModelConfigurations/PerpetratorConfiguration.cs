using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Femicides.Data
{
    public class PerpetratorConfiguration : IEntityTypeConfiguration<Perpetrator>
    {
        public void Configure(EntityTypeBuilder<Perpetrator> entity)
        {

            entity.HasKey(e => e.Id)
                .HasName("Perpetrator_pk")
                .IsClustered(false);

            entity.Property(e => e.Definition).HasMaxLength(100);

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.Property(e => e.Status).HasMaxLength(100);

            entity.Property(e => e.Surname).HasMaxLength(100);

        }
    }
}