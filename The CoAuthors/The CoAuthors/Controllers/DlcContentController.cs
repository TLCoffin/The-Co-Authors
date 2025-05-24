using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace The_CoAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        // Show the user the DLC content if they're authorized
        [Authorize(Policy = "HasDLC")]
        [HttpGet("getContent")]
        public IActionResult GetContent()
        {
            return Ok("DLC content!");
        }
    }
}
