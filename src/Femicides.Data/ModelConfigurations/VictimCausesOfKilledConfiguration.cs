using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Femicides.Data
{
    public class VictimCausesOfKilledConfiguration : IEntityTypeConfiguration<VictimCausesOfKilled>
    {
        public void Configure(EntityTypeBuilder<VictimCausesOfKilled> entity)
        {

            entity.HasKey(e => e.Id)
                .HasName("VictimCausesOfKilled_pk")
                .IsClustered(false);

            entity.Property(e => e.Cause).HasMaxLength(100);

            entity.HasOne(d => d.Victim)
                .WithMany(p => p.VictimCausesOfKilled)
                .HasForeignKey(d => d.VictimId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("VictimCauseOfKilled_Victim_Id_fk");

        }
    }
}