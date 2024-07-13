using Microsoft.AspNetCore.Mvc;
using TestEFHierarchyId.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestEFHierarchyId.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HierarchyController : ControllerBase
    {
        private readonly HierarchyService _service;

        public HierarchyController(HierarchyService service)
        {
            _service = service;
        }

        [HttpPost("AddItemToNode")]
        public async Task<IActionResult> AddItemToNode()
        {
            await _service.AddItemToNode(new models.Location
            {
                ParentId = 7,
                Title = "mou11"
            });
            return Ok();
        }

        [HttpGet("GetAllAncestors")]
        public async Task<IActionResult> GetAllAncestors()
        {
          var result=  await _service.GetAllAncestors(7); //modul1
            return Ok(result);
        }

        [HttpGet("GetAllDescendant")]
        public async Task<IActionResult> GetAllDescendant()
        {
            var result = await _service.GetAllDescendant(2); //ahya
            return Ok(result);
        }


    }
}
