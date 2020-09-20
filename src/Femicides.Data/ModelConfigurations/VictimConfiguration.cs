using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Femicides.Data
{
    public class VictimConfiguration : IEntityTypeConfiguration<Victim>
    {
        public void Configure(EntityTypeBuilder<Victim> entity)
        {

            entity.HasKey(e => e.Id)
                .HasName("Victim_pk")
                .IsClustered(false);

            entity.Property(e => e.Date).HasColumnType("date");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Surname).HasMaxLength(100);

            entity.HasOne(d => d.City)
                .WithMany(p => p.Victim)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Victim_City_Id_fk");

            entity.HasOne(d => d.Perpetrator)
                .WithMany(p => p.Victim)
                .HasForeignKey(d => d.PerpetratorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("Victim_Perpetrator_Id_fk");

        }
    }
}