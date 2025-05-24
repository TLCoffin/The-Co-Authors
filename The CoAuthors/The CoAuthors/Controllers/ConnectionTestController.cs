using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using The_CoAuthors.Models;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace The_CoAuthors.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConnectionTestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConnectionTestController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("/api/SeedItems")]
        public async Task<IActionResult> SeedItems()
        {
            List<Item> itemList = new List<Item>() {
                new Item { Name = "Book", Description = "Contains torn note pages" },
                new Item { Name = "Glasses", Description = "Grandma's broken glasses" },
                new Item { Name = "Necklace", Description = "Strange ancient jewelery" },
                new Item { Name = "Key", Description = "Opens the next room" }
            };

            foreach (Item item in itemList)
            {
                _context.Items.Add(item);
            }

            await _context.SaveChangesAsync();
            return Ok(itemList);
        }
    }
}
