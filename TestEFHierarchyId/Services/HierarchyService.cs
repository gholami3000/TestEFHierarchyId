using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using TestEFHierarchyId.models;

namespace TestEFHierarchyId.Services
{
    public class HierarchyService
    {
        private readonly HierarchyDbContext _context;

        public HierarchyService(HierarchyDbContext context)
        {
            _context = context;
        }

        public async Task AddItemToNode(Location newItem)
        {
            var parentItem = await _context.Locations.FirstOrDefaultAsync(x => x.Id == newItem.ParentId);

            var hierarchyId = await GenerateChildByParentId(newItem.ParentId.Value);
            if (hierarchyId != null)
            {
                _context.Locations.Add(new Location
                {
                    ParentId = newItem.ParentId,
                    HierarchyId = HierarchyId.Parse(hierarchyId),
                    Title = newItem.Title
                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GenerateChildByParentId(int parentId)
        {
            var parentItem = await _context.Locations.FirstOrDefaultAsync(x => x.Id == parentId);
            var parentHierarchyId = parentItem.HierarchyId.ToString();

            var childList = await _context.Locations
            .Where(x => x.HierarchyId.IsDescendantOf(parentItem.HierarchyId))
            .Where(x => x.Id != parentItem.Id)
            .ToListAsync();

            var newItemHierarchyId = string.Empty;

            for (var i = childList.Count + 1; i < childList.Count + 10; i++)
            {
                newItemHierarchyId = parentHierarchyId + (i).ToString() + "/";
                var item = childList.FirstOrDefault(x => x.HierarchyId.ToString() == newItemHierarchyId);
                if (item is null)
                {
                    return newItemHierarchyId;
                }
            }

            return null;
        }

        /// <summary>
        ///  Get all ancestors of an entity
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<List<Location>> GetAllAncestors(int parentId)
        {
            var allAncestors = await _context.Locations.Where(ancestor => _context.Locations
                   .Single(descendent =>
                                        descendent.ParentId == parentId
                                        && ancestor.Id != descendent.Id)
                   .HierarchyId.IsDescendantOf(ancestor.HierarchyId))
           .OrderByDescending(ancestor => ancestor.HierarchyId.GetLevel()).ToListAsync();

            return allAncestors;
        }

        /// <summary>
        ///  Get all descendants of an entity
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<List<Location>> GetAllDescendant(int parentId)
        {
            var allDescendents = await _context.Locations.Where(
            descendent => descendent.HierarchyId.IsDescendantOf(
                _context.Locations
                    .Single(ancestor =>
                            ancestor.ParentId == parentId
                            // && descendent.Id != ancestor.Id
                            )
                    .HierarchyId))
        .OrderBy(descendent => descendent.HierarchyId.GetLevel()).ToListAsync();

            return allDescendents;
        }

    }
}
