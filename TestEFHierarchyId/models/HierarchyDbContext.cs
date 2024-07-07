using Microsoft.EntityFrameworkCore;

namespace TestEFHierarchyId.models
{
    public class HierarchyDbContext: DbContext
    {
        public HierarchyDbContext(DbContextOptions<HierarchyDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Location> Locations { get; set; }

    }
}
