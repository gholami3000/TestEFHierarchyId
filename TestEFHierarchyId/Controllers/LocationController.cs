using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestEFHierarchyId.models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestEFHierarchyId.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HierarchyDbContext _context;
        public LocationController(HierarchyDbContext context)
        {
            _context = context;
        }
        // GET: api/<LocationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public async Task<List<Location>> Get(int id)
        {
            var item = await _context.Locations.FindAsync(3);
            var list = await _context.Locations
                .Where(x => x.HierarchyId.GetAncestor(1) == item.HierarchyId)
                .ToListAsync();

            var list2 = await _context.Locations
            .Where(x => x.HierarchyId.IsDescendantOf(item.HierarchyId))
            .ToListAsync();

  

            return list;
        }

        // POST api/<LocationController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            //_context.Locations.Add(new Location
            //{
            //    Id=1,
            //    ParentId=null,
            //    Title="foolad",
            //    HierarchyId= HierarchyId.Parse("/")
            //});
            //_context.Locations.Add(new Location
            //{
            //    Id=2,
            //    ParentId=1,
            //    Title="ahya",
            //    HierarchyId = HierarchyId.Parse("/1/")

            //});
            //_context.Locations.Add(new Location
            //{
            //    Id=3,
            //    ParentId=1,
            //    Title="zamzam",
            //    HierarchyId = HierarchyId.Parse("/2/")
            //});

            _context.Locations.Add(new Location
            {
                Id = 4,
                ParentId =3,
                Title = "zamzam1",
                HierarchyId = HierarchyId.Parse("/2/1/")
            });

            _context.SaveChanges();
        }

        // PUT api/<LocationController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LocationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
