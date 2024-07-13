using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestEFHierarchyId.models;
using TestEFHierarchyId.TreeFunctions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestEFHierarchyId.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HierarchyDbContext _context;
        public static List<Location> StaticLocationList { get; set; }
        public LocationController(HierarchyDbContext context)
        {
            _context = context;
            StaticLocationList = new List<Location>();
        }
        // GET: api/<LocationController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            var list3 = _context.Storages.Include(x => x.Location).Select(x => new
            {
                StorageName = x.StorageName,
                HierarchyId = x.Location.HierarchyId,
                //Ancestors = _context.Locations.Where(c=>c.HierarchyId.GetAncestor(1)==x.Location.HierarchyId).AsEnumerable()
                Ancestors = _context.Locations.Where(ancestor => _context.Locations
                .SingleOrDefault(descendent => descendent.Id == x.LocationId && ancestor.Id != descendent.Id)
                .HierarchyId.IsDescendantOf(ancestor.HierarchyId))
                            .OrderByDescending(ancestor => ancestor.HierarchyId.GetLevel()).ToList()
            }).ToList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine(elapsedMs.ToString());
            return new string[] { "value1", "value2" };
        }


        [HttpGet("Test1")]
        public IActionResult Test1()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            var list3 = _context.Storages.Include(x => x.Location).AsNoTracking().Select(x => new
            {
                StorageName = x.StorageName,
                HierarchyId = x.Location.HierarchyId,
                //Ancestors = _context.Locations.Where(c=>c.HierarchyId.GetAncestor(1)==x.Location.HierarchyId).AsEnumerable()
                Ancestors = _context.Locations.Where(ancestor => _context.Locations
                .SingleOrDefault(descendent => descendent.Id == x.LocationId && ancestor.Id != descendent.Id)
                .HierarchyId.IsDescendantOf(ancestor.HierarchyId))
                            .OrderByDescending(ancestor => ancestor.HierarchyId.GetLevel()).ToList()
            }).ToList();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("get all storage in:" + elapsedMs.ToString());
            return Ok(list3.Take(20));
            // return new string[] { "value1", "value2" };
        }



        [HttpGet("Test2")]
        public async Task<IActionResult> Test2()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            var ancestors = await _context.Locations.Include(x => x.Parent)
                        .Where(x => x.Id == 7).ToListAsync();

            var ancestors2 = await _context.Locations
              .FromSqlRaw(@"
        WITH RECUR AS
        (
            SELECT Id, Title, ParentId,HierarchyId
            FROM Locations
            WHERE Id = {0}
            UNION ALL
            SELECT e.Id, e.Title, e.ParentId,e.HierarchyId
            FROM Locations e
            INNER JOIN RECUR r ON e.Id = r.ParentId
        )
        SELECT Id, Title,HierarchyId,ParentId
        FROM RECUR", 7)
    .ToListAsync();

            foreach (var ancestor in ancestors2)
            {
                Console.WriteLine($"Ancestor: {ancestor.Title}");
            }

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("get all storage in:" + elapsedMs.ToString());
            return Ok(ancestors);
            // return new string[] { "value1", "value2" };
        }

        [HttpGet("Test3")]
        public async Task<IActionResult> Test3()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            var query = await _context.Storages
                        .Include(x => x.Location)
                        .ThenInclude(x => x.Parent)
                        .ThenInclude(x => x.Parent)
                        .ThenInclude(x => x.Parent)
                        .AsNoTracking().ToListAsync();

            //query.Select(x => new
            //{
            //    StorageName = x.StorageName,
            //    //Ancestors = 
            //}).ToList();

            var result = new List<object>();
            foreach (var item in query)
            {
               var s=  TestInclude(item.Location, new List<string>());
                result.Add(new
                {
                    StorageName = item.StorageName,
                    Ancestors = string.Join("/", s)
                });
              //  Console.WriteLine("//////////////////////////");
            }
           // TestInclude(item.Location);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("get all storage in:" + elapsedMs.ToString()+"ms");
            return Ok(result.Take(5));
            // return new string[] { "value1", "value2" };
        }

        static List<string> TestInclude(Location item, List<string> list)
        {
            if (item != null)
            {
               // Console.WriteLine(item.Title);
                list.Add(item.Title);
                TestInclude(item.Parent,list);
            }
            return list;
        }


        [HttpPost("InsertStorage1000")]
        public IActionResult InsertStorage1000()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // the code that you want to measure comes here

            for (int i = 0; i < 1000; i++)
            {
                _context.Storages.Add(new Storage
                {
                    LocationId = 4,
                    StorageName = $"StorageZamzam1{i.ToString()}"
                });
            }
            _context.SaveChanges();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("SaveChanges:" + elapsedMs.ToString());
            return Ok();
            // return new string[] { "value1", "value2" };
        }


        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public async Task<List<Location>> Get(int id)
        {
            //var item = await _context.Locations.FindAsync(3);
            var item = await _context.Locations.FindAsync(6);
            var list = await _context.Locations
                .Where(x => x.HierarchyId.GetAncestor(1) == item.HierarchyId)
                .ToListAsync();

            var list2 = await _context.Locations
            .Where(x => x.HierarchyId.IsDescendantOf(item.HierarchyId))
            //.Where(x=>x.Id!=3)
            .ToListAsync();

            var list3 = await _context.Storages.Include(x => x.Location).Select(x => new
            {
                StorageName = x.StorageName,
                HierarchyId = x.Location.HierarchyId,
                //Ancestors = _context.Locations.Where(c=>c.HierarchyId.GetAncestor(1)==x.Location.HierarchyId).AsEnumerable()
            }).ToListAsync();

            // Get all ancestors of an entity
            var FindAllAncestors = _context.Locations.Where(ancestor => _context.Locations
                   .SingleOrDefault(descendent => descendent.Title == "modul1" && ancestor.Id != descendent.Id)
                   .HierarchyId.IsDescendantOf(ancestor.HierarchyId))
           .OrderByDescending(ancestor => ancestor.HierarchyId.GetLevel());

            //Get all descendants of an entity
            var FindAllDescendents = _context.Locations.Where(
            descendent => descendent.HierarchyId.IsDescendantOf(
                _context.Locations
                    .Single(
                        ancestor =>
                            ancestor.Title == "ahya"
                            && descendent.Id != ancestor.Id)
                    .HierarchyId))
        .OrderBy(descendent => descendent.HierarchyId.GetLevel());

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

            //_context.Locations.Add(new Location
            //{
            //    Id = 4,
            //    ParentId =3,
            //    Title = "zamzam1",
            //    HierarchyId = HierarchyId.Parse("/2/1/")
            //});

            _context.SaveChanges();
        }

        // PUT api/<LocationController>/5
        [HttpPut("{id}")]
        public async Task Put(int id, [FromBody] string value)
        {
            var locationList = await GetLocations();
            StaticLocationList.AddRange(locationList);

            var root = locationList.ToList().GenerateTree(c => c.Id, c => c.ParentId);
            Test(root);

            foreach (var item in StaticLocationList)
            {
                var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == item.Id);
                if (location != null)
                {
                    location.HierarchyId = item.HierarchyId;
                    await _context.SaveChangesAsync();
                }

            }

        }

        static void Test(IEnumerable<TreeItem<Location>> categories, int deep = 0)
        {
            var indexItemInDeep = 1;
            foreach (var c in categories)
            {
                //Console.WriteLine(new String('\t', deep) + c.Item.Title+c.Item.HierarchyId+ " level:" + deep + " indexItemInDeep:" + indexItemInDeep);

                var itemStatic = StaticLocationList.FirstOrDefault(x => x.Id == c.Item.Id);
                if (itemStatic?.ParentId != null)
                {
                    var itemStaticParent = StaticLocationList.FirstOrDefault(x => x.Id == itemStatic.ParentId);
                    var key = GenerateHirarchyId(itemStaticParent.HierarchyId.ToString(), indexItemInDeep.ToString());
                    itemStatic.HierarchyId = HierarchyId.Parse(key);
                    Console.WriteLine(itemStatic.Title + "=>" + itemStatic.HierarchyId.ToString());
                }

                indexItemInDeep++;
                Test(c.Children, deep + 1);
            }
        }

        public static string GenerateHirarchyId(string parant, string indexLevel)
        {
            if (parant == "/")
            {
                var q = parant + indexLevel + "/";
                return q;
            }
            else
            {
                var q = parant + indexLevel + "/";
                return q;
            }
        }

        // DELETE api/<LocationController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        async Task<List<Location>> GetLocations()
        {
            var list = await _context.Locations.Include(x => x.Parent).AsNoTracking().ToListAsync();
            return list;
        }

    }
}
