namespace Femicides.Data
{
    public partial class VictimMethodsOfKilled
    {
        public int Id { get; set; }
        public int? VictimId { get; set; }
        public string Method { get; set; }
        public virtual Victim Victim { get; set; }
    }
}
