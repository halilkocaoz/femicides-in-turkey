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
        public string Url
        {
            get
            {
                return "https://femicidesinturkey.com/api/city/" + Id;
            }
        }
        public string FilterUrl
        {
            get
            {
                return "https://femicidesinturkey.com/api/victim?city=" + Name;
            }
        }
        public virtual ICollection<Victim> Victim { get; set; }
    }
}
