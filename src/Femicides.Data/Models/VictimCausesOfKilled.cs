namespace Femicides.Data
{
    public partial class VictimCausesOfKilled
    {
        public int Id { get; set; }
        public int? VictimId { get; set; }
        public string Cause { get; set; }
        public virtual Victim Victim { get; set; }
    }
}
