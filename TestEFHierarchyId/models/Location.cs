using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestEFHierarchyId.models
{
    public class Location
    {
        public Location()
        {
            
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string? Title { get; set; }
        public HierarchyId? HierarchyId { get; set; }
        public virtual Location Parent { get; set; }
    }
}