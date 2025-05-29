using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using The_CoAuthors.Models;
using static The_CoAuthors.Models.DataTransfers;

namespace The_CoAuthors.wwwroot
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerInventoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PlayerInventoryController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // add connection to js and collect the name of what item the user clicked
        // add that item the user clicked into the players db by query.
        [HttpPost("AddToUserInventory")]
        public async Task<IActionResult> AddToUserInventory([FromBody] string itemId)
        {
            string itemName = itemId.Replace("Template", "").ToLower();

            var currentUserId = _userManager.GetUserId(User);

            var userInventory = await _context.UserInventories
                .Include(u => u.Items)
                .FirstOrDefaultAsync(u => u.UserId == currentUserId);

            if (userInventory == null)
            {
                userInventory = new UserInventory
                {
                    UserId = currentUserId,
                    Items = new List<Item>()
                };
                _context.UserInventories.Add(userInventory);
                await _context.SaveChangesAsync();
            }

            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == itemName);

            if (item == null)
            {
                return BadRequest($"Item '{itemName}' not found in the database.");
            }

            if (!userInventory.Items.Any(i => i.Id == item.Id))
            {
                userInventory.Items.Add(item);
                await _context.SaveChangesAsync();
            }

            return Ok(userInventory);
        }

        [HttpGet("GetUserInventory")]
        [Authorize]
        public async Task<IActionResult> GetUserInventory()
        {
            var currentUserId = _userManager.GetUserId(User);

            var items = _context.UserInventories
                .Where(u => u.UserId == currentUserId)
                .SelectMany(u => u.Items)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Description
                })
                .ToList();

            return Ok(items);
        }

        [HttpGet("GetUserInventoryItems")]
        [Authorize]
        public async Task<IActionResult> GetUserInventoryItems()
        {
            var currentUserId = _userManager.GetUserId(User);

            var items = _context.UserInventories
                .Where(u => u.UserId == currentUserId)
                .SelectMany(u => u.Items)
                .Select(i => new
                {
                    i.Name
                })
                .ToList();

            return Ok(items);
        }
    }
}
