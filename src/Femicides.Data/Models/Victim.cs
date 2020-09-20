using System;
using System.Collections.Generic;

namespace Femicides.Data
{
    public partial class Victim
    {
        public Victim()
        {
            VictimCausesOfKilled = new HashSet<VictimCausesOfKilled>();
            VictimMethodsOfKilled = new HashSet<VictimMethodsOfKilled>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool? Adult { get; set; }
        public bool? ProtectionRequest { get; set; }
        public int? PerpetratorId { get; set; }
        public short CityId { get; set; }
        public DateTime Date { get; set; }
        public virtual City City { get; set; }
        public virtual Perpetrator Perpetrator { get; set; }
        public virtual ICollection<VictimCausesOfKilled> VictimCausesOfKilled { get; set; }
        public virtual ICollection<VictimMethodsOfKilled> VictimMethodsOfKilled { get; set; }
    }
}
