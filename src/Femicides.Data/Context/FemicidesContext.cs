using System;
using Microsoft.EntityFrameworkCore;

namespace Femicides.Data
{
    public partial class FemicidesContext : DbContext
    {
        public FemicidesContext(DbContextOptions<FemicidesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Perpetrator> Perpetrator { get; set; }
        public virtual DbSet<Victim> Victim { get; set; }
        public virtual DbSet<VictimCausesOfKilled> VictimCausesOfKilled { get; set; }
        public virtual DbSet<VictimMethodsOfKilled> VictimMethodsOfKilled { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CityConfiguration());
            modelBuilder.ApplyConfiguration(new PerpetratorConfiguration());
            modelBuilder.ApplyConfiguration(new VictimConfiguration());
            modelBuilder.ApplyConfiguration(new VictimCausesOfKilledConfiguration());
            modelBuilder.ApplyConfiguration(new VictimMethodsOfKilledConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
