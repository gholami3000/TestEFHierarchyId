namespace TestEFHierarchyId.models
{
    public class LocationDto
    {
        public LocationDto(int id, int? parentId, string name)
        {
            Id = id;
            ParentId = parentId;
            Name = name;
        }
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
