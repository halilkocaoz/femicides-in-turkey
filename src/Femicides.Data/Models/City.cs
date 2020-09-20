using System.Collections.Generic;

namespace Femicides.Data
{
    public partial class City
    {
        public City()
        {
            Victim = new HashSet<Victim>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Victim> Victim { get; set; }
    }
}
