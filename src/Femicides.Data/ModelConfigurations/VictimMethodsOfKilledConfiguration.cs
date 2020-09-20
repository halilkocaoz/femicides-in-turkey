using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Femicides.Data
{
    public class VictimMethodsOfKilledConfiguration : IEntityTypeConfiguration<VictimMethodsOfKilled>
    {
        public void Configure(EntityTypeBuilder<VictimMethodsOfKilled> entity)
        {

            entity.HasKey(e => e.Id)
                .HasName("VictimMethodsOfKilled_pk")
                .IsClustered(false);

            entity.Property(e => e.Method).HasMaxLength(100);

            entity.HasOne(d => d.Victim)
                .WithMany(p => p.VictimMethodsOfKilled)
                .HasForeignKey(d => d.VictimId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("VictimMethodsOfKilled_Victim_Id_fk");

        }
    }
}