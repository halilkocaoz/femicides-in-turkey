using System.Collections.Generic;

namespace Femicides.Data
{
    public partial class Perpetrator
    {
        public Perpetrator()
        {
            Victim = new HashSet<Victim>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Definition { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Victim> Victim { get; set; }
    }
}
