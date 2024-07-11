namespace TestEFHierarchyId.models
{
    public class Storage
    {
        public int Id { get; set; }
        public int? LocationId { get; set; }
        public string? StorageName { get; set; }
        //public virtual ICollection<Location> Locations { get; set; }
        public virtual Location Location { get; set; }
    }
}
